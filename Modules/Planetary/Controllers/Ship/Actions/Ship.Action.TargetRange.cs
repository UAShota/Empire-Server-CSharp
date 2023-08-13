/////////////////////////////////////////////////
//
// Поиск цели на другой планете по маркеру 
// наведения союзного кораблика
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
    /// Класс наведения артилерии
    /// </summary>
    internal class ActionTargetRange : PlanetaryAccess
    {
        /// <summary>
        /// Попытка нацелиться на подсвеченную цель
        /// </summary>
        /// <param name="aShip">Кораблик подсветки</param>
        /// <param name="aTarget">Кораблик цель</param>
        /// <returns>Разрешение на прицеливание</returns>
        private bool RetargetToHighLight(Ship aShip, out Ship aTarget)
        {
            if ((aShip != null) && (!aShip.TechActive(ShipTech.RangeDefence)))
            {
                aTarget = aShip;
                return true;
            }
            else
            {
                aTarget = null;
                return false;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionTargetRange(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Поиск цели для кораблика на указанной планете
        /// </summary>
        /// <param name="aShip">Артилерия</param>
        /// <param name="aPlanet">Планета прицеливания</param>
        /// <param name="aAutoTarget">Автоприцеливание без наводки</param>
        /// <returns>Успешность прицеливания</returns>
        public bool Highlight(Ship aShip, Planet aPlanet, bool aAutoTarget)
        {
            Ship tmpTargetRocket = null;
            // Поиск корабля противника, которого подсветил наш кораблик
            foreach (Ship tmpShip in aPlanet.Ships)
            {
                // Пропускаем не нацеленные
                if (!tmpShip.IsTargeted)
                    continue;
                // Пропускаем не свои
                if (!tmpShip.Owner.IsRoleFriend(aShip.Owner))
                    continue;
                // Проверим центральную цель
                if (RetargetToHighLight(tmpShip.TargetCenter, out tmpTargetRocket))
                    break;
                // Проверим левую цель
                if (RetargetToHighLight(tmpShip.TargetLeft, out tmpTargetRocket))
                    break;
                // Проверим правую цель
                if (RetargetToHighLight(tmpShip.TargetRight, out tmpTargetRocket))
                    break;
            }
            // Проверим смены цели
            if (aShip.TargetRocket != tmpTargetRocket)
            {
                // Приаттачим автонацеленный коаблик
                if (aAutoTarget)
                {
                    if (tmpTargetRocket != null)
                        Engine.Ships.Action.Attach.Call(aShip, aPlanet, true);
                    else
                        Engine.Ships.Action.Attach.Call(aShip, null, true);
                }
                aShip.TargetRocket = tmpTargetRocket;
                // Отправим смену цели
                Engine.SocketWriter.ShipRetarget(aShip, aShip.TargetRocket, ShipWeaponType.Rocket);
            }
            // Вернем результат
            return (tmpTargetRocket != null);
        }

        /// <summary>
        /// Поиск свободной артилерии для прицела на указанную планету
        /// </summary>
        /// <param name="aPlanet">Целевая планета</param>
        public void SearchRangers(Planet aPlanet)
        {
            // Игнорируем планеты без боя
            foreach (Planet tmpPlanet in aPlanet.Links)
            {
                // Игнорируем планеты с боем
                if (tmpPlanet.TimerEnabled(PlanetTimer.Battle))
                    continue;
                // Переберем все корабли планеты
                foreach (Ship tmpShip in tmpPlanet.Ships)
                {
                    if (!tmpShip.IsActive)
                        continue;
                    if (tmpShip.Attach != null)
                        continue;
                    if (tmpShip.TechActive(ShipTech.WeaponRocket))
                        Highlight(tmpShip, aPlanet, true);
                }
            }
        }

        /// <summary>
        /// Автоматический поиск цели для кораблика на соседних планетах
        /// </summary>
        /// <param name="aShip">Кораблик артилерии</param>
        public void SearchTargets(Ship aShip)
        {
            // Если кораблик привязан - то ищем только там, куда привязан
            if (aShip.Attach != null)
            {
                Highlight(aShip, aShip.Attach, false);
                return;
            }
            // Игнорируем планеты без боя
            foreach (Planet tmpPlanet in aShip.Planet.Links)
            {
                if (!tmpPlanet.TimerEnabled(PlanetTimer.Battle))
                    continue;
                // Если кораблик прицелен, больше не ищем
                if (Highlight(aShip, tmpPlanet, true))
                    break;
            }
        }
    }
}