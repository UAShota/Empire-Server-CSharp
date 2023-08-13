/////////////////////////////////////////////////
//
// Описание кораблика
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Empire.Modules.Classes;

namespace Empire.Planetary.Classes
{
    /// <summary>
    /// Технологии корабля
    /// </summary>
    internal enum ShipTech
    {
        /// <summary>
        /// Пустая теха
        /// </summary>
        Empty,
        /// <summary>
        /// Оружие пули
        /// </summary>
        WeaponBullet,
        /// <summary>
        /// Возможность постройки
        /// </summary>
        Active,
        /// <summary>
        /// Аннигиляция
        /// </summary>
        Annihilation,
        /// <summary>
        /// Броня
        /// </summary>
        Armor,
        /// <summary>
        /// Запрет разбора конструктором
        /// </summary>
        SolidBody,
        /// <summary>
        /// Цена
        /// </summary>
        Cost,
        /// <summary>
        /// Количество в стеке
        /// </summary>
        Count,
        /// <summary>
        /// Множитель урона
        /// </summary>
        Damage,
        /// <summary>
        /// Самопочинка
        /// </summary>
        Repair,
        /// <summary>
        /// Скрытое перемещение
        /// </summary>
        Hidden,
        /// <summary>
        /// Структура
        /// </summary>
        Hp,
        /// <summary>
        /// Размер топливного бака
        /// </summary>
        __empty_0,
        /// <summary>
        /// Безлимитный портал
        /// </summary>
        StablePortal,
        /// <summary>
        /// Выработка ресурса
        /// </summary>
        Production,
        /// <summary>
        /// Починка других корабликов
        /// </summary>
        Fix,
        /// <summary>
        /// Переносное хранилище
        /// </summary>
        Storage,
        /// <summary>
        /// Вес кораблика
        /// </summary>
        Weight,
        /// <summary>
        /// Скилл разбора
        /// </summary>
        SkillConstructor,
        /// <summary>
        /// Элитка разбор дополнительных противников
        /// </summary>
        SkillConstructorEnemy,
        /// <summary>
        /// Элитка разбор в дополнительных союзников
        /// </summary>
        SkillConstructorFriend,
        /// <summary>
        /// Стационарность
        /// </summary>
        Stationary,
        /// <summary>
        /// Приоритет атаки
        /// </summary>
        Priority,
        /// <summary>
        /// Использование нижней орбиты
        /// </summary>
        LowOrbit,
        /// <summary>
        /// Слет с планетоида в бою
        /// </summary>
        Faster,
        /// <summary>
        /// Защита от атак артилерии с другой планеты
        /// </summary>
        RangeDefence,
        /// <summary>
        /// Оружие прострела через кораблик
        /// </summary>
        WeaponOvershot,
        /// <summary>
        /// Возможность влета в тыл
        /// </summary>
        BackzoneIntruder,
        /// <summary>
        /// Возможность блокировать с краев
        /// </summary>
        CornerBlock,
        /// <summary>
        /// Оружие двойная пуля
        /// </summary>
        WeaponDoubleBullet,
        /// <summary>
        /// Цель для прострела
        /// </summary>
        OvershotTarget,
        /// <summary>
        /// Оружие лазер
        /// </summary>
        WeaponLaser,
        /// <summary>
        /// Оружие ракета
        /// </summary>
        WeaponRocket,
        /// <summary>
        /// Захват лояльности
        /// </summary>
        Capturer,
        /// <summary>
        /// Блокиратор прострела
        /// </summary>
        OvershotBlocker,
        /// <summary>
        /// Оружие двойной лазер
        /// </summary>
        WeaponDoubleLaser,
        /// <summary>
        /// Блокиратор быстрых корабликов
        /// </summary>
        FasterBlocker,
        /// <summary>
        /// Стабилизатор ЧТ
        /// </summary>
        WormholeGuard,
        /// <summary>
        /// Строитель корабликов
        /// </summary>
        Construction,
        /// <summary>
        /// Время заправки
        /// </summary>
        __empty_1
    }

    /// <summary>
    /// Тип корабля
    /// </summary>
    internal enum ShipType
    {
        /// <summary>
        /// Пустой
        /// </summary>
        Empty,
        /// <summary>
        /// Пустой тип
        /// </summary>
        _Free_0,
        /// <summary>
        /// Крейсер
        /// </summary>
        Cruiser,
        /// <summary>
        /// Дредноут
        /// </summary>
        Dreadnought,
        /// <summary>
        /// Корвет
        /// </summary>
        Corvete,
        /// <summary>
        /// Девастатор
        /// </summary>
        Devastator,
        /// <summary>
        /// Штурмовик
        /// </summary>
        Invader,
        /// <summary>
        /// Военная база
        /// </summary>
        Millitary,
        /// <summary>
        /// Верфь
        /// </summary>
        Shipyard,
        /// <summary>
        /// Научная станция
        /// </summary>
        Scient,
        /// <summary>
        /// Сервисная платформа
        /// </summary>
        Service,
        /// <summary>
        /// Флагман
        /// </summary>
        Flagship
    }

    /// <summary>
    /// Состояния корабля
    /// </summary>
    internal enum ShipState
    {
        /// <summary>
        /// Заблокирован для использования
        /// </summary>
        Disabled,
        /// <summary>
        /// Активен
        /// </summary>
        Available,
        /// <summary>
        /// Доступен к активации
        /// </summary>
        Interactive
    }

    /// <summary>
    /// Режим корабя
    /// </summary>
    internal enum ShipMode
    {
        /// <summary>
        /// Активен
        /// </summary>
        Active,
        /// <summary>
        /// Блокирован с краев
        /// </summary>
        Blocked,
        /// <summary>
        /// Лимит для активации
        /// </summary>
        Full,
        /// <summary>
        /// Походный режим
        /// </summary>
        Offline
    }

    /// <summary>
    /// Тип перелета
    /// </summary>
    internal enum ShipFlyType
    {
        /// <summary>
        /// Парковка
        /// </summary>
        Parking,
        /// <summary>
        /// Перелет без смены планеты
        /// </summary>
        Local,
        /// <summary>
        /// Перелет со сменой планеты
        /// </summary>
        Global
    }

    /// <summary>
    /// Как уничтожился кораблик
    /// </summary>
    internal enum ShipDestroyMode
    {
        /// <summary>
        /// Тихо исчез
        /// </summary>
        None,
        /// <summary>
        /// Взорвался
        /// </summary>
        Explose
    }

    /// <summary>
    /// Таймеры корабля
    /// </summary>
    internal enum ShipTimer
    {
        /// <summary>
        /// Операция постройки
        /// </summary>
        Construction,
        /// <summary>
        /// Операция прыжка в портал
        /// </summary>
        PortalJump,
        /// <summary>
        /// Операция дозаправки
        /// </summary>
        Refuel,
        /// <summary>
        /// Операция перелета
        /// </summary>
        FlightLocal,
        /// <summary>
        /// Операция полета
        /// </summary>
        FlightGlobal,
        /// <summary>
        /// Операция аннигиляции
        /// </summary>
        Annihilation,
        /// <summary>
        /// Операция самопочинки
        /// </summary>
        Fix,
        /// <summary>
        /// Операция ремонта союзника
        /// </summary>
        Repair,
        /// <summary>
        /// Кулдаун скилла разбора кораблика
        /// </summary>
        Constructor,
        /// <summary>
        /// Следование по пути
        /// </summary>
        PathHope
    }

    /// <summary>
    /// Тип орудийной системы
    /// </summary>
    internal enum ShipWeaponType
    {
        /// <summary>
        /// Центральное
        /// </summary>
        Center,
        /// <summary>
        /// Левое
        /// </summary>
        Left,
        /// <summary>
        /// Правое
        /// </summary>
        Right,
        /// <summary>
        /// Ракета
        /// </summary>
        Rocket
    }

    /// <summary>
    /// Структура описания количества активных/неактивных корабликов
    /// </summary>    
    internal class ShipCount
    {
        /// <summary>
        /// Количество на орбите
        /// </summary>
        public int Exist { get; set; }

        /// <summary>
        /// Количество активных на орбите
        /// </summary>
        public int Active { get; set; }
    }

    /// <summary>
    /// Купленные технологии
    /// </summary>
    internal class ShipTechKeys
    {
        /// <summary>
        /// Массив значений для технологии
        /// </summary>
        private readonly int[] fTech = new int[Enum.GetValues(typeof(ShipTech)).Length];

        /// <summary>
        /// Установка и получение значения технологии
        /// </summary>
        /// <param name="aShipTech">Технология</param>
        /// <returns>Значение технологии</returns>
        public int this[ShipTech aShipTech]
        {
            get => fTech[(int)aShipTech];
            set => fTech[(int)aShipTech] = value;
        }
    }

    /// <summary>
    /// Значения технологий для кораблика
    /// </summary>
    internal class ShipTechValues
    {
        /// <summary>
        /// Массив купленных технологий кораблика
        /// </summary>
        private readonly ShipTechKeys[] fTechKeys = new ShipTechKeys[Enum.GetValues(typeof(ShipType)).Length];

        /// <summary>
        /// Конструктор
        /// </summary>
        public ShipTechValues()
        {
            for (int tmpI = 0; tmpI < fTechKeys.Length; tmpI++)
                fTechKeys[tmpI] = new ShipTechKeys();
        }

        /// <summary>
        /// Купленная технологии кораблика
        /// </summary>
        /// <param name="aShipTech">Тип кораблика</param>
        /// <returns>Купленная технология</returns>
        public ShipTechKeys this[ShipType aShipType]
        {
            get => fTechKeys[(int)aShipType];
            set => fTechKeys[(int)aShipType] = value;
        }
    }

    /// <summary>
    /// Ссылка на технологии для кораблика
    /// </summary>
    internal class ShipTechUnit
    {
        /// <summary>
        /// Массив купленных технологий кораблика
        /// </summary>
        private readonly TechItem[] fTechItems = new TechItem[Enum.GetValues(typeof(ShipTech)).Length];

        /// <summary>
        /// Технология кораблика
        /// </summary>
        /// <param name="aShipTech">Технология</param>
        /// <returns>Технология кораблика</returns>
        public TechItem this[ShipTech aShipTech]
        {
            get => fTechItems[(int)aShipTech];
            set => fTechItems[(int)aShipTech] = value;
        }

        /// <summary>
        /// Возвращение значения для технологии
        /// </summary>
        /// <param name="aShipTech">Технология кораблика</param>
        /// <param name="aLevel">Уровень прокачки</param>
        /// <returns>Значение технологии</returns>
        public int Value(ShipTech aShipTech, int aLevel)
        {
            return fTechItems[(int)aShipTech].Levels[aLevel];
        }

        /// <summary>
        /// Признак поддержки технологии
        /// </summary>
        /// <param name="aShipTech">Технология кораблика</param>
        /// <param name="aLevel">Уровень прокачки</param>
        /// <returns>Поддерживаемость технологии</returns>
        public bool IsSupported(ShipTech aShipTech, int aLevel)
        {
            var tmpTech = fTechItems[(int)aShipTech];
            return ((tmpTech != null) && (tmpTech.Levels[aLevel] > 0));
        }
    }

    /// <summary>
    /// Ссылка на технологии для профиля
    /// </summary>
    internal class ShipTechProfile
    {
        /// <summary>
        /// Массив технологий корабликов
        /// </summary>
        private readonly ShipTechUnit[] fTechUnits = new ShipTechUnit[Enum.GetValues(typeof(ShipType)).Length];

        /// <summary>
        /// Конструктор
        /// </summary>
        public ShipTechProfile()
        {
            for (int tmpI = 0; tmpI < fTechUnits.Length; tmpI++)
                fTechUnits[tmpI] = new ShipTechUnit();
        }

        /// <summary>
        /// Технология для типа кораблика
        /// </summary>
        /// <param name="aShipType">Тип кораблика</param>
        /// <returns>Технологии кораблика</returns>
        public ShipTechUnit this[ShipType aShipType]
        {
            get => fTechUnits[(int)aShipType];
            set => fTechUnits[(int)aShipType] = value;
        }
    }

    /// <summary>
    /// Технологии для всех рас
    /// </summary>
    internal class ShipTechRace
    {
        /// <summary>
        /// Массив технологий профиля (по расе)
        /// </summary>
        private readonly ShipTechProfile[] fTechProfile = new ShipTechProfile[Enum.GetValues(typeof(PlayerRace)).Length];

        /// <summary>
        /// Конструктор
        /// </summary>
        public ShipTechRace()
        {
            for (int tmpI = 0; tmpI < fTechProfile.Length; tmpI++)
                fTechProfile[tmpI] = new ShipTechProfile();
        }

        /// <summary>
        /// Технология для расы
        /// </summary>
        /// <param name="aRace">Тип расы</param>
        /// <returns>Технологии кораблика</returns>
        public ShipTechProfile this[PlayerRace aRace]
        {
            get => fTechProfile[(int)aRace];
            set => fTechProfile[(int)aRace] = value;
        }
    }

    /// <summary>
    /// Планетарный кораблик
    /// </summary>
    internal class Ship : TimerObject
    {
        /// <summary>
        /// Ссылка на технологии типа
        /// </summary>
        private ShipTechUnit fTechUnit { get; }

        /// <summary>
        /// Ссылка на купленные технологии
        /// </summary>
        private ShipTechKeys fTechKeys { get; }

        /// <summary>
        /// Слот посадки
        /// </summary>
        private Landing fLanding { get; set; }

        /// <summary>
        /// Идентификатор кораблика
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Владелец кораблика
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Тип кораблика
        /// </summary>
        public ShipType ShipType { get; }

        /// <summary>
        /// Режим
        /// </summary>
        public ShipMode Mode { get; set; }

        /// <summary>
        /// Состояние
        /// </summary>
        public ShipState State { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Здоровье
        /// </summary>
        public int HP { get; set; }

        /// <summary>
        /// Объект постройки
        /// </summary>
        public Construction Construction { get; set; }

        /// <summary>
        /// Противник на центральное орудие
        /// </summary>
        public Ship TargetCenter { get; set; }

        /// <summary>
        /// Противник на левое орудие
        /// </summary>
        public Ship TargetLeft { get; set; }

        /// <summary>
        /// Противник на правое орудие
        /// </summary>
        public Ship TargetRight { get; set; }

        /// <summary>
        /// Противник на ракетное орудие
        /// </summary>
        public Ship TargetRocket { get; set; }

        /// <summary>
        /// Количество топлива
        /// </summary>
        public int Fuel { get; set; }

        /// <summary>
        /// Количество убитых стеков
        /// </summary>
        public int Destructed { get; set; }

        /// <summary>
        /// Планета, к которой привязан кораблик
        /// </summary>
        public Planet Attach { get; set; }

        /// <summary>
        /// Планета корабля
        /// </summary>
        public Planet Planet => Landing.Planet;

        /// <summary>
        /// Слот на планете
        /// </summary>
        public Landing Landing { get => fLanding; set => SetLanding(value); }

        /// <summary>
        /// Тип уничтожения кораблика
        /// </summary>
        public ShipDestroyMode DestroyMode { get; set; }

        /// <summary>
        /// Список планет в пути перелета
        /// </summary>
        public List<Planet> Path { get; set; }

        /// <summary>
        /// Выход портала
        /// </summary>
        public Planet Portal
        {
            get
            {
                if (Planet == Planet.Portal.In)
                    return Planet.Portal.Out;
                else
                    return Planet.Portal.In;
            }
        }

        /// <summary>
        /// Признак изменения параметров кораблика
        /// </summary>
        public bool IsChanged { get; set; }

        /// <summary>
        /// Признак что кораблик активен
        /// </summary>
        public bool IsActive => (State == ShipState.Available)
                             && (Mode == ShipMode.Active);

        /// <summary>
        /// Возможность взаимодействовать с кораблем
        /// </summary>
        public bool IsOperable => (State == ShipState.Available)
                               || (State == ShipState.Interactive);

        /// <summary>
        /// Признак, что кораблик может начать захват планеты
        /// </summary>
        public bool IsCapture => (Attach == Planet)
                              && IsActive
                              && TechActive(ShipTech.Capturer)
                              && !Planet.Owner.IsRoleFriend(Owner);

        /// <summary>
        /// Признак прикрепления на стрельбу с другой планеты
        /// </summary>
        public bool IsRangeAttach => (Attach != null)
                                  && (Attach != Landing.Planet);

        /// <summary>
        /// Признак автоприцела
        /// </summary>
        public bool IsAutoTarget { get; set; }

        /// <summary>
        /// Признак нацеленного кораблика
        /// </summary>
        /// <returns>Нацеленность кораблика</returns>
        public bool IsTargeted
        {
            get => (TargetCenter != null)
                || (TargetLeft != null)
                || (TargetRight != null)
                || (TargetRocket != null);
            set
            {
                TargetCenter = null;
                TargetLeft = null;
                TargetRight = null;
                TargetRocket = null;
            }
        }

        /// <summary>
        /// Смена идентификатора корабля
        /// </summary>
        /// <param name="aLanding">Новая позиция</param>
        private void SetLanding(Landing aLanding)
        {
            fLanding = aLanding;
            fLanding.Ship = this;
            ID = fLanding.Planet.ID << 16 | fLanding.Position;
        }

        /// <summary>
        /// Возвращение количества типов таймера
        /// </summary>
        /// <returns>Количество типов таймеров</returns>
        protected override int GetTimersCount()
        {
            return Enum.GetNames(typeof(ShipTimer)).Length;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aOwner">Владелец кораблика</param>
        /// <param name="aShipType">Тип кораблика</param>
        public Ship(Player aOwner, ShipType aShipType)
        {
            Owner = aOwner;
            ShipType = aShipType;
            fTechUnit = Owner.Planetary.TechShipProfile[ShipType];
            fTechKeys = Owner.Planetary.TechShipValues[ShipType];
        }

        /// <summary>
        /// Доступность технологии
        /// </summary>
        /// <param name="aTechType">Тип технологии</param>
        /// <returns>Доступность технологии</returns>
        public bool TechActive(ShipTech aTechType)
        {
            TechItem tmpTech = fTechUnit[aTechType];
            return (tmpTech != null) && (tmpTech.Count == 1 || tmpTech.Levels[fTechKeys[aTechType]] > 0);
        }

        /// <summary>
        /// Значение технологии
        /// </summary>
        /// <param name="aTechType">Тип технологии</param>
        /// <returns>Значение технологии</returns>
        public int TechValue(ShipTech aTechType)
        {
            TechItem tmpTech = fTechUnit[aTechType];
            return tmpTech != null ? fTechUnit[aTechType].Levels[fTechKeys[aTechType]] : 0;
        }

        /// <summary>
        /// Время отката технологии
        /// </summary>
        /// <param name="aTechType">Тип технологии</param>
        /// <returns>Миллисекунды отката технологии</returns>
        public int TechCooldown(ShipTech aTechType)
        {
            return fTechUnit[aTechType].Cooldowns[fTechKeys[aTechType]];
        }

        /// <summary>
        /// Возвращение активности таймера
        /// </summary>
        /// <param name="aTimer">Таймер</param>
        /// <returns>Признак включенного таймера</returns>
        public bool TimerEnabled(ShipTimer aTimer)
        {
            return Timers[(int)aTimer] != null;
        }

        /// <summary>
        /// Возвращение времени таймера
        /// </summary>
        /// <param name="aTimer">Таймер</param>
        /// <returns>Признак включенного таймера</returns>
        public long TimerValue(ShipTimer aTimer)
        {
            Timer tmpTimer = Timers[(int)aTimer];
            return tmpTimer != null ? tmpTimer.Time : 0;
        }
    }
}