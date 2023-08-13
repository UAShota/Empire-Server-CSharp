/////////////////////////////////////////////////
//
// Команда игрока - уничтожение стека
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
    /// Класс самоуничтожения корабля
    /// </summary>
    internal class CmdDestroy : PlanetaryCommand
    {
        /// <summary>
        /// Нельзя уничтожать если идет бой
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckBattle(Ship aShip)
        {
            if (aShip.Planet.TimerEnabled(PlanetTimer.Battle))
                return Warning("In battle");
            else
                return true;
        }

        /// <summary>
        /// Проверка состояния кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckOperable(Ship aShip)
        {
            if (!aShip.IsOperable)
                return Warning("Not operable");
            else
                return true;
        }

        /// <summary>
        /// Проверка принадлежности кораблика
        /// </summary>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Player aPlayer, Ship aShip)
        {
            if (aShip.Owner != aPlayer)
                return Warning("Wrong owner");
            else
                return true;
        }

        /// <summary>
        /// Проверка корабля источника
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
        public CmdDestroy(PlanetaryEngine aEngine) : base(aEngine)
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
            if (!CheckBattle(tmpShip))
                return;
            if (!CheckRole(aPacket.Connection.Player, tmpShip))
                return;
            // Отправим команду на исполнение
            Engine.Ships.Action.Relocation.Delete(tmpShip, ShipDestroyMode.Explose);
        }
    }
}