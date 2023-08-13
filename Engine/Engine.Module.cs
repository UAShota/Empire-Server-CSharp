/////////////////////////////////////////////////
//
// Базовый класс обработки команд модулями
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System;
using System.Threading.Tasks;
using Empire.Classes;
using Empire.EngineSpace;
using Empire.Sockets;

namespace Empire.Modules
{
    /// <summary>
    /// Обработка модулями команд первого уровня
    /// </summary>
    internal abstract class CustomModule : IDisposable
    {
        /// <summary>
        /// Объект задачи потока
        /// </summary>
        private Task fTask { get; }

        /// <summary>
        /// Очередь с ожиданием события
        /// </summary>
        protected ConcurrentQueueSignal<SocketPacket> fQueue { get; private set; }

        /// <summary>
        /// Выполнение потока обработки команд
        /// </summary>
        private void ExecuteThread()
        {
            while (fQueue.TryDequeue(out SocketPacket tmpBuffer))
            {
                if (!DoExecute(tmpBuffer))
                    tmpBuffer.Dispose();
            }
        }

        /// <summary>
        /// Запуск модуля
        /// </summary>
        protected virtual void DoStart()
        {
        }

        /// <summary>
        /// Остановка модуля
        /// </summary>
        protected virtual void DoStop()
        {
        }

        /// <summary>
        /// Обработчик потока
        /// </summary>
        /// <param name="aBuffer">Буфер команды</param>
        protected abstract bool DoExecute(SocketPacket aBuffer);

        /// <summary>
        /// Конструктор
        /// </summary>
        public CustomModule()
        {
            // Сперва запустим чтобы был готов к обработке
            DoStart();
            // Затем запустим поток
            fQueue = new ConcurrentQueueSignal<SocketPacket>();
            fTask = Core.StartTask(ExecuteThread);
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {
            // Сперва остановим поток
            fQueue.Awake();
            fTask.Wait();
            fQueue.Dispose();
            fTask.Dispose();
            // Затем остановим сам модуль
            DoStop();
        }

        /// <summary>
        /// Обработка команды
        /// </summary>
        /// <param name="aBuffer">Буфер данных</param>
        public void Command(SocketPacket aBuffer)
        {
            fQueue.Enqueue(aBuffer);
        }
    }
}