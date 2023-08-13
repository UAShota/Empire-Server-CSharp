/////////////////////////////////////////////////
//
// Базовый класс для планетарных протоколов
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////
using System;
///
using Empire.Sockets;

namespace Empire.Planetary.Protocol
{
    /// <summary>
    /// Базовый класс для планетарных протоколов
    /// </summary>
    internal class CustomProtocol : IDisposable
    {
        /// <summary>
        /// Планетарный движок
        /// </summary>
        protected PlanetaryEngine Engine { get; private set; }

        /// <summary>
        /// Временный буффер записи всех команд
        /// </summary>
        protected SocketPacket fPacket;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Экземпляр планетарки</param>
        public CustomProtocol(PlanetaryEngine aEngine)
        {
            Engine = aEngine;
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {
        }
    }
}