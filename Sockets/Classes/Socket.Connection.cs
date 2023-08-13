/////////////////////////////////////////////////
//
// Обработка соединения клиента, буферы чтения и записи
//
// Copyright(c) 2016 UAShota
//
// Rev I  2020.05.01
//
/////////////////////////////////////////////////

using System;
using System.Threading.Tasks;
using Empire.Classes;
using Empire.EngineSpace;
using Empire.Modules.Classes;

namespace Empire.Sockets
{
    /// <summary>
    /// Описание клиентского соединения
    /// </summary>
    internal class SocketConnection : Leak
    {
        /// <summary>
        /// Очередь отправки команд
        /// </summary>
        private ConcurrentQueueSignal<SocketPacket> fQueue { get; set; }

        /// <summary>
        /// Каллбак записи пакета
        /// </summary>
        private Action<SocketPacket> fOnPacket { get; set; }

        /// <summary>
        /// Поток обработки сообщений
        /// </summary>
        private Task fTask { get; }

        /// <summary>
        /// Объект соединения (TCP клиент, WebSocket и т.п.
        /// </summary>
        public object Socket { get; private set; }

        /// <summary>
        /// Адрес клиента
        /// </summary>
        public string IP { get; }

        /// <summary>
        /// Ассоциированный соединению игрок
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Признак сброса соединения
        /// </summary>
        public bool Dropped { get; private set; }

        /// <summary>
        /// Метод выполнения потока отправки сообщений
        /// </summary>
        private void Execute()
        {
            while (fQueue.TryDequeue(out SocketPacket tmpPacket))
                fOnPacket(tmpPacket);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aSocket">Объект сокета</param>
        /// <param name="aIP">Адрес клиента</param>
        public SocketConnection(object aSocket, string aIP) : base()
        {
            Socket = aSocket;
            IP = aIP;
            fQueue = new ConcurrentQueueSignal<SocketPacket>();
            fTask = Core.StartTask(Execute);
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public override void Dispose()
        {
            Drop();
            fQueue.Dispose();
            fTask.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Отправка буфера в очередь
        /// </summary>
        /// <param name="aBuffer">Буфер</param>
        public void Push(SocketPacket aBuffer)
        {
#if DEBUG

            if (Dropped)
                return;
                /*throw new Exception("wrong way dude");*/
#endif
            aBuffer.Connection = this;
            aBuffer.Commit();
            fQueue.Enqueue(aBuffer);
        }

        /// <summary>
        /// Установка каллбака записи пакетов
        /// </summary>        
        /// <param name="aOnPacket">Каллбак</param>
        public void AcceptPacket(Action<SocketPacket> aOnPacket)
        {
            fOnPacket = aOnPacket;
        }

        /// <summary>
        /// Установка владеющего игрока
        /// </summary>        
        /// <param name="aPlayer">Игрок</param>
        public SocketConnection AcceptPlayer(Player aPlayer)
        {
            Player = aPlayer;
            return this;
        }

        /// <summary>
        /// Отмена очереди отправки
        /// </summary>
        public void Drop()
        {
            Dropped = true;
            fQueue.Awake();
            fTask.Wait();
        }
    }
}