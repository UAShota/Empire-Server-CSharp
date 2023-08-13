/////////////////////////////////////////////////
//
// Модуль управления таймерами
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace Empire.Modules.Classes
{
    /// <summary>
    /// Базовый класс таймингового объекта
    /// </summary>
    internal abstract class TimerObject
    {
        /// <summary>
        /// Возвращение количества типов таймера
        /// </summary>
        /// <returns>Количество типов таймеров</returns>
        protected abstract int GetTimersCount();

        /// <summary>
        /// Таймеры объекта
        /// </summary>
        public Timer[] Timers { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TimerObject()
        {
            int tmpCount = GetTimersCount();
            Timers = new Timer[tmpCount];
        }
    }

    /// <summary>
    /// Класс описывающий объект таймера
    /// </summary>
    internal class Timer
    {
        /// <summary>
        /// Таймер
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Значение
        /// </summary>
        public long Time { get; private set; }

        /// <summary>
        /// Слепок времени прохода в момент создания
        /// </summary>
        public long TimeStamp { get; private set; }

        /// <summary>
        /// Признак активности таймера
        /// </summary>
        public bool Active => Object != null;

        /// <summary>
        /// Нода таймера
        /// </summary>
        public LinkedListNode<Timer> Node { get; private set; }

        /// <summary>
        /// Объект
        /// </summary>
        public TimerObject Object { get; private set; }

        /// <summary>
        /// Каллбак срабатывания таймера
        /// </summary>
        public Func<TimerObject, int> OnTimer { get; private set; }

        /// <summary>
        /// Каллбак изменения таймера
        /// </summary>
        public Action<Timer> OnChange { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Timer()
        {
            Node = new LinkedListNode<Timer>(this);
        }

        /// <summary>
        /// Инициализация тайминга
        /// </summary>
        /// <param name="aID">Идентификатор таймера</param>
        /// <param name="aObject">Объект тайминга</param>
        /// <param name="aOnTimer">Каллбак таймера</param>
        /// <param name="aOnChange">Каллбак смены значения</param>
        public void Init(int aID, TimerObject aObject, Func<TimerObject, int> aOnTimer, Action<Timer> aOnChange)
        {
            // Настроим таймер
            ID = aID;
            OnTimer = aOnTimer;
            OnChange = aOnChange;
            Object = aObject;
            Object.Timers[aID] = this;
        }

        /// <summary>
        /// Установка времени срабатывания таймера
        /// </summary>
        /// <param name="aTime">Время тайминга</param>
        /// <param name="aTimeStamp">Время текущего прохода</param>
        public void Restart(int aTime, long aTimeStamp)
        {
            TimeStamp = aTimeStamp;
            // Устанавливаем время только если оно задано явно
            if (aTime > 0)
                Time = DateTime.Now.AddMilliseconds(aTime).Ticks;
            else
                Time = 0;
            // Уведомим о смене значения
            OnChange(this);
        }

        /// <summary>
        /// Сброс параметров таймера
        /// </summary>
        public void Reset()
        {
            Restart(0, 0);
            Object.Timers[ID] = null;
            Object = null;
            OnTimer = null;
            OnChange = null;
            ID = -1;
        }
    }

    /// <summary>
    /// Класс управления таймерами
    /// </summary>
    internal class Timers
    {
        /// <summary>
        /// Слепок времени текущего прохода
        /// </summary>
        private long fTimeStamp { get; set; }

        /// <summary>
        /// Список таймеров
        /// </summary>
        private LinkedList<Timer> fTimers { get; set; }

        /// <summary>
        /// Указатель обхода очереди
        /// </summary>
        private LinkedListNode<Timer> fPointer { get; set; }

        /// <summary>
        /// Количество мсек за тик
        /// </summary>
        public int TimeDelta => 50;

        /// <summary>
        /// Добавление таймера в очередь
        /// </summary>
        /// <param name="aNode">Нода таймера</param>
        /// <param name="aTime">Время таймера</param>
        /// <param name="aTimeStamp">Слепок времени прохода</param>
        private void Add(LinkedListNode<Timer> aNode, int aTime, long aTimeStamp)
        {
            // Запустим таймер
            aNode.Value.Restart(aTime, aTimeStamp);
            // Поищем место добавления
            LinkedListNode<Timer> tmpNode = fTimers.First;
            // Если не нашли, значит он последний
            while (tmpNode != null)
            {
                // Нода пустая или подходит по временной метке
                if (!tmpNode.Value.Active || (tmpNode.Value.Time > aNode.Value.Time))
                    break;
                else
                    tmpNode = tmpNode.Next;
            }
            // Если нода уже существует - удалим
            if (aNode.List != null)
                fTimers.Remove(aNode);
            // И добавим перед найденной нодой
            if (tmpNode == null)
                fTimers.AddLast(aNode);
            else
                fTimers.AddBefore(tmpNode, aNode);
        }

        /// <summary>
        /// Удаление ноды
        /// </summary>
        /// <param name="aNode">Удаляемая нода</param>
        private void Delete(LinkedListNode<Timer> aNode)
        {
            fTimers.Remove(aNode);
            fTimers.AddLast(aNode);
            aNode.Value.Reset();
        }

        /// <summary>
        /// Перезапуск таймера
        /// </summary>
        /// <param name="aNode">Нода таймера</param>
        /// <param name="aTime">Время таймера</param>
        private void Restart(LinkedListNode<Timer> aNode, int aTime)
        {
            // Если меняется текущая нода - передвинем
            if (fPointer == aNode)
                fPointer = aNode.Next;
            // Если время не задано - уберем
            if (aTime <= 0)
                Delete(aNode);
            else
                Add(aNode, aTime, fTimeStamp);
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public Timers()
        {
            fTimers = new LinkedList<Timer>();
        }

        /// <summary>
        /// Добавление нового таймера
        /// </summary>
        /// <param name="aObject">Объект тайминга</param>
        /// <param name="aID">Идентификатор таймера</param>
        /// <param name="aOnTimer">Каллбак срабатывания таймера</param>
        /// <param name="aOnChange">Каллбак изменения таймера</param>
        /// <param name="aTime">Время срабатывания</param>
        public void Add(TimerObject aObject, int aID, Func<TimerObject, int> aOnTimer, Action<Timer> aOnChange, int aTime)
        {
            // Скипнем пустой таймер
            if (aTime <= 0)
                return;
            Timer tmpTimer;
            // Добавим в конец списка или в пустой слот списка
            if ((fTimers.Last == null) || fTimers.Last.Value.Active)
                tmpTimer = new Timer();
            else
                tmpTimer = fTimers.Last.Value;
            // Запустим таймер
            tmpTimer.Init(aID, aObject, aOnTimer, aOnChange);
            // Добавим в список            
            Add(tmpTimer.Node, aTime, fTimeStamp);
        }

        /// <summary>
        /// Удаление конкретного таймера из списка
        /// </summary>
        /// <param name="aTimer">Таймер</param>
        public void Remove(Timer aTimer)
        {
            if (aTimer != null)
                Restart(aTimer.Node, 0);
        }

        /// <summary>
        /// Удаление всех таймеров объекта
        /// </summary>
        /// <param name="aObject">Объект обработки</param>
        public void RemoveAll(TimerObject aObject)
        {
            foreach (Timer tmpTimer in aObject.Timers)
                Remove(tmpTimer);
        }

        /// <summary>
        /// Обработка списка таймеров
        /// </summary>
        public void Work()
        {
            fTimeStamp = DateTime.Now.Ticks;
            fPointer = fTimers.First;
            LinkedListNode<Timer> tmpNode;
            int tmpTime;
            // Перебираем только элементы доступные по времени
            while (fPointer != null)
            {
                // Закончились ноды
                if (!fPointer.Value.Active)
                    break;
                // Текущий проход покрыл все ноды
                if (fPointer.Value.Time > fTimeStamp)
                    break;
                // Текущая нода создана в конктексте обработки
                if (fPointer.Value.TimeStamp == fTimeStamp)
                {
                    fPointer = fPointer.Next;
                    continue;
                }
                // Сохраним текущий указатель
                tmpNode = fPointer;
                // Выполним срабатывание таймера
                tmpTime = tmpNode.Value.OnTimer(tmpNode.Value.Object);
                // Если таймер не активен - значит убит во время срабатывания (аннигиляция и т.п.)
                if (tmpNode.Value.Active)
                    Restart(tmpNode, tmpTime);
            }
            fTimeStamp = 0;
        }
    }
}