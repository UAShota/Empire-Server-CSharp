/////////////////////////////////////////////////
//
// Команда игрока - подписка на планету
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс подписки на планету
    /// </summary>
    internal class CmdSubscribe : PlanetaryCommand
    {
        /// <summary>
        /// Проверка планеты назначения
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPlanet(Planet aPlanet)
        {
            if (aPlanet == null)
                return Warning("Invalid planet");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdSubscribe(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Planet tmpPlanet = ReadPlanet(aPacket);
            // Основная валидация
            if (!CheckPlanet(tmpPlanet))
                return;
            // Отправим команду на исполнение
            Engine.Planets.Action.Subscribe.Call(tmpPlanet, aPacket.Connection.Player);
        }
    }
}