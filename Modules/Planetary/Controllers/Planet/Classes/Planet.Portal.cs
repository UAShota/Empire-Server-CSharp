/////////////////////////////////////////////////
//
// Описание портала
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System;
using Empire.Modules.Classes;

namespace Empire.Planetary.Classes
{
    /// <summary>
    /// Объект планетарного портала
    /// </summary>
    internal class Portal : IDisposable
    {
        /// <summary>
        /// Планета входа
        /// </summary>
        public Planet In { get; private set; }

        /// <summary>
        /// Планета выхода
        /// </summary>
        public Planet Out { get; private set; }

        /// <summary>
        /// Владелец
        /// </summary>
        public Player Owner { get; private set; }

        /// <summary>
        /// Лимит перемещений
        /// </summary>
        public int Limit { get; private set; }

        /// <summary>
        /// Возможность перебить
        /// </summary>
        public bool Breakable { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aIn">Планета входа</param>
        /// <param name="aOut">Планета выхода</param>
        /// <param name="aOwner">Владелец</param>
        /// <param name="aLimit">Лимит перемещений</param>
        /// <param name="aBreakable">Возможность перебить</param>
        public Portal(Planet aIn, Planet aOut, Player aOwner, int aLimit, bool aBreakable)
        {
            In = aIn;
            Out = aOut;
            Owner = aOwner;
            Limit = aLimit;
            Breakable = aBreakable;
            In.Portal = this;
            Out.Portal = this;
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {
            In.Portal = null;
            Out.Portal = null;
        }
    }
}