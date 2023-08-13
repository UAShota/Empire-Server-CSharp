/////////////////////////////////////////////////
//
// Усреднение количества кораблей в стеках
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс усреднения количества кораблей в стеках
    /// </summary>
    internal class ActionHypodispersion : PlanetaryAccess
    {
        /// <summary>
        /// Список активных непривязанных стеков
        /// </summary>
        private List<Ship> fListActive { get; set; }

        /// <summary>
        /// Список активных привязанных стеков
        /// </summary>
        private List<Ship> fListAttached { get; set; }

        /// <summary>
        /// Список пассивных стеков
        /// </summary>
        private List<Ship> fListPassive { get; set; }

        /// <summary>
        /// Список дублирующих типов
        /// </summary>
        private List<Ship> fListDuplicates { get; set; }

        /// <summary>
        /// Количество активных непривязанных кораблей
        /// </summary>
        private int fActiveCount { get; set; }

        /// <summary>
        /// Количество активных привязанных кораблей
        /// </summary>
        private int fAttachedCount { get; set; }

        /// <summary>
        /// Количество пассивных кораблей
        /// </summary>
        private int fPassiveCount { get; set; }

        /// <summary>
        /// Пополнение активных стеков пассивными
        /// </summary>
        /// <param name="aTechCount">Потолок количества кораблей в стеке</param>
        /// <returns>Был обработан хотя бы один стек</returns>
        private bool MergePassive(int aTechCount)
        {
            if (fListPassive.Count == 0)
                return false;
            // Сохраним сумму 
            int tmpPassiveMerged = 0;
            int tmpSize;
            // Переберем все активные для пополнения структуры с пассивных
            foreach (Ship tmpShip in fListActive)
            {
                // Стек имеет максимум по технологии
                if (tmpShip.Count >= aTechCount)
                    continue;
                // Пассивная структура закончилась
                if (fPassiveCount == 0)
                    break;
                // Докинем текущий стек до максимума
                tmpSize = Math.Min(fPassiveCount, aTechCount - tmpShip.Count);
                tmpShip.Count += tmpSize;
                tmpShip.IsChanged = true;
                fActiveCount += tmpSize;
                fPassiveCount -= tmpSize;
                tmpPassiveMerged += tmpSize;
            }
            // Удалим пустые пассивные, сняв с них затраченную структуру
            foreach (Ship tmpShip in fListPassive)
            {
                if (tmpPassiveMerged == 0)
                    break;
                tmpSize = Math.Min(tmpShip.Count, tmpPassiveMerged);
                tmpShip.Count -= tmpSize;
                tmpShip.IsChanged = true;
                tmpPassiveMerged -= tmpSize;
            }
            // Вернем что параметры кораблей изменились
            return true;
        }

        /// <summary>
        /// Усреднение активных стеков
        /// </summary>
        /// <param name="aOnlyAttached">Все или только привязанные</param>
        /// <returns>Был обработан хотя бы один стек</returns>
        private bool MergeActive(bool aOnlyAttached)
        {
            List<Ship> tmpList;
            int tmpShipCount;
            // Для уравнения приаттаченных нужно иметь минимум 2 корабля
            if (aOnlyAttached)
            {
                if (fListAttached.Count < 2)
                    return false;
                tmpList = fListAttached;
                tmpShipCount = fAttachedCount;
            }
            else
            {
                if (fListActive.Count < 2)
                    return false;
                tmpList = fListActive;
                tmpShipCount = fActiveCount;
            }
            // Найдем среднее количество
            int tmpSize = tmpShipCount / tmpList.Count;
            Ship tmpLastShip = null;
            // И распределим активные кораблики поровну
            foreach (Ship tmpShip in tmpList)
            {
                tmpShip.Count = tmpSize;
                tmpShip.IsChanged = true;
                tmpShipCount -= tmpSize;
                tmpLastShip = tmpShip;
            }
            // А излишки отправим в последнюю обработанную пачку
            tmpLastShip.Count += tmpShipCount;
            // Вернем что параметры кораблей изменились
            return true;
        }

        /// <summary>
        /// Уравнивание активных или привязанных корабликов
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aAuto">Авто или ручное усреднение</param>
        private void CollectShips(Ship aShip, bool aAuto)
        {
            // Очистим списки и получим техи
            Clear();
            // Переберем все корабли на наличие подходящих
            foreach (Ship tmpShip in aShip.Planet.Ships)
            {
                if ((tmpShip.Owner != aShip.Owner)
                    || (tmpShip.ShipType != aShip.ShipType)
                    || (tmpShip.Count == 0)
                    || (tmpShip.State != ShipState.Available)
                    || (tmpShip.Landing.IsLowOrbit))
                    continue;
                // Активные приаттаченные пополняют друг друга
                if (tmpShip.Mode == ShipMode.Active)
                {
                    fListActive.Add(tmpShip);
                    fActiveCount += tmpShip.Count;
                }
                // Если авто - учтем пассивные
                if (!aAuto || (tmpShip.Attach == null))
                    continue;
                // Неактивные приаттаченные пополняют все активные стеки
                if (tmpShip.Mode != ShipMode.Active)
                {
                    fListPassive.Add(tmpShip);
                    fPassiveCount += tmpShip.Count;
                }
                else
                {
                    fListAttached.Add(tmpShip);
                    fAttachedCount += tmpShip.Count;
                }
            }
            // Обработаем полученные списки
            bool tmpHasChanges;
            if (aAuto)
                tmpHasChanges = MergePassive(aShip.TechValue(ShipTech.Count)) || MergeActive(true);
            else
                tmpHasChanges = MergeActive(false);
            // И отправим изменения, если планета не в бою или это принудительный баланс
            if (tmpHasChanges && (!aAuto || !aShip.Planet.TimerEnabled(PlanetTimer.Battle)))
                Engine.Ships.Action.Utils.WorkShipHP(aShip.Planet);
        }

        /// <summary>
        /// Очистка временных буферов
        /// </summary>
        private void Clear()
        {
            fListActive.Clear();
            fListAttached.Clear();
            fListPassive.Clear();
            fListDuplicates.Clear();
            fActiveCount = 0;
            fAttachedCount = 0;
            fPassiveCount = 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionHypodispersion(PlanetaryEngine aEngine) : base(aEngine)
        {
            fListActive = new List<Ship>();
            fListAttached = new List<Ship>();
            fListPassive = new List<Ship>();
            fListDuplicates = new List<Ship>();
        }

        /// <summary>
        /// Уравнивание всех привязанных корабликов на планете
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        public void Call(Planet aPlanet)
        {
            bool tmpFound;
            // Обработаем все стеки планеты
            for (int tmpI = aPlanet.Ships.Count - 1; tmpI > 0; tmpI--)
            {
                Ship tmpShip = aPlanet.Ships[tmpI];
                // Каждый тип корабля обрабатывается только один раз
                if (tmpShip.Attach != aPlanet)
                    continue;
                else
                    tmpFound = false;
                // Поищем уже имеющийся стек владельца с указанным типов корабля
                foreach (Ship tmpDuplicate in fListDuplicates)
                {
                    if ((tmpDuplicate.Owner == tmpShip.Owner)
                        && (tmpDuplicate.ShipType == tmpShip.ShipType))
                    {
                        tmpFound = true;
                        break;
                    }
                }
                // Добавим кораблик к списку использованных
                if (!tmpFound)
                {
                    fListDuplicates.Add(tmpShip);
                    CollectShips(tmpShip, true);
                }
            }
        }

        /// <summary>
        /// Уравнивание всех кораблей указанного типа
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        public void Call(Ship aShip)
        {
            CollectShips(aShip, false);
        }
    }
}