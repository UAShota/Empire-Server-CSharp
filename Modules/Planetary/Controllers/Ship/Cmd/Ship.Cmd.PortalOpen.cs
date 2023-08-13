/////////////////////////////////////////////////
//
// Команда игрока - открытие портала
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс открытия портала
    /// </summary>
    internal class CmdPortalOpen : PlanetaryCommand
    {
        /// <summary>
        /// Проверка планеты назначения
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPlanet(Planet aPlanet)
        {
            if (aPlanet == null)
                return Warning("Invalid source");
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
        public CmdPortalOpen(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Ship tmpShip = ReadShip(aPacket);
            Planet tmpPlanet = ReadPlanet(aPacket);
            // Основная валидация
            if (!CheckShip(tmpShip))
                return;
            if (!CheckPlanet(tmpPlanet))
                return;
            // Отправим команду на исполнение
            Engine.Planets.Action.Portal.Open(tmpShip, tmpPlanet, aPacket.Connection.Player);
        }
    }
}