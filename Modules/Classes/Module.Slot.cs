/////////////////////////////////////////////////
//
// Модуль описания ячейки слота и ее контейнера
//
// Copyright(c) 2016 UAShota
//
// Rev J  2019.05.15
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Empire.EngineSpace;

namespace Empire.Modules.Classes
{
    /// <summary>
    /// Ячейка слота
    /// </summary>
    internal abstract class Slot : Leak
    {
        /// <summary>
        /// Позиция слота
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Количество кораблей
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Признак блокировки от изменений
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Признак пустого слота
        /// </summary>
        public bool IsEmpty => GetIsEmpty();

        /// <summary>
        /// Признак пустого слота
        /// </summary>
        protected abstract bool GetIsEmpty();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aPosition"></param>
        public Slot(int aPosition)
        {
            Position = aPosition;
        }

        /// <summary>
        /// Обмен позиции слота
        /// </summary>
        /// <param name="aSlot">Слот обмена</param>
        public void Swap(Slot aSlot)
        {
            int tmpPosition = Position;
            Position = aSlot.Position;
            aSlot.Position = tmpPosition;
        }
    }

    /// <summary>
    /// Контейнер слотов
    /// </summary>
    internal abstract class SlotZone<T> : Leak where T : Slot
    {
        /// <summary>
        /// Массив слотов ангара
        /// </summary>
        public List<T> Slots { get; private set; }

        /// <summary>
        /// Максимальный размер контейнера
        /// </summary>
        public int MaxSize => GetMaxSize();

        /// <summary>
        /// Максимальный размер контейнера
        /// </summary>
        /// <returns>Максимальный размер контейнера</returns>
        protected abstract int GetMaxSize();

        /// <summary>
        /// Добавление слота в контейнер
        /// </summary>
        protected abstract T AddSlot();

        /// <summary>
        /// Удаление слота из контейнера
        /// </summary>
        protected abstract void RemoveSlot(T aSlot);

        /// <summary>
        /// Сравнение двух слотов
        /// </summary>
        /// <param name="aLeft">Левый слот</param>
        /// <param name="aRight">Правый слот</param>
        /// <returns>Равнозначность слотов</returns>
        protected abstract bool Compare(T aLeft, T aRight);

        /// <summary>
        /// Объединение двух слотов
        /// </summary>
        /// <param name="aLeft">Левый слот</param>
        /// <param name="aRight">Правый слот</param>
        protected abstract void Merge(T aLeft, T aRight);

        /// <summary>
        /// Попытка получить свободный слот
        /// </summary>
        /// <param name="aSlot">Найденный слот ангара</param>
        /// <param name="aOnCheck">Проверка слота на подходимость</param>
        /// <returns>Существование слота ангара</returns>
        protected bool TryGetSlot(out T aSlot, Func<T, bool> aOnCheck)
        {
            aSlot = null;
            foreach (T tmpSlot in Slots)
            {
                // Если пустой - сохраним первый
                if (tmpSlot.IsEmpty)
                {
                    if (aSlot == null)
                        aSlot = tmpSlot;
                }
                // Если совпадает тип - сразу возвращаем
                else if (aOnCheck(tmpSlot))
                {
                    aSlot = tmpSlot;
                    return true;
                }
            }
            // Если есть пустой слот - скинем туда
            return (aSlot != null);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aSize">Размер контейнера</param>
        public SlotZone()
        {
            Slots = new List<T>();
        }

        /// <summary>
        /// Изменение размеров контейнера
        /// </summary>
        /// <param name="aSize">Новый размер контейнера</param>
        public void Resize(int aSize)
        {
            int tmpSize = Slots.Count;
            int tmpCount = Math.Abs(aSize - tmpSize);
            // Добавим или удалим разницу в слотах
            while (tmpCount-- > 0)
            {
                if (aSize > tmpSize)
                    Slots.Add(AddSlot());
                else
                {
                    int tmpPos = Slots.Count - 1;
                    RemoveSlot(Slots[tmpPos]);
                    Slots.RemoveAt(tmpPos);
                }
            }
        }

        /// <summary>
        /// Перемещение слотов
        /// </summary>
        /// <param name="aSource">Первый слот</param>
        /// <param name="aTarget">Второй слот</param>
        public void Swap(T aSource, T aTarget)
        {
            // Если в слотах одинаковые типы - объединяем
            if (Compare(aSource, aTarget))
                Merge(aSource, aTarget);
            else
            {
                // Обменяем объекты
                Slots[aTarget.Position] = aSource;
                Slots[aSource.Position] = aTarget;
                // Обменяем позиции
                aSource.Swap(aTarget);
            }
        }

        /// <summary>
        /// Попытка получить слот по индексу
        /// </summary>
        /// <param name="aPosition">Позиция слота</param>
        /// <param name="aSlot">Найденный слот</param>
        /// <returns>Существование слота ангара</returns>
        public bool TryGetSlot(int aPosition, out T aSlot)
        {
            // Слот ангара должен входить в рамки
            if ((aPosition >= 0) && (aPosition < Slots.Count))
                aSlot = Slots[aPosition];
            else
                aSlot = null;
            // Успех если слот есть
            return (aSlot != null);
        }
    }
}