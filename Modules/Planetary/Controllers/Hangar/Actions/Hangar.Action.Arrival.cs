/////////////////////////////////////////////////
//
// Отправка флота в ангар
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Planetary.Classes;

namespace Empire.Planetary.HangarSpace
{
    /// <summary>
    /// Класс отправки в ангар
    /// </summary>
    internal class ActionArrival : PlanetaryAccess
    {
        /// <summary>
        /// Количество топлива для отправки в ангар
        /// </summary>
        private int ciFuelCost => 2;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionArrival(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Отправка корабля в определенный слот ангара
        /// </summary>
        /// <param name="aShip">Корабль</param>
        /// <param name="aHangar">Ангар</param>
        /// <returns>Разрешение операции</returns>
        public bool Call(Ship aShip, Hangar aHangar)
        {
            // Обновим ангар
            aHangar.Change(aShip.Count, aShip.ShipType);
            Engine.SocketWriter.PlayerHangarUpdate(aHangar, aShip.Owner);
            // Удалим с планеты
            Engine.Ships.Action.Relocation.Delete(aShip);
            // Учета массы пока нет, всегда успешно
            return true;
        }

        /// <summary>
        /// Отправка корабля в неопределенный слот ангара
        /// </summary>
        /// <param name="aShip">Корабль</param>
        /// <returns>Разрешение операции</returns>
        public bool Call(Ship aShip)
        {
            // Поищем свободный слот для корабля
            if (aShip.Owner.Planetary.HangarZone.TryGetSlot(aShip.ShipType, out Hangar tmpHangar))
                return Call(aShip, tmpHangar);
            else
                return false;
        }

        /// <summary>
        /// Возвращение наличия доступного топлива для отправки в ангар
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Наличие топлива для отправки в ангар</returns>
        public bool HasFuel(Ship aShip)
        {
            return aShip.Fuel >= ciFuelCost;
        }
    }
}