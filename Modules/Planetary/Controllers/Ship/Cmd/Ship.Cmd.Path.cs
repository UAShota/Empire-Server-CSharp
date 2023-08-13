/////////////////////////////////////////////////
//
// Команда игрока - обработка группы корабликов
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System.Collections.Generic;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс обработки группы корабликов
    /// </summary>
    internal class CmdPath : PlanetaryCommand
    {
        /// <summary>
        /// Запуск группы на полет
        /// </summary>
        /// <param name="aPlanets">Список планет</param>
        /// <param name="aShips">Список кораблей</param>
        /// <param name="aOwner">Иинициирующий игрок</param>
        private void Call(List<Planet> aPlanets, List<Ship> aShips, Player aOwner)
        {
            foreach (Ship tmpShip in aShips)
            {
                if (!CheckRole(tmpShip, aOwner))
                    continue;
                if (!CheckStationary(tmpShip))
                    continue;
                Engine.Ships.Action.Path.Add(tmpShip, aPlanets);
            }
        }

        /// <summary>
        /// Нельзя отправить стационарный
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckStationary(Ship aShip)
        {
            if (aShip.TechActive(ShipTech.Stationary))
                return Warning("Stationary");
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
        /// Проверка наличия кораблей
        /// </summary>
        /// <param name="aShips">Список кораблей</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckShipCount(List<Ship> aShips)
        {
            if (aShips.Count == 0)
                return Warning("Not enough ships");
            else
                return true;
        }

        /// <summary>
        /// Проверка наличия планет
        /// </summary>
        /// <param name="aPlanets">Список кораблей</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPlanetsCount(List<Planet> aPlanets)
        {
            if (aPlanets.Count == 0)
                return Warning("Not enough planets");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdPath(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            int tmpPlanetCount = aPacket.ReadInt();
            int tmpShipCount = aPacket.ReadInt();
            List<Planet> tmpPlanetList = new List<Planet>();
            List<Ship> tmpShipList = new List<Ship>();
            // Сбор планет
            for (int tmpI = 0; tmpI < tmpPlanetCount; tmpI++)
            {
                Planet tmpPlanet = ReadPlanet(aPacket);
                if (tmpPlanet != null)
                    tmpPlanetList.Add(tmpPlanet);
            }
            // Сбор корабликов
            for (int tmpI = 0; tmpI < tmpShipCount; tmpI++)
            {
                Ship tmpShip = ReadShip(aPacket);
                if (tmpShip != null)
                    tmpShipList.Add(tmpShip);
            }
            // Основная валидация
            if (!CheckShipCount(tmpShipList))
                return;
            // Проверим планеты
            if (!CheckPlanetsCount(tmpPlanetList))
                return;
            // Запустим 
            Call(tmpPlanetList, tmpShipList, aPacket.Connection.Player);
        }
    }
}