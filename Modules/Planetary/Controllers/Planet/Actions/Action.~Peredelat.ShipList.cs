/////////////////////////////////////////////////
//
// Обновление списка наличия кораблей 
// определенного типа на орбите
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2019.06.01
//
/////////////////////////////////////////////////

using System.Collections.Generic;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс работы со спиками кораблей планеты
    /// </summary>
    internal class ShipList : PlanetaryAccess
    {
        /// <summary>
        /// Обновление списка наличия кораблей определенного типа на орбите
        /// </summary>
        /// <param name="aList">Список объектов каждого игрока</param>
        /// <param name="aShip">Оперируемый кораблик</param>
        /// <param name="aCount">Учетное количество</param>
        /// <returns>Итоговое количество корабликов типа оперируемого кораблика</returns>
        private int UpdateList(Dictionary<Player, int> aList, Ship aShip, int aCount)
        {
            // Найдем количество флота для указанного владельца
            bool tmpFound = aList.TryGetValue(aShip.Owner, out int tmpCount);
            // Изменим количество
            tmpCount += aCount;
            // Если кораблик удаляется
            if (aCount < 0)
            {
                // Если улетает последний кораблик владельца, почистить ресурсы
                if (tmpCount == 0)
                    aList.Remove(aShip.Owner);
                else
                    aList[aShip.Owner] = tmpCount;
            }
            else
            {
                // Если прилетает новый кораблик - добавить об этом запись
                if (!tmpFound)
                    aList.Add(aShip.Owner, tmpCount);
                else
                    aList[aShip.Owner] = tmpCount;
            }
            // Вернем
            return tmpCount;
        }

        /// <summary>
        /// Обновление параметров хранилища
        /// </summary>
        /// <param name="aShip">Оперируемый кораблик</param>
        /// <param name="aCount">Учетное количество</param>
        private void UpdateStorages(Ship aShip, int aCount)
        {
            // Для сервиски также обновляем количество складов
            /*TPlanetThread(Engine).ControlStorages.ChangeStorageCount(AShip.Planet, ACount, True);
            // Пересчитаем производ. т.к. сервиска может вырабатывать газ
            TPlanetThread(Engine).WorkerProduction.CalculateProduction(AShip.Planet);*/
        }

        /// <summary>
        /// Обновление парамтеров портала
        /// </summary>
        /// <param name="aShip">Оперируемый кораблик</param>
        /// <param name="aCount">Учетное количество</param>
        private void UpdateShipListPortalers(Ship aShip, int aCount)
        {
            // Если кораблик удаляется - смотрим, есть ли еще такие
            if (aCount > 0)
                return;
            if (aShip.Planet.Portal == null)
                return;
/*            if (!CheckFriendlyPortaler(aShip.Planet, aShip.Owner))
                Controller.Portal.Close(aShip.Planet, aShip.Planet.Portal.Owner);*/
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ShipList(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Обновление списков планеты от типа корабля
        /// </summary>
        /// <param name="aShip">Корабль</param>
        /// <param name="aCount">Количество кораблей для учета</param>
        public void Call(Ship aShip, int aCount) /*проверить отличается ли от количества кораблей*/
        {
            /*if (aShip.TechActive(ShipTech.Fix))
                UpdateList(aShip.Planet.Constructors, aShip, aCount);
            else if (aShip.TechActive(ShipTech.StablePortal))
                UpdateShipListPortalers(aShip, aCount);
            else if (aShip.TechActive(ShipTech.Storage))
                UpdateStorages(aShip, aCount);*/
        }
    }
}