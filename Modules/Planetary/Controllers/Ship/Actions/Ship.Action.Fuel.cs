/////////////////////////////////////////////////
//
// Обработка топлива флота
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
    /// Класс обработки топлива флота
    /// </summary>
    internal class ActionFuel : PlanetaryAccess
    {
        /// <summary>
        /// Время автопополнения топлива
        /// </summary>
        private int ciTimerRefuel => 30000;

        /// <summary>
        /// Количества топлива в баках кораблика
        /// </summary>
        private int ciMaxFuelCount => 5;

        /// <summary>
        /// Возвращение времени обработки для конкретного юнита
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Врем отката</returns>
        private int GetCooldown(Ship aShip)
        {
            return ciTimerRefuel;
        }

        /// <summary>
        /// Возвращение необходимости продолжить заправку юнита
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Признак необходимости дальнейшей заправки юнита</returns>
        private bool GetNeedRefuel(Ship aShip)
        {
            return aShip.Fuel < Capacity(aShip);
        }

        /// <summary>
        /// Изменение значения заправки
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aAmount">Количество</param>
        /// <returns>Признак необходимости продлить таймер</returns>
        private bool Refuel(Ship aShip, int aAmount)
        {
            aShip.Fuel += aAmount;
            // Уведомим
            Engine.SocketWriter.ShipUpdateState(aShip);
            // Вернем необходимость продолжения таймера
            return GetNeedRefuel(aShip);
        }

        /// <summary>
        /// Добавление топлива
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aAmount">Количество топлива</param>
        private void Change(Ship aShip, int aAmount)
        {
            bool tmpTimered = aShip.TimerEnabled(ShipTimer.Refuel);
            // Обработаем необходимость дальнейшей заправки
            if (Refuel(aShip, aAmount))
            {
                if (!tmpTimered)
                    Engine.Ships.Action.Utils.TimerAdd(aShip, ShipTimer.Refuel, OnTimer, GetCooldown(aShip));
            }
            else
            {
                if (tmpTimered)
                    Engine.Ships.Action.Utils.TimerRemove(aShip, ShipTimer.Refuel);
            }
        }

        /// <summary>
        /// Каллбак таймера заправки
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Время продления таймера</returns>
        private int OnTimer(TimerObject aShip)
        {
            Ship tmpShip = (Ship)aShip;
            // Заправим
            return Refuel(tmpShip, 1) ? GetCooldown(tmpShip) : 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionFuel(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Добавление топлива кораблю
        /// </summary>
        /// <param name="aShip">Корабль</param>
        /// <param name="aAmount">Количество топлива</param>
        /// <returns>Количество добавленного топлива</returns>
        public int Add(Ship aShip, int aAmount)
        {
            // Определим сколько сможем добавить
            aAmount = Math.Min(aAmount, Capacity(aShip) - aShip.Fuel);
            // Уберем и стартанем таймер
            Change(aShip, aAmount);
            // Вернем количество добавленного топлива
            return aAmount;
        }

        /// <summary>
        /// Удаление топлива корабля
        /// </summary>
        /// <param name="aShip">Корабль</param>
        /// <param name="aAmount">Количество топлива</param>
        /// <returns>Количество удаленного топлива</returns>
        public int Remove(Ship aShip, int aAmount)
        {
            // Определим сколько сможем убрать
            aAmount = Math.Min(aAmount, aShip.Fuel);
            // Уберем и стартанем таймер
            Change(aShip, -aAmount);
            // Вернем количество убранного топлива
            return aAmount;
        }

        /// <summary>
        /// Возвращение размера топливного бака
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Размер топливного бака</returns>
        public int Capacity(Ship aShip)
        {
            return ciMaxFuelCount;
        }
    }
}