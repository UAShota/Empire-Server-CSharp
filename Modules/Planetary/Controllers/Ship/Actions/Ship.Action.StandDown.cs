/////////////////////////////////////////////////
//
// Учет удаления кораблика с планетоида и  
// попытка перевести в неактивное состояние
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс перехода в пассивный режим
    /// </summary>
    internal class ActionStandDown : PlanetaryAccess
    {
        /// <summary>
        /// Восстановление своих корабликов из переполнения
        /// </summary>
        /// <param name="aShip">Инициирующий кораблик</param>
        private void UpFriends(Ship aShip)
        {
            foreach (Ship tmpShip in aShip.Planet.Ships)
            {
                if (tmpShip.Mode != ShipMode.Full)
                    continue;
                if (!aShip.Owner.IsRoleFriend(tmpShip.Owner))
                    continue;
                // Если подходит - поднимаем
                Engine.Ships.Action.StandUp.Call(tmpShip);
                break;
            }
        }

        /// <summary>
        /// Восстановление вражеских корабликов из блокировки
        /// </summary>
        /// <param name="aShip">Инициирующий кораблик</param>
        private void UpEnemies(Ship aShip)
        {
            // Включим кораблик справа
            if (Engine.Ships.Action.Utils.CheckShipSide(aShip.Landing.Prev, ShipMode.Blocked, out Ship tmpShip))
                Engine.Ships.Action.StandUp.Call(tmpShip);
            // Включим кораблик справа
            if (Engine.Ships.Action.Utils.CheckShipSide(aShip.Landing.Next, ShipMode.Blocked, out tmpShip))
                Engine.Ships.Action.StandUp.Call(tmpShip);
        }

        /// <summary>
        /// Смена активности флота
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aMode">Режим инициации</param>
        /// <param name="aChangePlanet">Необходимость изменения количества стеков</param>
        private void ChangeState(Ship aShip, ShipMode aMode, bool aChangePlanet)
        {
            if (aChangePlanet)
            {
                // Если не принудительный оффлайн - сменим на актив
                if (aShip.Mode != ShipMode.Offline)
                    aShip.Mode = ShipMode.Active;
                // Задисаблим доступ
                aShip.State = ShipState.Disabled;
                Engine.SocketWriter.ShipUpdateState(aShip);
            }
            // При смене режима - тоже нужно уведомить
            else if (aShip.Mode != aMode)
            {
                aShip.Mode = aMode;
                Engine.SocketWriter.ShipUpdateState(aShip);
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionStandDown(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Удаление кораблика из учета количества или актива
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aMode">Режим инициации</param>
        /// <param name="aChangePlanet">Необходимость изменения количества стеков</param>
        public void Call(Ship aShip, ShipMode aMode, bool aChangePlanet = false)
        {
            // Нижние стеки не учитываются при удалении корабля
            if (aShip.Landing.IsLowOrbit)
                return;
            // А для боевых стеков проведем логику
            bool tmpFlagFound = false;
            // Переберем все записи количества
            foreach (var tmpIndex in aShip.Planet.ShipCount)
            {
                if (!tmpFlagFound)
                    tmpFlagFound = (tmpIndex.Key == aShip.Owner);
                // Найдем союзные стеки для обновления количество стеков на планете
                if (!aShip.Owner.IsRoleFriend(tmpIndex.Key))
                    continue;
                // Заберем количество стеков
                ShipCount tmpCount = tmpIndex.Value;
                // Удалим запись если это последний кораблик игрока
                if (aChangePlanet && (tmpCount.Exist == 1))
                {
                    aShip.Planet.ShipCount.Remove(tmpIndex.Key);
                    break;
                }
                // Если кораблик убит в полете - он еще пассивный и не учитывается как активный
                if (aChangePlanet)
                    tmpCount.Exist--;
                if (aShip.IsActive)
                    tmpCount.Active--;
            }
            // Уберем активность
            if (aShip.IsActive)
                Engine.Planets.Action.Control.Call(aShip.Planet, aShip.Owner, false, aShip.Landing.IsLowOrbit);
            // При удалении кораблика дисаблим сразу
            ChangeState(aShip, aMode, aChangePlanet);
            // Восстановим из перелимита свой стек (6 минус перелимит минус выключенный)
            UpFriends(aShip);
            // Разблокируем крайние вражеские
            UpEnemies(aShip);
            // Уберем автоприцел, если есть
            if (aShip.IsAutoTarget)
                Engine.Ships.Action.Attach.Call(aShip, null, true);
            // Переприцелим кораблики
            Engine.Ships.Action.Battle.Call(aShip);
        }
    }
}