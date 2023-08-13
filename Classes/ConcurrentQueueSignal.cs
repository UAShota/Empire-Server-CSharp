/////////////////////////////////////////////////
//
// Класс потокобезопасной очереди с ожиданием события
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Collections.Concurrent;
using System.Threading;
using Empire.EngineSpace;

namespace Empire.Classes
{
    /// <summary>
    /// Класс потокобезопасной очереди с ожиданием события
    /// </summary>
    internal class ConcurrentQueueSignal<T> : ConcurrentQueue<T>, IDisposable
    {
        /// <summary>
        /// Семафор ожидания
        /// </summary>
        private SemaphoreSlim fSemaphore { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ConcurrentQueueSignal()
        {
            Leaks.Enter(this);
            fSemaphore = new SemaphoreSlim(0);
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {
            fSemaphore.Dispose();
            Leaks.Leave(this);
        }

        /// <summary>
        /// Добавление в очередь
        /// </summary>
        /// <param name="aContainer">Значение</param>
        public new void Enqueue(T aContainer)
        {
            base.Enqueue(aContainer);
            fSemaphore.Release();
        }

        /// <summary>
        /// Переопределенный запрос объекта с учетом ожидания события
        /// </summary>
        /// <param name="aContainer">Значение</param>
        /// <returns>Наличие значения в очереди</returns>
        public new bool TryDequeue(out T aContainer)
        {
            fSemaphore.Wait();
            return base.TryDequeue(out aContainer);
        }

        /// <summary>
        /// Пробуждение очереди
        /// </summary>
        public void Awake()
        {
            fSemaphore.Release();
        }
    }
}