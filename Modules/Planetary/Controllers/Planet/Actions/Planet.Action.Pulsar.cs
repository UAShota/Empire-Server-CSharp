/////////////////////////////////////////////////
//
// Обработка пульсаров
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс обработки пульсаров
    /// </summary>
    internal class Pulsar : PlanetaryAccess
    {
        /// <summary>
        /// Каллбак события
        /// </summary>
        /// <param name="aPlanet">Планета срабатывания</param>
        /// <returns>Время продления</returns>
        private int OnTimer(TimerObject aPlanet)
        {
            Planet tmpPlanet = (Planet)aPlanet;
            // Передернем
            if (tmpPlanet.State == PlanetState.Active)
                tmpPlanet.State = PlanetState.Activation;
            else
                tmpPlanet.State = PlanetState.Active;
            // Уведомим
            Engine.SocketWriter.PlanetStateUpdate(tmpPlanet, null);
            // Продлим таймер на следующее состояние
            return new Random().Next((int)Math.Round(Engine.TimePulsarActive / 1.5), Engine.TimePulsarActive);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public Pulsar(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Активация таймера пульсара
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aTimer">Тип таймера</param>
        /// <param name="aValue">Значение таймера</param>
        public void Activate(Planet aPlanet, PlanetTimer aTimer, int aValue)
        {
            Engine.Planets.Action.Utils.TimerAdd(aPlanet, aTimer, OnTimer, aValue);
        }
    }
}