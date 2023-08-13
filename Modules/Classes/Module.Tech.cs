/////////////////////////////////////////////////
//
// Класс описания технологии
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

namespace Empire.Modules.Classes
{
    /// <summary>
    /// Элемент технологии
    /// </summary>
    internal class TechItem
    {
        /// <summary>
        /// Количество уровней
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Список значений
        /// </summary>
        public int[] Levels { get; private set; }

        /// <summary>
        /// Список времени отката для каждого уровня
        /// </summary>
        public int[] Cooldowns { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aLevelCount">Количество уровней</param>
        public TechItem(int aLevelCount)
        {
            Count = aLevelCount;
            Levels = new int[aLevelCount + 1];
            Cooldowns = new int[aLevelCount + 1];
        }
    }
}