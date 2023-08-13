/////////////////////////////////////////////////
//
// Постройка кораблика
//
// Copyright(c) 2016 UAShota
//
// Rev J  2019.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс постройки корабликов
    /// </summary>
    internal class ActionConstruction : PlanetaryAccess
    {
        /// <summary>
        /// Количество верфей, учитываемых для ускорения постройки
        /// </summary>
        private int ciMaxShipyardActive => 4;

        /// <summary>
        /// Интервал постройки корабля
        /// </summary>
        private int ciConstructTick => 1000;

        /// <summary>
        /// Событие срабатывания таймера постройки
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Остановка или продление таймера</returns>
        private int OnTimer(TimerObject aShip)
        {
            Ship tmpShip = (Ship)aShip;
            // Выставим стартовые состояния если кораблик не в состоянии полета
            Engine.Ships.Action.StandUp.Call(tmpShip, false);
            // Обновим параметры планеты для стационарных построек
            /*Engine.Planets.ShipList.Call(tmpShip, tmpShip.Count);*/
            // Остановим таймер
            return 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionConstruction(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Постройка кораблика
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aShipType">Тип кораблика</param>
        /// <param name="aSlot">Слот постройки</param>
        /// <param name="aCount">Количество корабликов</param>
        /// <param name="aCost">Цена постройки</param>/// 
        /// <param name="aPlayer">Инициирующий игрок</param>
        public void Call(Planet aPlanet, ShipType aShipType, Landing aSlot, int aCount, int aCost, Player aPlayer)
        {
            // Если все есть - построим кораблик
            Ship tmpShip = Engine.Ships.Action.Utils.CreateShip(aShipType, aCount, aPlayer);
            // Уменьшим количество затраченных ресурсов
            /*TPlanetThread(Engine).ControlStorages.DecrementResource(resModules, aPlanet, aCost);*/
            // Добавим созданный кораблик на планеты
            Engine.Ships.Action.Relocation.Add(tmpShip, aSlot, true, true);
            // Добавим таймер
            Engine.Ships.Action.Utils.TimerAdd(tmpShip, ShipTimer.Construction, OnTimer, ciConstructTick);
        }

        public void Recalc(Planet aPlanet, Player aPlayer)
        {
            /*   int tmpCount = 0;
               // Для кораблей нужны верфи, стационарки могут строиться без верфей
               foreach (Ship tmpShip in aShip.Planet.Ships)
               {
                   if (tmpShip.Owner == aShip.Owner)
                       tmpCount += aShip.TechValue(ShipTech.Construction);
               }
               if (!aShip.Planet.Constructors.TryGetValue(aShip.Owner, out int tmpCount))
               {
                   if (aShip.TechActive(ShipTech.Stationary))
                       tmpCount = 1;
                   else
                       return 0;
               }
               // Больше 4-х верфей строить нет смысла
               return Math.Min(tmpCount, ciMaxShipyardActive) * ;*/
        }
    }
}