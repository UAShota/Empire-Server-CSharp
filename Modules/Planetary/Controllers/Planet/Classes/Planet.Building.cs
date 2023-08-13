/////////////////////////////////////////////////
//
// Описание строения
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
    /// Типы строений
    /// </summary>
    internal enum BuildingType
    {
        /// <summary>
        /// Пустая теха
        /// </summary>
        Empty,
        /// <summary>
        /// Электростанция
        /// </summary>
        Electro,
        /// <summary>
        /// Склад
        /// </summary>
        Warehouse,
        /// <summary>
        /// Модули
        /// </summary>
        Modules,
        /// <summary>
        /// Ксенон
        /// </summary>
        Xenon,
        /// <summary>
        /// Титан
        /// </summary>
        Titan,
        /// <summary>
        /// Кремний
        /// </summary>
        Kremniy,
        /// <summary>
        /// Кристалы
        /// </summary>
        Crystal,
        /// <summary>
        /// Комбинат еды
        /// </summary>
        CombinatFarm,
        /// <summary>
        /// Комбинат электрочастей
        /// </summary>
        CombinatElectro,
        /// <summary>
        /// Комбинат антипротонок
        /// </summary>
        CombinatCrystal,
        /// <summary>
        /// Комбинат плазмы
        /// </summary>
        CombinatXenon,
        /// <summary>
        /// Завод кредитов
        /// </summary>
        MakeCredits,
        /// <summary>
        /// Завод кварков
        /// </summary>
        MakeKvark,
        /// <summary>
        /// Завод модулей
        /// </summary>
        MakeModules,
        /// <summary>
        /// Завод топлива
        /// </summary>
        MakeFuel,
        /// <summary>
        /// Завод мин
        /// </summary>
        MakeMines,
        /// <summary>
        /// Завод ремонтных дроидов
        /// </summary>
        MakeDrules,
        /// <summary>
        /// Завод защитных дроидов
        /// </summary>
        MakeArmor,
        /// <summary>
        /// Завод ударных дроидов
        /// </summary>
        MakeDamage
    }

    /// <summary>
    /// Режимы производства
    /// </summary>
    internal enum BuildingMode
    {
        /// <summary>
        /// Первичное
        /// </summary>
        Primary,
        /// <summary>
        /// Вторичное
        /// </summary>
        Secondary
    }

    /// <summary>
    /// Технологии строений
    /// </summary>
    internal enum BuildingTech
    {
        /// <summary>
        /// Стоимость покупки
        /// </summary>
        BuyCost,
        /// <summary>
        /// Стоимость апгрейда
        /// </summary>
        UpgradeCost,
        /// <summary>
        /// Признак открытого строения
        /// </summary>
        Active,
        /// <summary>
        /// Расход и выработка энергии
        /// </summary>
        Energy,
        /// <summary>
        /// Увеличение потолка лояльности планетоида
        /// </summary>
        Capture,
        /// <summary>
        /// Количество 1 ресурса для 1 режима
        /// </summary>
        ResInCount11,
        /// <summary>
        /// Количество 2 ресурса для 1 режима
        /// </summary>
        ResInCount21,
        /// <summary>
        /// Количество 1 ресурса для 2 режима
        /// </summary>
        ResInCount12,
        /// <summary>
        /// Количество 2 ресурс для 2 режима
        /// </summary>
        ResInCount22,
        /// <summary>
        /// Количество 2 ресурс для 2 режима
        /// </summary>
        ResOutCount1,
        /// <summary>
        /// Количество выходного ресурса 2 режима
        /// </summary>
        ResOutCount2,
        /// <summary>
        /// Тип 1 ресурса для 1 режима
        /// </summary>
        ResInId11,
        /// <summary>
        /// Тип 2 ресурса для 1 режима
        /// </summary>
        ResInId21,
        /// <summary>
        /// Тип 1 ресурса для 2 режима
        /// </summary>
        ResInId12,
        /// <summary>
        /// Тип 2 ресурса для 2 режима
        /// </summary>
        ResInId22,
        /// <summary>
        /// Тип выходного ресурса для 1 режима
        /// </summary>
        ResOutId1,
        /// <summary>
        /// Тип выходного ресурса для 1 режима
        /// </summary>
        ResOutId2
    }

    /// <summary>
    /// Элемент технологии словаря
    /// </summary>
    internal class BuildingTechItem
    {
        /// <summary>
        /// Поддерживаемость технологии
        /// </summary>
        public bool Supported { get; set; }

        /// <summary>
        /// Количество уровней технологий
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Список значений для уровня
        /// </summary>
        public int[] Levels { get; set; }

        /// <summary>
        /// Текущий уровень
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public int Value { get; set; }
    }

    /// <summary>
    /// Купленные технологии
    /// </summary>
    internal class BuildingTechKeys
    {
        /// <summary>
        /// Массив значений для технологии
        /// </summary>
        private readonly int[] fTech = new int[Enum.GetValues(typeof(BuildingTech)).Length];

        /// <summary>
        /// Установка и возврат значения технологии
        /// </summary>
        /// <param name="aBuildingTech">Технология</param>
        /// <returns>Значение технологии</returns>
        public int this[BuildingTech aBuildingTech]
        {
            get => fTech[(int)aBuildingTech];
            set => fTech[(int)aBuildingTech] = value;
        }
    }

    /// <summary>
    /// Значения технологий для строения
    /// </summary>
    internal class BuildingTechValues
    {
        /// <summary>
        /// Массив купленных технологий строения
        /// </summary>
        private readonly BuildingTechKeys[] fTechKeys = new BuildingTechKeys[Enum.GetValues(typeof(BuildingType)).Length];

        /// <summary>
        /// Конструктор
        /// </summary>
        public BuildingTechValues()
        {
            for (int tmpI = 0; tmpI < fTechKeys.Length; tmpI++)
                fTechKeys[tmpI] = new BuildingTechKeys();
        }

        /// <summary>
        /// Возврат купленной технологии строения
        /// </summary>
        /// <param name="aBuildingType">Тип строения</param>
        /// <returns>Купленная технология</returns>
        public BuildingTechKeys this[BuildingType aBuildingType]
        {
            get => fTechKeys[(int)aBuildingType];
            set => fTechKeys[(int)aBuildingType] = value;
        }
    }

    /// <summary>
    /// Ссылка на технологии для кораблика
    /// </summary>
    internal class BuildingTechUnit
    {
        /// <summary>
        /// Массив купленных технологий строения
        /// </summary>
        private readonly BuildingTechItem[] fTechItems = new BuildingTechItem[Enum.GetValues(typeof(BuildingTech)).Length];

        /// <summary>
        /// Возврат технологий строения
        /// </summary>
        /// <param name="aBuildingTech">Технология</param>
        /// <returns>Технология кораблика</returns>
        public BuildingTechItem this[BuildingTech aBuildingTech]
        {
            get => fTechItems[(int)aBuildingTech];
            set => fTechItems[(int)aBuildingTech] = value;
        }
    }

    /// <summary>
    /// Ссылка на технологии для профиля
    /// </summary>
    internal class BuildingTechProfile
    {
        /// <summary>
        /// Массив технологий строений
        /// </summary>
        private readonly BuildingTechUnit[] fTechUnits = new BuildingTechUnit[Enum.GetValues(typeof(BuildingType)).Length];

        /// <summary>
        /// Конструктор
        /// </summary>
        public BuildingTechProfile()
        {
            for (int tmpI = 0; tmpI < fTechUnits.Length; tmpI++)
                fTechUnits[tmpI] = new BuildingTechUnit();
        }

        /// <summary>
        /// Возврат технологий для типа строения
        /// </summary>
        /// <param name="aBuildingType">Тип строения</param>
        /// <returns>Технологии кораблика</returns>
        public BuildingTechUnit this[BuildingType aBuildingType]
        {
            get => fTechUnits[(int)aBuildingType];
            set => fTechUnits[(int)aBuildingType] = value;
        }
    }

    /// <summary>
    /// Технологии для всех рас
    /// </summary>
    internal class BuildingTechRace
    {
        /// <summary>
        /// Массив технологий профиля (по расе)
        /// </summary>
        private readonly BuildingTechProfile[] fTechProfile = new BuildingTechProfile[Enum.GetValues(typeof(PlayerRace)).Length];

        /// <summary>
        /// Конструктор
        /// </summary>
        public BuildingTechRace()
        {
            for (int tmpI = 0; tmpI < fTechProfile.Length; tmpI++)
                fTechProfile[tmpI] = new BuildingTechProfile();
        }

        /// <summary>
        /// Возврат технологий для расы
        /// </summary>
        /// <param name="aRace">Тип расы</param>
        /// <returns>Технологии кораблика</returns>
        public BuildingTechProfile this[PlayerRace aRace]
        {
            get => fTechProfile[(int)aRace];
            set => fTechProfile[(int)aRace] = value;
        }
    }

    /// <summary>
    /// Зона построек
    /// </summary>
    internal class BuildingZone
    {
        /// <summary>
        /// Размер поля
        /// </summary>
        private int fSize { get; }

        /// <summary>
        /// Массив строений
        /// </summary>
        private Building[] fSlots { get; }

        /// <summary>
        /// Конструктор определяет слоты
        /// </summary>
        /// <param name="aSize"></param>
        public BuildingZone(int aSize)
        {
            fSize = aSize;
            fSlots = new Building[fSize];
            // Создадим площадки строений
            for (int tmpIndex = 0; tmpIndex < aSize; tmpIndex++)
                fSlots[tmpIndex] = new Building();
        }

        /// <summary>
        /// Получение строения
        /// </summary>
        /// <param name="aPosition">Позиция строения</param>
        /// <returns>Строение</returns>
        public Building this[int aPosition] => fSlots[aPosition];
    }

    /// <summary>
    /// Планетарное строение
    /// </summary>
    internal class Building
    {
        /// <summary>
        /// Ссылка на технологии строения
        /// </summary>
        private BuildingTechUnit fTechUnit { get; set; }

        /// <summary>
        /// Ссылка на купленные технологии
        /// </summary>
        private BuildingTechKeys fTechKeys { get; set; }

        /// <summary>
        /// Тип здания
        /// </summary>
        public BuildingType Type { get; private set; }

        /// <summary>
        /// Активный режим работы
        /// </summary>
        public BuildingMode Mode { get; private set; }

        /// <summary>
        /// Планета строения
        /// </summary>
        public Planet Planet { get; private set; }

        /// <summary>
        /// Уровень здания
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Позиция площадки расположения
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Признак строительства
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Структура строения, 1000 максимум
        /// </summary>
        public int HP { get; private set; }

        /// <summary>
        /// Смена набора технологий
        /// </summary>
        /// <param name="aTechUnit">Ссылка на технологию кораблика</param>
        /// <param name="aTechKeys">Ссылка на купленные технологии</param>
        public void ChangeTech(BuildingTechUnit aTechUnit, BuildingTechKeys aTechKeys)
        {
            fTechUnit = aTechUnit;
            fTechKeys = aTechKeys;
        }
    }
}