/////////////////////////////////////////////////
//
// Обработка подключений через TCP сокет
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Net;
using System.Net.Sockets;
using Empire.EngineSpace;

namespace Empire.Sockets
{
    /// <summary>
    /// TCP сокет
    /// </summary>
    internal class SocketTCP : CustomSocket
    {
        /// <summary>
        /// Слушатель порта
        /// </summary>
        private TcpListener fListener { get; set; }

        /// <summary>
        /// Чтение данных с сокета
        /// </summary>
        /// <param name="aConnection">Соединение</param>
        /// <param name="aBuffer">Буфер данных</param>
        /// <param name="aCount">Размер буфера</param>
        /// <returns>Размер считанного буфера</returns>
        public override int Read(SocketConnection aConnection, ref byte[] aBuffer, int aCount)
        {
            // Попытаемся считать запрошенный буфер
            try
            {
                return ((TcpClient)aConnection.Socket).Client.Receive(aBuffer, aCount, SocketFlags.None);
            }
            // Чтение пакетов прервано штатно
            catch (SocketException E) when (
                E.ErrorCode == 10004 ||
                E.ErrorCode == 10053 ||
                E.ErrorCode == 10054)
            {
                return 0;
            }
            // Обработка остальных ошибок
            catch (Exception E)
            {
                Core.Log.Error(E);
                return 0;
            }
        }

        /// <summary>
        /// Запись данных в сокет
        /// </summary>
        /// <param name="aConnection">Соединение</param>
        /// <param name="aBuffer">Буфер данных</param>
        /// <param name="aCount">Размер буфера</param>
        /// <returns>Размер записанного буфера</returns>
        public override int Write(SocketConnection aConnection, ref byte[] aBuffer, int aCount)
        {
            // Попытаемся отправить запрошенный буфер
            try
            {
                return ((TcpClient)aConnection.Socket).Client.Send(aBuffer, aCount, SocketFlags.None);
            }
            // Чтение пакетов прервано штатно
            catch (SocketException E) when (
                E.ErrorCode == 10004 ||
                E.ErrorCode == 10053 ||
                E.ErrorCode == 10054)
            {
                return 0;
            }
            // Обработка остальных ошибок
            catch (Exception E)
            {
                Core.Log.Error(E);
                return 0;
            }
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        /// <param name="aConnection">Закрываемое соединение</param>
        public override void Close(SocketConnection aConnection)
        {
            ((TcpClient)aConnection.Socket).Client.Close();
        }

        /// <summary>
        /// Каллбак нового подключения
        /// </summary>
        /// <param name="aAsyncResult">Вызывающий листенер</param>
        private void AcceptCallback(IAsyncResult aAsyncResult)
        {
            // При остановке зачем-то вызывается каллбак с уничтоженным листенером
            if (OnAccept == null)
                return;
            // Получение листенера от каллбака
            try
            {
                var tmpListener = (TcpListener)aAsyncResult.AsyncState;
                var tmpClient = tmpListener.EndAcceptTcpClient(aAsyncResult);
                var tmpIP = tmpClient.Client.RemoteEndPoint.ToString();
                // Перейдем к следующему коннекту
                NextAccept();
                // Завершить асинхронный вызов
                OnAccept(new SocketConnection(tmpClient, tmpIP));
            }
            catch (Exception E)
            {
                Core.Log.Error(E);
            }
        }

        /// <summary>
        /// Метод запуска листенера
        /// </summary>
        private void NextAccept()
        {
            try
            {
                fListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), fListener);
            }
            catch (Exception E)
            {
                Core.Log.Error(E);
            }
        }

        /// <summary>
        /// Запуск слушателя ожидания соединений
        /// </summary>
        /// <param name="aPort">Порт слушателя</param>
        /// <param name="aOnAccept">Каллбак подключения нового клиента</param>
        public SocketTCP(int aPort, Action<SocketConnection> aOnAccept)
        {
            fListener = new TcpListener(IPAddress.Any, aPort);
            Core.CheckError("Start listener", () =>
            {
                fListener.Server.NoDelay = true;
                fListener.Start();
            });
            // Начнем асинхронную прослушку
            OnAccept = aOnAccept;
            NextAccept();
        }

        /// <summary>
        /// Остановка слушателя
        /// </summary>
        public override void Dispose()
        {
            OnAccept = null;
            try
            {
                fListener.Stop();
            }
            catch (Exception E)
            {
                Core.Log.Error(E);
            }
            base.Dispose();
        }
    }
}