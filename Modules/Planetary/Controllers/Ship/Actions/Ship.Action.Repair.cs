/////////////////////////////////////////////////
//
// Саморемонт стека
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс саморемонта стека
    /// </summary>
    internal class ActionRepair : PlanetaryAccess
    {
        /// <summary>
        /// Интервал починки
        /// </summary>
        private int ciTimerInterval => 1000;

        /// <summary>
        /// Скидка постройки кораблика с убитого
        /// </summary>
        private float ciDiscount => 0.70f;

        /// <summary>
        /// Подсчет цены ремонта
        /// </summary>
        /// <param name="aShip">Ремонтируемый кораблик</param>
        /// <returns>Цена ремонта в модулях</returns>
        private int RestoreCost(Ship aShip)
        {
            return (int)Math.Round(aShip.TechValue(ShipTech.Cost) * ciDiscount);
        }

        /// <summary>
        /// Определение возможности использовать починку
        /// </summary>
        /// <param name="aShip">Инициирующий кораблик</param>
        /// <returns>Разрешение на починку</returns>
        private bool Available(Ship aShip)
        {
            return (aShip.HP < aShip.TechValue(ShipTech.Hp))
                || (aShip.Destructed > 0);  /*&& (aShip.Planet.ResAvailIn[resModules] >= RepairRestoreCost(aShip))*/
        }

        /// <summary>
        /// Таймер ремонта
        /// </summary>
        /// <param name="aShip">Ремонтируемый кораблик</param>
        private int OnTimer(TimerObject aShip)
        {
            Ship tmpShip = (Ship)aShip;
            int tmpRepairCount = tmpShip.TechValue(ShipTech.Repair);
            bool tmpInBattle = tmpShip.Planet.TimerEnabled(PlanetTimer.Battle);
            // Стационарки не в бою в два раза быстрее
            if (tmpShip.TechActive(ShipTech.Stationary) && (!tmpInBattle))
                tmpRepairCount *= 2;
            // Восстановим указанное количество ХП
            Call(tmpShip, tmpRepairCount);
            // Уведомим
            if (!tmpInBattle)
            {
                tmpShip.IsChanged = false;
                Engine.SocketWriter.ShipUpdateHP(tmpShip);
            }
            // Вернем прокидку таймера дальше
            return Available(tmpShip) ? ciTimerInterval : 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionRepair(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Починка корабля на указанное значение структуры
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aMount">Количество структуры</param>
        /// <returns>Количество использованной структуры</returns>
        public int Call(Ship aShip, int aMount)
        {
            int tmpRepaired = 0;
            int tmpMaxHP = aShip.TechValue(ShipTech.Hp);
            // Проверим можно ли восстановить стек
            if (aShip.Destructed > 0)
            {
                int tmpCost = RestoreCost(aShip);
                int tmpAvailable = Math.Min(aShip.Destructed, Math.Min(aMount / tmpMaxHP, /*aShip.Planet.ResAvailIn[resModules]*/ 1 / tmpCost));
                if (tmpAvailable > 0)
                {
                    aShip.Count += tmpAvailable;
                    aShip.Destructed -= tmpAvailable;
                    aShip.IsChanged = true;
                    tmpRepaired += tmpAvailable * tmpMaxHP;
                    /*TPlanetThread(Engine).ControlStorages.DecrementResource(resModules, AShip.Planet, tmpAvailable * tmpCost);*/
                }
            }
            // Увеличим ХП
            if (aShip.HP < tmpMaxHP)
            {
                int tmpCount = Math.Min(aMount, aShip.TechValue(ShipTech.Hp) - aShip.HP);
                tmpRepaired += tmpCount;
                aShip.HP += tmpCount;
                aShip.IsChanged = true;
            }
            return tmpRepaired;
        }

        /// <summary>
        /// Проверка на возможность запустить таймер самопочинки кораблика
        /// </summary>
        /// <param name="aShip">Проверяемый кораблик</param>
        public void Check(Ship aShip)
        {
            // Проверим наличие уже работающего таймера
            if (aShip.TimerEnabled(ShipTimer.Repair))
                return;
            // Проверим наличие технологии саморемонта
            if (!aShip.TechActive(ShipTech.Repair))
                return;
            // Проверим возможность восстановления
            if (Available(aShip))
                Engine.Ships.Action.Utils.TimerAdd(aShip, ShipTimer.Repair, OnTimer, ciTimerInterval);
        }
    }
}