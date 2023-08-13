/////////////////////////////////////////////////
//
// Команда игрока - прыжок в портал
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс прыжка в портал
    /// </summary>
    internal class CmdPortalJump : PlanetaryCommand
    {
        /// <summary>
        /// Нельзя отправлять во вражеский портал
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPortalOwner(Ship aShip)
        {
            // На чт можно отправляться всегда
            if (aShip.Planet.Type == PlanetType.Hole)
                return true;
            // На вражеский нельзя
            if (!aShip.Planet.Portal.Owner.IsRoleFriend(aShip.Owner))
                return Warning("Enemy portal");
            else
                return true;
        }

        /// <summary>
        /// Нельзя отправлять если нет портала
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPortal(Ship aShip)
        {
            if (aShip.Planet.Portal == null)
                return Warning("No active portal");
            else
                return false;
        }

        /// <summary>
        /// Отмена отправки корабля по порталу
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckCancellation(Ship aShip)
        {
            if (aShip.TimerEnabled(ShipTimer.PortalJump))
            {
                Engine.Ships.Action.Portaling.Call(aShip, true);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Нельзя отправить чужой кораблик
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlayer">Инициирующий игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Ship aShip, Player aPlayer)
        {
            if (aShip.Owner != aPlayer)
                return Warning("Wrong owner");
            else
                return true;
        }

        /// <summary>
        /// Кораблик должен быть доступен
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckOperable(Ship aShip)
        {
            if (!aShip.IsOperable)
                return Warning("Ship is not operable");
            else
                return true;
        }

        /// <summary>
        /// Существование корабля
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckShip(Ship aShip)
        {
            if (aShip == null)
                return Warning("Invalid ship");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdPortalJump(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Ship tmpShip = ReadShip(aPacket);
            // Основная валидация
            if (!CheckShip(tmpShip))
                return;
            if (!CheckOperable(tmpShip))
                return;
            if (!CheckRole(tmpShip, aPacket.Connection.Player))
                return;
            if (!CheckCancellation(tmpShip))
                return;
            if (!CheckPortal(tmpShip))
                return;
            if (!CheckPortalOwner(tmpShip))
                return;
            // Отправим команду на исполнение
            Engine.Ships.Action.Portaling.Call(tmpShip, false);
        }
    }
}