/////////////////////////////////////////////////
//
// Вспомогательные методы
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2020.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Базовый класс команды
    /// </summary>
    internal class ActionUtils : PlanetaryAccess
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый контроллер</param>
        public ActionUtils(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Поиск союзных портальных установок
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aOwner">Владелец портала</param>
        /// <returns>Наличие союзной портальной установки</returns>
        public bool CheckFriendlyPortaler(Planet aPlanet, Player aOwner)
        {
            // Ищем союзные порталеры
            foreach (Ship tmpShip in aPlanet.Ships)
            {
                if ((tmpShip.TechActive(ShipTech.StablePortal))
                    && (aOwner.IsRoleFriend(tmpShip.Owner)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Добавление таймера
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aTimer">Тип таймера</param>
        /// <param name="aOnTimer">Каллбак срабатывания</param>
        /// <param name="aTime">Время тайминга</param>
        public void TimerAdd(Planet aPlanet, PlanetTimer aTimer, Func<TimerObject, int> aOnTimer, int aTime)
        {
            Engine.Timers.Add(aPlanet, (int)aTimer, aOnTimer, Engine.SocketWriter.PlanetUpdateTimer, aTime);
        }

        /// <summary>
        /// Удаление таймера
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aTimer">Тип таймера</param>
        public void TimerRemove(Planet aPlanet, PlanetTimer aTimer)
        {
            Engine.Timers.Remove(aPlanet.Timers[(int)aTimer]);
        }
    }
}