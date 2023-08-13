/////////////////////////////////////////////////
//
// Команда игрока - cмена активности кораблика
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
    /// Класс смены состояния кораблика
    /// </summary>
    internal class CmdActivity : PlanetaryCommand
    {
        /// <summary>
        /// Смена состояния кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        private void Call(Ship aShip)
        {
            // Уход в походку без штрафа
            if (aShip.Mode != ShipMode.Offline)
            {
                Engine.Ships.Action.StandDown.Call(aShip, ShipMode.Offline);
                return;
            }
            // Без боя поднимаемся сразу, иначе штраф
            if (!aShip.Planet.TimerEnabled(PlanetTimer.Battle))
                Engine.Ships.Action.StandUp.Call(aShip);
            else
            {
                aShip.Mode = ShipMode.Active;
                Engine.Ships.Action.Fly.Call(aShip, ShipFlyType.Parking);
            }
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
        public CmdActivity(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Ship tmpShip = ReadShip(aPacket);
            // Проверки
            if (!CheckShip(tmpShip))
                return;
            if (!CheckOperable(tmpShip))
                return;
            if (!CheckRole(aPacket.Connection.Player, tmpShip))
                return;
            // Выполним смену режима
            Call(tmpShip);
        }
    }
}