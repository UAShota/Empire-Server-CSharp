/////////////////////////////////////////////////
//
// Модуль управления хранилищем планетоида
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;

namespace Empire.Planetary.Classes
{
    /// <summary>
    /// Слот ангара
    /// </summary>
    internal class Storage : Slot
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
        public Storage(int aPosition) : base(aPosition)
        {
        }

        /// <summary>
        /// Установка корабля в слот
        /// </summary>
        /// <param name="aResourceType">Тип корабля</param>
        /// <param name="aCount">Количество корабликов</param>
        public void Change(int aCount, ResourceType aResourceType)
        {
            ResourceType = aResourceType;
            Change(aCount);
        }

        /// <summary>
        /// Установка корабля в слот
        /// </summary>
        /// <param name="aCount">Количество корабликов</param>
        public void Change(int aCount)
        {
            Count += aCount;
            // Если слот не зафиксирован, обнулим
            if ((Count == 0) && !Locked)
                ResourceType = ResourceType.Empty;
        }
    }

    /// <summary>
    /// Контроллер ангара
    /// </summary>
    internal class StorageZone : SlotZone<Storage>
    {
        /// <summary>
        /// Максимальный размер хранилища
        /// </summary>
        private int ciMaxSize => 20;

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
        protected override Storage AddSlot()
        {
            return new Storage(Slots.Count);
        }

        /// <summary>
        /// Удаление слота 
        /// </summary>
        /// <param name="aStorage">Слот</param>
        protected override void RemoveSlot(Storage aStorage)
        {
            aStorage.Dispose();
        }

        /// <summary>
        /// Сравнение слотов
        /// </summary>
        /// <param name="aLeft">Левый слот</param>
        /// <param name="aRight">Правый слот</param>
        /// <returns>Равнозначность типов слотов</returns>
        protected override bool Compare(Storage aLeft, Storage aRight)
        {
            return (aLeft.ResourceType == aRight.ResourceType);
        }

        /// <summary>
        /// Объединение содержимого слотов
        /// </summary>
        /// <param name="aLeft">Левый слот</param>
        /// <param name="aRight">Правый слот</param>
        protected override void Merge(Storage aLeft, Storage aRight)
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