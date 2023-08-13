/////////////////////////////////////////////////
//
// Описание планетоида
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Empire.Modules.Classes;

namespace Empire.Planetary.Classes
{
    /// <summary>
    /// Таймеры планеты
    /// </summary>
    internal enum PlanetTimer
    {
        /// <summary>
        /// Боевой тик
        /// </summary>
        Battle,
        /// <summary>
        /// Захват лояльности
        /// </summary>
        Capture,
        /// <summary>
        /// Тик смены состояния
        /// </summary>
        Activity,
        /// <summary>
        /// Активен гравитационный потенциал после аннигиляции
        /// </summary>
        LowGravity
    }

    /// <summary>
    /// Состояние планетоида
    /// </summary>
    internal enum PlanetState
    {
        /// <summary>
        /// Активен
        /// </summary>
        Active,
        /// <summary>
        /// Активируется
        /// </summary>
        Activation,
        /// <summary>
        /// Неактивен
        /// </summary>
        Inactive
    }

    /// <summary>
    /// Типы планет
    /// </summary>
    internal enum PlanetType
    {
        /// <summary>
        /// Маленькая
        /// </summary>
        Earth,
        /// <summary>
        /// Звезда
        /// </summary>
        Sun,
        /// <summary>
        /// Гидросостав
        /// </summary>
        Hydro,
        /// <summary>
        /// Карлик
        /// </summary>
        Rock,
        /// <summary>
        /// Черная дыра
        /// </summary>
        Hole,
        /// <summary>
        /// Пульсар
        /// </summary>
        Pulsar
    }

    /// <summary>
    /// Режим планеты
    /// </summary>
    internal enum PlanetMode
    {
        /// <summary>
        /// Обычная
        /// </summary>
        Normal,
        /// <summary>
        /// Большая
        /// </summary>
        Big
    }

    /// <summary>
    /// Планетарная планета
    /// </summary>
    internal class Planet : TimerObject
    {
        /// <summary>
        /// Количество слотов для малой планеты
        /// </summary>
        private int ciSmallSize => 7;

        /// <summary>
        /// Количество слотов для большой планеты
        /// </summary>
        private int ciBigSize => 10;

        /// <summary>
        /// Уникальный идентификатор в базе
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Владелец планеты
        /// </summary>
        public Player Owner { get; set; }

        /// <summary>
        /// Имя планеты
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип планеты
        /// </summary>
        public PlanetType Type { get; set; }

        /// <summary>
        /// Режим планетоида
        /// </summary>
        public PlanetMode Mode { get; set; }

        /// <summary>
        /// Уровень энергии на планете
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// Уровень планеты
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Координаты на сетке по X
        /// </summary>
        public int CoordX { get; }

        /// <summary>
        /// Координаты на сетке по Y
        /// </summary>
        public int CoordY { get; }

        /// <summary>
        /// Квадрат на сетке по X
        /// </summary>
        public int PosX { get; }

        /// <summary>
        /// Квадрат на сетке по Y
        /// </summary>
        public int PosY { get; }

        /// <summary>
        /// Идентификатор захватчика
        /// </summary>
        public Player CapturePlayer { get; set; }

        /// <summary>
        /// Уровень лояльности захватчика
        /// </summary>
        public int CaptureValue { get; set; }

        /// <summary>
        /// Ресурс, вырабатываемый планетой
        /// </summary>
        public ResourceType ResFactory { get; set; }

        /// <summary>
        /// Исходящий торговый пути с планеты
        /// </summary>
        public Planet ResPathOut { get; }

        /// <summary>
        /// Признак включенного производства
        /// </summary>
        public bool HaveProduction { get; }

        /// <summary>
        /// Состояние активности планеты
        /// </summary>
        public PlanetState State { get; set; }

        /// <summary>
        /// Топливный запас
        /// </summary>
        public int FuelCapacity { get; set; }

        /// <summary>
        /// Ссылка на портал для планеты
        /// </summary>
        public Portal Portal { get; set; }

        /// <summary>
        /// Количество атакующих
        /// </summary>
        public int CaptureAttackers { get; set; }

        /// <summary>
        /// Количество защищающихся
        /// </summary>
        public int CaptureDefenders { get; set; }

        /// <summary>
        /// Признак большой черной дыры
        /// </summary>
        public bool IsBigHole => (Type == PlanetType.Hole)
                              && (Mode == PlanetMode.Big);

        /// <summary>
        /// Признак округи черной дыры
        /// </summary>
        public bool IsBigEdge { get; set; }

        /// <summary>
        /// Список кораблей планеты
        /// </summary>
        public List<Ship> Ships { get; }

        /// <summary>
        /// Список количества кораблей каждого участника
        /// </summary>
        public Dictionary<Player, ShipCount> ShipCount { get; }

        /// <summary>
        /// Список внешних кораблей, нацеленных на планету
        /// </summary>
        public List<Ship> RangeAttackers { get; }

        /// <summary>
        /// Посадочные места планеты
        /// </summary>
        public LandingZone LandingZone { get; }

        /// <summary>
        /// Строения планеты
        /// </summary>
        public BuildingZone BuildingZone { get; }

        /// <summary>
        /// Хранилище планеты
        /// </summary>
        public StorageZone StorageZone { get; }

        /// <summary>
        /// Количество отрядов каждого игрока, определяющие полную видимость планетоида
        /// </summary>
        public Dictionary<Player, int> PlayerLightSoft { get; }

        /// <summary>
        /// Количество отрядов каждого игрока, определяющие частичную видимость планетоида
        /// </summary>
        public Dictionary<Player, int> PlayerLightHard { get; }

        /// <summary>
        /// Количество отрядов каждого игрока, которые определяют область закраски
        /// </summary>
        public Dictionary<Player, int> PlayerCoverage { get; }

        /// <summary>
        /// Список соседних планет в радиусе перелета
        /// </summary>
        public List<Planet> Links { get; }

        /// <summary>
        /// Возвращение количества типов таймера
        /// </summary>
        /// <returns>Количество типов таймеров</returns>
        protected override int GetTimersCount()
        {
            return Enum.GetNames(typeof(PlanetTimer)).Length;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aID">Уникальный идентификатор</param>
        /// <param name="aCoordX">Координата X</param>
        /// <param name="aCoordY">Координата Y</param>
        /// <param name="aType">Тип планеты</param>
        /// <param name="aType">Режим планеты</param>
        public Planet(int aID, int aCoordX, int aCoordY, int aPosX, int aPosY, PlanetType aType, PlanetMode aMode)
        {
            ID = aID;
            CoordX = aCoordX;
            CoordY = aCoordY;
            PosX = aPosX;
            PosY = aPosY;
            Type = aType;
            Mode = aMode;
            // Постройки для маленьких планет
            if (aType == PlanetType.Earth)
            {
                if (aMode == PlanetMode.Normal)
                    BuildingZone = new BuildingZone(ciSmallSize);
                else
                    BuildingZone = new BuildingZone(ciBigSize);
            }
            // Вспомогательные модули
            Ships = new List<Ship>();
            Links = new List<Planet>();
            ShipCount = new Dictionary<Player, ShipCount>();
            RangeAttackers = new List<Ship>();
            LandingZone = new LandingZone(this);
            StorageZone = new StorageZone();
            PlayerLightSoft = new Dictionary<Player, int>();
            PlayerLightHard = new Dictionary<Player, int>();
            PlayerCoverage = new Dictionary<Player, int>();
        }

        /// <summary>
        /// Определение видимости для игрока
        /// </summary>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aHardLight">Только текущая планета</param>
        /// <returns>Видимость планеты для игрока</returns>
        public bool VisibleByPlayer(Player aPlayer, bool aHardLight)
        {
            // Активная БЧТ и ее окраина всегда видима
            if (IsBigHole || IsBigEdge)
            {
                if (aHardLight)
                    return State == PlanetState.Active;
                else
                    return true;
            }
            // Выберем словарь
            Dictionary<Player, int> tmpShips;
            if (aHardLight)
                tmpShips = PlayerLightHard;
            else
                tmpShips = PlayerLightSoft;
            // Проверим наличие записей
            if (tmpShips.Count == 0)
                return false;
            // Поищем все вхождения для соседей игрока
            foreach (var tmpPair in tmpShips)
            {
                if (aPlayer.IsRoleFriend(tmpPair.Key))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Определение покрытия для игрока
        /// </summary>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aFullData">Признак наложения дружественных войск на вражейские (зеленый + красный = фиолетовая закраска карты)</param>
        /// <param name="AFriendCount">Количество союзников</param>
        /// <param name="AEnemyCount">Количество противников</param>
        /// <returns>Количество своих корабликов</returns>
        public bool CoverageByPlayer(Player aPlayer, bool aFullData, out bool aHaveFriends, out bool aHaveEnemy)
        {
            bool tmpHaveSelf = false;
            aHaveFriends = false;
            aHaveEnemy = false;
            // Проверим наличие записей
            if (PlayerCoverage.Count == 0)
                return tmpHaveSelf;
            // Поищем все карты контроля
            foreach (var tmpCover in PlayerCoverage)
            {
                PlayerRole tmpRole = aPlayer.Role(tmpCover.Key);
                if (tmpRole == PlayerRole.Self)
                    tmpHaveSelf = true;
                else if (tmpRole == PlayerRole.Enemy)
                    aHaveEnemy = true;
                else if (tmpRole == PlayerRole.Friends)
                    aHaveFriends = true;
            }
            // Если у игрока нет артефакта и есть противник - закрасим зону красным
            if ((!aFullData) && (aHaveFriends))
            {
                aHaveFriends = false;
                tmpHaveSelf = false;
            }
            // Вернем размер покрытия
            return tmpHaveSelf;
        }

        /// <summary>
        /// Определения состояния для роли игрока
        /// </summary>
        /// <param name="aVisible">Предопределенная видимость</param>
        /// <returns>Состояние планетоида</returns>
        public PlanetState StateByVisible(bool aVisible)
        {
            // Состояние показываем только для ЧТ или видимых игроку планет
            if (((Type == PlanetType.Hole) && (State != PlanetState.Inactive)) || (aVisible))
                return State;
            else
                return PlanetState.Inactive;
        }

        /// <summary>
        /// Возвращение активности таймера
        /// </summary>
        /// <param name="aTimer">Таймер</param>
        /// <returns>Признак включенного таймера</returns>
        public bool TimerEnabled(PlanetTimer aTimer)
        {
            return Timers[(int)aTimer] != null;
        }

        /// <summary>
        /// Возвращение времени таймера
        /// </summary>
        /// <param name="aTimer">Таймер</param>
        /// <returns>Признак включенного таймера</returns>
        public long TimerValue(PlanetTimer aTimer)
        {
            Timer tmpTimer = Timers[(int)aTimer];
            return tmpTimer != null ? tmpTimer.Time : 0;
        }
    }
}