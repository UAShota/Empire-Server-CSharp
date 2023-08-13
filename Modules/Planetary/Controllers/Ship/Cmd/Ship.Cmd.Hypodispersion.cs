/////////////////////////////////////////////////
//
// Команда игрока - усреднение количества кораблей в стеках
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
    /// Класс усреднения количества кораблей в стеках
    /// </summary>
    internal class CmdHypodispersion : PlanetaryCommand
    {
        /// <summary>
        /// Нельзя управлять чужими
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlayer">Игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Ship aShip, Player aPlayer)
        {
            if (!aShip.Owner.IsRoleFriend(aPlayer))
                return Warning("Wrong role");
            else
                return true;
        }

        /// <summary>
        /// Проверка вызывателя
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckShip(Ship aShip)
        {
            if (aShip == null)
                return Warning("Invalid source");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый контроллер</param>
        public CmdHypodispersion(PlanetaryEngine aEngine) : base(aEngine)
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
            if (!CheckRole(tmpShip, aPacket.Connection.Player))
                return;
            // Отправим команду на исполнение
            Engine.Ships.Action.Hypodispersion.Call(tmpShip);
        }
    }
}