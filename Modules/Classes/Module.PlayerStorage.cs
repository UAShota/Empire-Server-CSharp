/////////////////////////////////////////////////
//
// Модуль управления хранилищем
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;

namespace Empire.Players.Classes
{
    /// <summary>
    /// Слот хранилища
    /// </summary>
    internal class Holding : Slot
    {
        /// <summary>
        /// Тип ресурса
        /// </summary>
        public ResourceType ResourceType { get; private set; }

        /// <summary>
        /// Определение пустого слота
        /// </summary>
        /// <returns>Признак пустого слота</returns>
        protected override bool GetIsEmpty()
        {
            return (ResourceType == ResourceType.Empty);
        }

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        /// <param name="aPosition">Позиция слота</param>
        public Holding(int aPosition) : base(aPosition)
        {
        }

        /// <summary>
        /// Установка ресурса в слот
        /// </summary>
        /// <param name="aResourceType">Тип корабля</param>
        /// <param name="aCount">Количество корабликов</param>
        public void Change(int aCount, ResourceType aResourceType)
        {
            ResourceType = aResourceType;
            Change(aCount);
        }

        /// <summary>
        /// Установка ресурса в слот
        /// </summary>
        /// <param name="aCount">Количество ресурса</param>
        public void Change(int aCount)
        {
            Count += aCount;
            // Если слот не зафиксирован, обнулим
            if ((Count == 0) && !Locked)
                ResourceType = ResourceType.Empty;
        }
    }

    /// <summary>
    /// Контроллер хранилища
    /// </summary>
    internal class HoldingZone : SlotZone<Holding>
    {
        /// <summary>
        /// Максимальный размер хранилища
        /// </summary>
        private int ciMaxSize => 80;

        /// <summary>
        /// Возвращение максимального размера ангара
        /// </summary>
        /// <returns>Размер максимального ангара</returns>
        protected override int GetMaxSize()
        {
            return ciMaxSize;
        }

        /// <summary>
        /// Добавление слота
        /// </summary>
        /// <returns>Экземпляр слота</returns>
        protected override Holding AddSlot()
        {
            return new Holding(Slots.Count);
        }

        /// <summary>
        /// Удаление слота 
        /// </summary>
        /// <param name="aStorage">Слот</param>
        protected override void RemoveSlot(Holding aStorage)
        {
            aStorage.Dispose();
        }

        /// <summary>
        /// Сравнение слотов
        /// </summary>
        /// <param name="aLeft">Левый слот</param>
        /// <param name="aRight">Правый слот</param>
        /// <returns>Равнозначность типов слотов</returns>
        protected override bool Compare(Holding aLeft, Holding aRight)
        {
            return (aLeft.ResourceType == aRight.ResourceType);
        }

        /// <summary>
        /// Объединение содержимого слотов
        /// </summary>
        /// <param name="aLeft">Левый слот</param>
        /// <param name="aRight">Правый слот</param>
        protected override void Merge(Holding aLeft, Holding aRight)
        {
            /*merge*/
        }

        /// <summary>
        /// Покупка нового слота
        /// </summary>
        public void Buy()
        {
            Resize(Slots.Count + 1);
        }
    }
}