/////////////////////////////////////////////////
//
// Слоты посадки на планету
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
    /// Класс слота посадки
    /// </summary>
    internal class Landing
    {
        /// <summary>
        /// Владелец слота
        /// </summary>
        private LandingZone fOwner { get; }

        /// <summary>
        /// кораблик в слоте
        /// </summary>
        public Ship Ship { get; set; }

        /// <summary>
        /// Ресурс в слоте
        /// </summary>
        public Resource Resource { get; set; }

        /// <summary>
        /// Индекс слота в списке
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Предыдущий слот боевой посадки
        /// </summary>
        public Landing Prev => fOwner[Position + 1, true];

        /// <summary>
        /// Следующий слот боевой посадки
        /// </summary>
        public Landing Next => fOwner[Position - 1, true];

        /// <summary>
        /// Планета слота
        /// </summary>
        public Planet Planet => fOwner.Planet;

        /// <summary>
        /// Признак внутренней орбиты
        /// </summary>
        public bool IsLowOrbit { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aOwner">Владелец посадочного слота</param>
        /// <param name="aPosition">Позиция в списке</param>
        public Landing(LandingZone aOwner, int aPosition, bool aIsLowOrbit)
        {
            fOwner = aOwner;
            Position = aPosition;
            IsLowOrbit = aIsLowOrbit;
        }
    }

    /// <summary>
    /// Посадочные места планетоида
    /// </summary>
    internal class LandingZone
    {
        /// <summary>
        /// Количество боевых слотов
        /// </summary>
        private static int ciFightCount => 14;

        /// <summary>
        /// Количество доступных слотов
        /// </summary>
        private static int ciTotalCount => ciFightCount + 5;

        /// <summary>
        /// Массив посадочных мест
        /// </summary>
        private Landing[] fSlots { get; }

        /// <summary>
        /// Планета посадочной зоны
        /// </summary>
        public Planet Planet { get; private set; }

        /// <summary>
        /// Конструктор определяет слоты
        /// </summary>
        public LandingZone(Planet aPlanet)
        {
            Planet = aPlanet;
            fSlots = new Landing[ciTotalCount + 1];
            // Создадим посадочные зоны
            for (int tmpIndex = 0; tmpIndex <= ciTotalCount; tmpIndex++)
                fSlots[tmpIndex] = new Landing(this, tmpIndex, tmpIndex > ciFightCount);
        }

        /// <summary>
        /// Получение слота боевой посадочной зоны
        /// </summary>
        /// <param name="aPosition">Позиция слота</param>
        /// <returns>Слот посадочной зоны</returns>
        public Landing this[int aPosition, bool aOnlyFight = false]
        {
            get
            {
                if (aOnlyFight)
                {
                    // Если больше - перейдем на начало
                    if (aPosition > ciFightCount)
                        aPosition = aPosition - ciFightCount - 1;
                    // Если меньше - перейдем на конец
                    else if (aPosition < 0)
                        aPosition = ciFightCount + aPosition + 1;
                }
                // Вернем слот по этой позиции
                return fSlots[aPosition];
            }
        }

        /// <summary>
        /// Попытка взять слот по его индексу
        /// </summary>
        /// <param name="aPosition">Индекс слота</param>
        /// <param name="aLanding">Возвращамеый слот</param>
        /// <returns>Существование запрошенного слота</returns>
        public bool TryGet(int aPosition, out Landing aLanding)
        {
            if ((aPosition < 0) || (aPosition > ciTotalCount))
                aLanding = null;
            else
                aLanding = fSlots[aPosition];
            // Вернем результат
            return (aLanding != null);
        }

        /// <summary>
        /// Возвращение случайного слота
        /// </summary>
        /// <returns>Слот посадки</returns>
        public Landing Random(bool aOnlyFight = true)
        {
            int tmpMax;
            // Определим максимум слотов
            if (aOnlyFight)
                tmpMax = ciFightCount;
            else
                tmpMax = ciTotalCount;
            // Разыграем
            return fSlots[new Random().Next(0, tmpMax)];
        }

        /// <summary>
        /// Возвращение крайнего слота для максимума свободных слотов
        /// </summary>
        /// <returns>Слот посадки</returns>
        public Landing Corner()
        {
            // Если флота нет - вернем любой слот
            if (Planet.Ships.Count == 0)
                return Random();
            // Найдем первый слот с кораблем
            Landing tmpLandingPoint = null;
            for (int tmpI = 0; tmpI <= ciFightCount; tmpI++)
            {
                if (Planet.LandingZone[tmpI].Ship != null)
                {
                    tmpLandingPoint = Planet.LandingZone[tmpI];
                    break;
                }
            }
            // Если боевого слота нет - вернем опять же рандом
            if (tmpLandingPoint == null)
                return Random();
            //  Поищем максимальный пустой блок
            int tmpMaxCount = 0;
            int tmpCount = 0;
            Landing tmpMaxLanding = null;
            Landing tmpLanding = tmpLandingPoint.Next;
            // От текущего корабля на верхней орбите пойдем вперед
            while (tmpLanding != tmpLandingPoint)
            {
                // Если в слоте нет корабля - значит доступен для прыжка
                if (tmpLanding.Ship == null)
                {
                    tmpCount++;
                    if (tmpCount > tmpMaxCount)
                    {
                        tmpMaxCount = tmpCount;
                        tmpMaxLanding = tmpLanding;
                    }
                }
                else
                    tmpCount = 0;
                // Перейдем к следующему слоту
                tmpLanding = tmpLanding.Next;
            } 
            // Вернем найденный слот
            return tmpMaxLanding;
        }
    }
}