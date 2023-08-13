/////////////////////////////////////////////////
//
// Путешествие через тоннель
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
    /// Класс путешествия через тоннель
    /// </summary>
    internal class ActionTravel : PlanetaryAccess
    {
        /// <summary>
        /// Время прыжка в портал ЧТ
        /// </summary>
        private int ciTimeTravel => 5000;

        /// <summary>
        /// Возвращение времени путешествия
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Время путешествия</returns>
        private int TravelTime(Ship aShip)
        {
            return ciTimeTravel;
        }

        /// <summary>
        /// Каллбак таймера перелета через портал
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Необходимость продлить таймер</returns>
        private int OnTimer(TimerObject aShip)
        {
            Ship tmpShip = (Ship)aShip;
            // Определим планету выхода
            Planet tmpPlanet = tmpShip.Path[0];
            // Если планета уже не активна, уничтожим стек
            if (tmpPlanet.State != PlanetState.Active)
            {
                /*Engine.Ships.Action.Relocation.Drop(tmpShip);*/
                return 0;
            }
            // Если слота для высадки нет - пусть покатаются еще
            if (Engine.Ships.Action.Utils.GetSlot(tmpPlanet, tmpShip.Owner, false, true, out Landing tmpLanding))
            {
                Engine.Ships.Action.Relocation.Add(tmpShip, tmpLanding, true, true);
                return 0;
            }
            else
                return TravelTime(tmpShip);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionTravel(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Путешествие по каналу
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        public void Call(Ship aShip)
        {
            // Установим путь прыжка
            aShip.Path = new List<Planet>
            {
                aShip.Portal
            };
            // Запустим таймер путешествия
            Engine.Ships.Action.Utils.TimerAdd(aShip, ShipTimer.PathHope, OnTimer, TravelTime(aShip));
            // Удалим кораблик с орбиты
            Engine.Ships.Action.Relocation.Remove(aShip, true);
            // Попросим удалить с созвездия
            Engine.SocketWriter.ShipDelete(aShip);
        }
    }
}