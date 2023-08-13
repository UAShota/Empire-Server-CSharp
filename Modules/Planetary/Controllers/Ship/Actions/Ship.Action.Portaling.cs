/////////////////////////////////////////////////
//
// Прыжок в портал
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс прыжка в портал
    /// </summary>
    internal class ActionPortaling : PlanetaryAccess
    {
        /// <summary>
        /// Время прыжка в обычный портал
        /// </summary>
        private int ciTimePortal => 15000;

        /// <summary>
        /// Возвращение времени телепорта
        /// </summary>
        /// <param name="aShip">Телепортируемый кораблик</param>
        /// <returns>Время телепорта</returns>
        private int PortalTime(Ship aShip)
        {
            return ciTimePortal;
        }

        /// <summary>
        /// Каллбак таймера перелета через портал
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Необходимость продлить таймер</returns>
        private int OnTimer(TimerObject aShip)
        {
            Ship tmpShip = (Ship)aShip;
            // Попробуем отправить
            return !Engine.Ships.Action.Relocation.Jump(tmpShip, tmpShip.Portal) ? PortalTime(tmpShip) : 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionPortaling(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Прыжок или отмена прыжка в портал
        /// </summary>
        /// <param name="aShip">Телепортируемый кораблик</param>
        /// <param name="aBreak">Прыжок или отмена</param>
        public void Call(Ship aShip, bool aBreak)
        {
            // Прерывание прыжка
            if (aBreak)
            {
                Engine.Ships.Action.Utils.TimerRemove(aShip, ShipTimer.PortalJump);
                Engine.Ships.Action.StandUp.Call(aShip, true, false);
            }
            // Старт прыжка
            else
            {
                Engine.Ships.Action.Utils.TimerAdd(aShip, ShipTimer.PortalJump, OnTimer, PortalTime(aShip));
                Engine.Ships.Action.StandDown.Call(aShip, aShip.Mode);
            }
            // Обновим состояние
            Engine.SocketWriter.ShipUpdateState(aShip);
        }
    }
}