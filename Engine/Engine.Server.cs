/////////////////////////////////////////////////
//
// Сервер управления модулями 
//
// Copyright(c) 2016 UAShota
//
// Rev I  2020.05.01
//
/////////////////////////////////////////////////

using System;
using Empire.Modules;
using Empire.Modules.Classes;
using Empire.Sockets;

namespace Empire.EngineSpace
{
    /// <summary>
    /// Сервер управления модулями
    /// </summary>
    internal class EngineServer : IDisposable
    {
        /// <summary>
        /// Размер буфера чтения
        /// </summary>
        private int ciBufferSize => 1024;

        /// <summary>
        /// Серверный скокет
        /// </summary>
        private CustomSocket fSocket { get; set; }

        /// <summary>
        /// Модуль планетарок
        /// </summary>
        public PlanetaryModule Planetary { get; private set; }

        /// <summary>
        /// Модуль галактики
        /// </summary>
        public GalaxyModule Galaxy { get; private set; }

        /// <summary>
        /// Модуль авторизации
        /// </summary>
        public AuthorizationModule Auth { get; private set; }

        /// <summary>
        /// Чтения буфера пакета
        /// </summary>
        /// <param name="aPacket">Пакет</param>
        /// <param name="aBuffer">Буфер</param>
        /// <param name="aCount">Количество читаемых байт</param>
        /// <returns>Успешность чтения</returns>
        private bool ReadPacket(SocketPacket aPacket, ref byte[] aBuffer, int aCount)
        {
            // Проверим размер буфера запроса, мин 1 макс 10кб
            if ((aCount <= 0) || (aCount >= ciBufferSize * 10))
                return false;
            // Попробуем считать запрашиваемый буфер
            while (aCount > 0)
            {
                // Определим размер буфера чтения
                int tmpCount = Math.Min(ciBufferSize, aCount);
                // Попробуем считать
                tmpCount = fSocket.Read(aPacket.Connection, ref aBuffer, tmpCount);
                if (tmpCount > 0)
                    aCount -= tmpCount;
                else
                    return false;
                // Запишем в буфер
                aPacket.WriteBuffer(ref aBuffer, 0, tmpCount);
            }
            return true;
        }

        /// <summary>
        /// Запись пкакета в сокет
        /// </summary>
        /// <param name="aPacket">Пакет</param>
        private void WritePacket(SocketPacket aPacket)
        {
            byte[] tmpBytes = aPacket.Buffer;
            fSocket.Write(aPacket.Connection, ref tmpBytes, aPacket.Length);
            aPacket.Dispose();
        }

        /// <summary>
        /// Каллбак нового соединения
        /// </summary>
        /// <param name="aConnection">Экземпляр соединения</param>
        private void OnAccept(SocketConnection aConnection)
        {
            // Добавим обработчик каллбака
            aConnection.AcceptPacket(WritePacket);
            // Определим буфер чтения сокета
            byte[] tmpBuffer = new byte[ciBufferSize];
            SocketPacket tmpPacket;
            // Запустим цикл чтения
            while (true)
            {
                // Для каждой команды новый пакет, он потом уходит в очередь нужного модуля
                tmpPacket = new SocketPacket(aConnection);
                // Считаем размер пакета
                if (!ReadPacket(tmpPacket, ref tmpBuffer, sizeof(int)))
                    break;
                // Считаем тело пакета
                if (!ReadPacket(tmpPacket, ref tmpBuffer, tmpPacket.Reset(ref tmpBuffer)))
                    break;
                // Попробуем считать команду с буфера        
                int tmpCommand = tmpPacket.ReadCommand();
                // Иначе добавим в обработчики 0x1F01 >> 0x1
                switch (tmpCommand >> 12)
                {
                    case 0:
                        Auth.Command(tmpPacket);
                        break;
                    case 1:
                        Planetary.Command(tmpPacket);
                        break;
                    case 2:
                        Galaxy.Command(tmpPacket);
                        break;
                    default:
                        Core.Log.Error("Unknow command 0x{0:X}", tmpCommand);
                        break;
                }
            }
            // Убьем текущий пакет
            tmpPacket.Dispose();
            // Если подключение небыло сброшено вручную, уберем у игрока
            /*  if (!aConnection.Dropped)
                  aConnection.Player.Decline();
              // Убьем подключение
              aConnection.Dispose();*/
        }

        /// <summary>
        /// Каллбак дисконнекта
        /// </summary>
        /// <param name="aConnection">Экземпляр соединения</param>
        private void OnDisconnect(SocketConnection aConnection)
        {

        }

        /// <summary>
        /// Запуск сервера
        /// </summary>
        /// <param name="aPort">Порт сервера</param>
        public EngineServer(int aPort)
        {
            Core.StartAction(nameof(PlanetaryModule), () => Planetary = new PlanetaryModule());
            Core.StartAction(nameof(AuthorizationModule), () => Auth = new AuthorizationModule());
            Core.StartAction(nameof(SocketTCP), () => fSocket = new SocketTCP(aPort, OnAccept));
        }

        /// <summary>
        /// Остановка сервера
        /// </summary>
        public void Dispose()
        {
            Core.StartAction(nameof(SocketTCP), () => fSocket.Dispose());
            Core.StartAction(nameof(AuthorizationModule), () => Auth.Dispose());
            Core.StartAction(nameof(PlanetaryModule), () => Planetary.Dispose());
        }

        /// <summary>
        /// Отключение клиента
        /// </summary>
        /// <param name="aPlayer">Клиент</param>
        public void Kick(Player aPlayer)
        {
            aPlayer.Connection.Drop();
            fSocket.Close(aPlayer.Connection);
        }
    }
}