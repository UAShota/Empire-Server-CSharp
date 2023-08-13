/////////////////////////////////////////////////
//
// Модуль управления планетарным ангаром
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
    internal class Hangar : Slot
    {
        /// <summary>
        /// Тип корабля
        /// </summary>
        public ShipType ShipType { get; private set; }

        /// <summary>
        /// Определение пустого слота
        /// </summary>
        /// <returns>Признак пустого слота</returns>
        protected override bool GetIsEmpty()
        {
            return (ShipType == ShipType.Empty);
        }

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        /// <param name="aPosition">Позиция слота</param>
        public Hangar(int aPosition) : base(aPosition)
        {
        }

        /// <summary>
        /// Установка корабля в слот
        /// </summary>
        /// <param name="aCount">Количество корабликов</param>
        /// <param name="aShipType">Тип корабля</param>
        public void Change(int aCount, ShipType aShipType)
        {
            ShipType = aShipType;
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
                ShipType = ShipType.Empty;
        }
    }

    /// <summary>
    /// Контроллер ангара
    /// </summary>
    internal class HangarZone : SlotZone<Hangar>
    {
        /// <summary>
        /// Максимальный размер ангара
        /// </summary>
        private int ciMaxSize => 7;

        /// <summary>
        /// Возвращение максимального размера ангара
        /// </summary>
        /// <returns>Размер максимального ангара</returns>
        protected override int GetMaxSize()
        {
            return ciMaxSize;
        }

        /// <summary>
        /// Добавление слота ангара
        /// </summary>
        /// <returns>Созданный ангар</returns>
        protected override Hangar AddSlot()
        {
            return new Hangar(Slots.Count);
        }

        /// <summary>
        /// Удаление слота ангара
        /// </summary>
        /// <param name="aHangar">Экземпляр ангара</param>
        protected override void RemoveSlot(Hangar aHangar)
        {
            aHangar.Dispose();
        }

        /// <summary>
        /// Сравнение слотов
        /// </summary>
        /// <param name="aLeft">Левый слот</param>
        /// <param name="aRight">Правый слот</param>
        /// <returns>Равнозначность типов слотов</returns>
        protected override bool Compare(Hangar aLeft, Hangar aRight)
        {
            return (aLeft.ShipType == aRight.ShipType);
        }

        /// <summary>
        /// Объединение содержимого слотов
        /// </summary>
        /// <param name="aSource">Левый слот</param>
        /// <param name="aTarget">Правый слот</param>
        protected override void Merge(Hangar aSource, Hangar aTarget)
        {
            aTarget.Change(aSource.Count);
            aSource.Change(-aSource.Count);
        }

        /// <summary>
        /// Попытка получить свободный слот
        /// </summary>
        /// <param name="aSlot">Найденный слот ангара</param>
        /// <param name="aOnCheck">Проверка слота на подходимость</param>
        /// <returns>Существование слота ангара</returns>
        public bool TryGetSlot(ShipType aShipType, out Hangar aHangar)
        {
            return TryGetSlot(out aHangar, (Hangar aSlot) => { return aSlot.ShipType == aShipType; });
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