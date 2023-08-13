/////////////////////////////////////////////////
//
// Обработка группы корабликов
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System.Collections.Generic;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс обработки группы корабликов
    /// </summary>
    internal class ActionPath : PlanetaryAccess
    {
        /// <summary>
        /// Время попытки перепрыгнуть дальше
        /// </summary>
        private int ciTimerHope => 1000;

        /// <summary>
        /// Завершение пути следования
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Успех завершения</returns>
        private int DropPath(Ship aShip)
        {
            aShip.Path = null;
            return 0;
        }

        /// <summary>
        /// Каллбак таймера полета
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Необходимость продлить таймер</returns>
        private int OnTimer(TimerObject aShip)
        {
            Ship tmpShip = (Ship)aShip;
            // Продлим таймер, если кораблик недоступен
            if (tmpShip.State != ShipState.Available)
                return ciTimerHope;
            // Определим планету назначения            
            Planet tmpPlanet = tmpShip.Path[0];
            // Дропнем если планета недоступна по раcтоянию
            if (!tmpShip.Planet.Links.Contains(tmpPlanet))
                return DropPath(tmpShip);
            // Попробуем переместить
            if (!Engine.Ships.Action.Relocation.Move(tmpShip, tmpPlanet.LandingZone.Corner(), false))
                return ciTimerHope;
            // Если переместить удалось - убираем планету с очереди
            tmpShip.Path.RemoveAt(0);
            // Если планета последняя - закроем таймер
            if (tmpShip.Path.Count == 0)
                return DropPath(tmpShip);
            else
                return /*tmpShip.TimerValue(ShipTimer.FlightGlobal) + */ciTimerHope;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionPath(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Добавленеи пути следования
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlanets">Список планет перелета</param>
        public void Add(Ship aShip, List<Planet> aPlanets)
        {
            aShip.Path = new List<Planet>(aPlanets);
            Engine.Ships.Action.Utils.TimerAdd(aShip, ShipTimer.PathHope, OnTimer, OnTimer(aShip));
        }

        /// <summary>
        /// Удаление пути следования
        /// </summary>
        /// <param name="aShip">Кораблик следования</param>
        public void Drop(Ship aShip)
        {
            DropPath(aShip);
            Engine.Ships.Action.Utils.TimerRemove(aShip, ShipTimer.PathHope);
        }
    }
}