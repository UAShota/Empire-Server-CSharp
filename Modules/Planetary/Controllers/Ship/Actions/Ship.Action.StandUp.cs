/////////////////////////////////////////////////
//
// Учет добавления кораблика на планетоид
// и попытка перевести в боевой режим
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
    /// Класс перехода в активный режим
    /// </summary>
    internal class ActionStandUp : PlanetaryAccess
    {
        /// <summary>
        /// Проверка на необходимость заблокировать соседний кораблик
        /// </summary>
        /// <param name="aShip">Проверяемый кораблик</param>
        private void CheckBlock(Ship aShip)
        {
            // Есть ли вражеский кораблик для блокирования справа и наш еще правее
            if (Engine.Ships.Action.Utils.CheckShipBlocker(aShip, aShip.Landing.Prev, true, ShipMode.Active, out Ship tmpTargetShip))
            {
                if (Engine.Ships.Action.Utils.CheckShipBlocker(aShip, aShip.Landing.Prev.Prev, false, ShipMode.Active, out _))
                    Engine.Ships.Action.StandDown.Call(tmpTargetShip, ShipMode.Blocked);
            }
            // Есть ли кораблик для блокирования слева
            if (Engine.Ships.Action.Utils.CheckShipBlocker(aShip, aShip.Landing.Next, true, ShipMode.Active, out tmpTargetShip))
            {
                if (Engine.Ships.Action.Utils.CheckShipBlocker(aShip, aShip.Landing.Next.Next, false, ShipMode.Active, out _))
                    Engine.Ships.Action.StandDown.Call(tmpTargetShip, ShipMode.Blocked);
            }
        }

        /// <summary>
        /// Проверка на необходимость самозаблокироваться
        /// </summary>
        /// <param name="aShip">Проверяемый кораблик</param>
        /// <returns>Кораблик должен быть заблокирован</returns>
        private void CheckBlocked(Ship aShip)
        {
            // Есть ли кораблик для блокирования справа
            if (!Engine.Ships.Action.Utils.CheckShipBlocker(aShip, aShip.Landing.Prev, true, ShipMode.Active, out _))
                return;
            // Есть ли кораблик для блокирования слева
            if (!Engine.Ships.Action.Utils.CheckShipBlocker(aShip, aShip.Landing.Next, true, ShipMode.Active, out _))
                return;
            // Если есть два противника - блокируемся
            Engine.Ships.Action.StandDown.Call(aShip, ShipMode.Blocked);
        }

        /// <summary>
        /// Обработка возможных действий кораблика после подьема в стойку
        /// </summary>
        /// <param name="aShip">Обрабатываемый кораблик</param>
        private void Activate(Ship aShip)
        {
            // Закрасим
            Engine.Planets.Action.Control.Call(aShip.Landing.Planet, aShip.Owner, true, aShip.Landing.IsLowOrbit);
            // При отправке например с ангара это не нужно
            if (aShip.Planet.TimerEnabled(PlanetTimer.Battle))
            {
                // Проверим, какие кораблики заблокировал этот кораблик и отправим закраску
                CheckBlock(aShip);
                // Проверим что можно подняться, если поднимается дева - ищем автоатаку
                CheckBlocked(aShip);
            }
            // Переприцелим планету
            Engine.Ships.Action.Battle.Call(aShip);
            // Начнем захват если можно
            Engine.Planets.Action.Capture.Call(aShip);
        }

        /// <summary>
        /// Пересчет количества активных и доступных стеков
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aChangeMode">Разрешение менять режим</param>
        /// <param name="aChangeCount">Разрешение менять количество</param>
        private void Recalc(Ship aShip, bool aChangeMode, bool aChangeCount)
        {
            // По умолчанию кораблик есть и он активен
            bool tmpFlagFound = false;
            // Сперва добавим кораблик во все списки как активный
            foreach (var tmpIndex in aShip.Planet.ShipCount)
            {
                // Добавляем только в списки союзных войск
                if (!aShip.Owner.IsRoleFriend(tmpIndex.Key))
                    continue;
                ShipCount tmpCount = tmpIndex.Value;
                // Определим, есть ли вообще свои стеки
                if (!tmpFlagFound)
                    tmpFlagFound = (tmpIndex.Key == aShip.Owner);
                // Активным делаем при явном указании либо избыточным если он уже активный
                if (aChangeMode)
                {
                    if (tmpCount.Active == Engine.Ships.MaxShipActive)
                        aShip.Mode = ShipMode.Full;
                    else
                        aShip.Mode = ShipMode.Active;
                }
                // Обновляем общее количество корабликов и количество активных
                if (aChangeCount)
                    tmpCount.Exist++;
                if (aShip.IsActive)
                    tmpCount.Active++;
            }
            // Если это первый кораблик - начнем список
            if (!tmpFlagFound)
            {
                ShipCount tmpCount = new ShipCount
                {
                    Exist = 1,
                    Active = aShip.IsActive ? 1 : 0
                };
                // Запишем назад
                aShip.Planet.ShipCount.Add(aShip.Owner, tmpCount);
            }
            // Для активных кораблей сменим контроль и проверим боевку
            if (aShip.IsActive)
                Activate(aShip);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый контроллер</param>
        public ActionStandUp(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Попытка поднять кораблик в активный режим
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aChangeState">Разрешено ли менять состояние</param>
        /// <param name="aChangeMode">Разрешение смены режима</param>
        /// <param name="aChangeCount">Разрешение смены количества флота игрока на планете</param>
        public void Call(Ship aShip, bool aChangeState = true, bool aChangeMode = true, bool aChangeCount = false)
        {
            if (aChangeState)
                aShip.State = ShipState.Available;
            // Если перемещение идет на нижний слот - добавление в сумму не нужно
            if (aShip.Landing.IsLowOrbit)
            {
                if (aChangeMode)
                    aShip.Mode = ShipMode.Active;
            }
            else
                Recalc(aShip, aChangeMode, aChangeCount);
            // При запросе активных действий, уведомим клиента
            if (aChangeState || aChangeMode)
                Engine.SocketWriter.ShipUpdateState(aShip);
        }
    }
}