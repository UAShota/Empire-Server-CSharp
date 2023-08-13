/////////////////////////////////////////////////
//
// Обработка полетов флота
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс обработки полетов флота
    /// </summary>
    internal class ActionFly : PlanetaryAccess
    {
        /// <summary>
        /// Время перемещения между планетами
        /// </summary>
        private const int ciTimeMovingGlobal = 4000;

        /// <summary>
        /// Время перемещения на одной планете
        /// </summary>
        private const int ciTimeMovingLocal = 2000;

        /// <summary>
        /// Время штрафа парковки
        /// </summary>
        private const int ciTimeMovingParking = 2000;

        /// <summary>
        /// Каллбак таймера полета
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Время продления таймера</returns>
        private int OnTimer(TimerObject aShip)
        {
            Ship tmpShip = (Ship)aShip;
            // Проверим прилет на ЧТ
            if (tmpShip.Planet.Type == PlanetType.Hole)
            {
                Engine.Ships.Action.Travel.Call(tmpShip);
                return 0;
            }
            // Если не парковка - прицелим планету
            if ((tmpShip.State != ShipState.Interactive)
                && Engine.Ships.Action.Battle.Call(tmpShip))
            {
                Call(tmpShip, ShipFlyType.Parking);
                return 0;
            }
            // Не меняем режим если кораблик в ручной пассивке
            Engine.Ships.Action.StandUp.Call(tmpShip, true, tmpShip.Mode != ShipMode.Offline);
            return 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionFly(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Запуск кораблика в полет
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aFlyType">Тип перелета</param>
        public void Call(Ship aShip, ShipFlyType aFlyType)
        {
            Func<TimerObject, int> tmpOnTimer = OnTimer;
            int tmpTime;
            ShipTimer tmpTimer;
            // Установим время для полетов
            switch (aFlyType)
            {
                case ShipFlyType.Parking:
                    aShip.State = ShipState.Interactive;
                    tmpTime = ciTimeMovingParking;
                    tmpTimer = ShipTimer.FlightLocal;
                    break;
                case ShipFlyType.Local:
                    aShip.State = ShipState.Disabled;
                    tmpTime = ciTimeMovingLocal;
                    tmpTimer = ShipTimer.FlightLocal;
                    break;
                case ShipFlyType.Global:
                    aShip.State = ShipState.Disabled;
                    tmpTime = ciTimeMovingGlobal;
                    tmpTimer = ShipTimer.FlightGlobal;
                    Engine.Ships.Action.Fuel.Remove(aShip, 1);
                    break;
                default:
                    Core.Log.Error("Fly wrong type");
                    return;
            }
            Engine.Ships.Action.Utils.TimerAdd(aShip, tmpTimer, tmpOnTimer, tmpTime);
            // Сперва обновим состояние
            Engine.SocketWriter.ShipUpdateState(aShip);
        }
    }
}