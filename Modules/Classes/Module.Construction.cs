/////////////////////////////////////////////////
//
// Класс описания игрока
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2020.02.29
//
/////////////////////////////////////////////////

namespace Empire.Modules.Classes
{
    internal class Construction
    {
        /// <summary>
        /// Количество единиц стройки в тик
        /// </summary>
        public int Speed { get; private set; }

        /// <summary>
        /// Количество построенных единиц
        /// </summary>
        public int Done { get; private set; }

        /// <summary>
        /// Количество единиц для постройки
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aSpeed">Количество единиц стройки в тик</param>
        /// <param name="aTotal">Количество единиц для постройки</param>
        public Construction(int aSpeed, int aTotal)
        {
            Speed = aSpeed;
            Total = aTotal;
        }
    }
}