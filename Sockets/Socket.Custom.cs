/////////////////////////////////////////////////
//
// Базовый класс сокета
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.EngineSpace;

namespace Empire.Sockets
{
    /// <summary>
    /// Базовый класс сокета
    /// </summary>
    internal abstract class CustomSocket : Leak
    {
        /// <summary>
        /// Каллбак подключения клиента
        /// </summary>
        protected Action<SocketConnection> OnAccept { get; set; }

        /// <summary>
        /// Каллбак отключения клиента
        /// </summary>
        protected Action<SocketConnection> OnDisconnect { get; set; }

        /// <summary>
        /// Чтение данных с сокета
        /// </summary>
        /// <param name="aConnection">Соединение</param>
        /// <param name="aBuffer">Буфер данных</param>
        /// <param name="aCount">Размер буфера</param>
        /// <returns>Размер считанного буфера</returns>
        public abstract int Read(SocketConnection aConnection, ref byte[] aBuffer, int aCount);

        /// <summary>
        /// Запись данных в сокет
        /// </summary>
        /// <param name="aConnection">Соединение</param>
        /// <param name="aBuffer">Буфер данных</param>
        /// <param name="aCount">Размер буфера</param>
        /// <returns>Размер записанного буфера</returns>
        public abstract int Write(SocketConnection aConnection, ref byte[] aBuffer, int aCount);

        /// <summary>
        /// Отключение клиента
        /// </summary>
        /// <param name="aConnection">Соединение</param>
        public abstract void Close(SocketConnection aSocket);
    }
}