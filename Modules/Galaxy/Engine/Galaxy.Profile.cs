/////////////////////////////////////////////////
//
// Галактический профиль игрока
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2020.02.29
//
/////////////////////////////////////////////////

using System;
using Empire.Modules.Classes;

namespace Empire.Planetary
{
    /// <summary>
    /// Класс управления галактическим профилем
    /// </summary>
    internal class GalaxyProfile : IDisposable
    {
        /// <summary>
        /// Игрок владеющий профилем
        /// </summary>
        private Player fPlayer { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aPlayer">Экземпляр игрока</param>
        public GalaxyProfile(Player aPlayer)
        {
            fPlayer = aPlayer;
        }
        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {

        }
    }
}