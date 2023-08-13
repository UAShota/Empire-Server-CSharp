/////////////////////////////////////////////////
//
// Обработка гравиторов
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс обработки гравиторов
    /// </summary>
    internal class Gravitor : PlanetaryAccess
    {
        /// <summary>
        /// Время работы гравитационного потенциала
        /// </summary>
        private int ciTimeGravity => 30000;

        /// <summary>
        /// Каллбак события
        /// </summary>
        /// <param name="aPlanet">Планета срабатывания</param>
        /// <returns>Время продления</returns>
        private int OnTimer(TimerObject aPlanet)
        {
            return 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public Gravitor(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Активация таймера гравитора
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aTimer">Тип таймера</param>
        /// <param name="aValue">Значение таймера</param>
        public void Activate(Planet aPlanet, PlanetTimer aTimer, int aValue)
        {
            Engine.Planets.Action.Utils.TimerAdd(aPlanet, aTimer, OnTimer, aValue);
        }

        /// <summary>
        /// Запуск гравитационный потенциала
        /// </summary>
        /// <param name="aShip">Подрываемый кораблик</param>
        public void Call(Ship aShip)
        {
            Activate(aShip.Planet, PlanetTimer.LowGravity, ciTimeGravity);
        }
    }
}