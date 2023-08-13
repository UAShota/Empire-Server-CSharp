/////////////////////////////////////////////////
//
// Контроллер планет
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс контроллера планет
    /// </summary>
    internal class PlanetController : PlanetaryController
    {
        /// <summary>
        /// Сектор на карте для выявления путей перелета
        /// </summary>
        private class Sector
        {
            /// <summary>
            /// Квадрат X
            /// </summary>
            public int X { get; }

            /// <summary>
            /// Квадрат Y
            /// </summary>
            public int Y { get; }

            /// <summary>
            /// Список планет в этом квадрате
            /// </summary>
            public List<Planet> Planets { get; }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="aX">Квадрат X</param>
            /// <param name="aY">Квадрат Y</param>
            public Sector(int aX, int aY)
            {
                X = aX;
                Y = aY;
                Planets = new List<Planet>();
            }
        }

        /// <summary>
        /// Матрица секторов
        /// </summary>
        private Sector[,] fSectorMatrix { get; set; }

        /// <summary>
        /// Планеты системы
        /// </summary>
        public List<Planet> PlanetList { get; }

        /// <summary>
        /// Установка свойств планеты
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        private void SetProps(Planet aPlanet)
        {
            // Найдем сектор расположения планеты
            Sector tmpSector = fSectorMatrix[aPlanet.PosX, aPlanet.PosY];
            // Если сектор не заселен - создадим
            if (tmpSector == null)
            {
                tmpSector = new Sector(aPlanet.PosX, aPlanet.PosY);
                fSectorMatrix[aPlanet.PosX, aPlanet.PosY] = tmpSector;
            }
            tmpSector.Planets.Add(aPlanet);
            // Добавки к каждому типу планеты
            switch (aPlanet.Type)
            {
                case PlanetType.Earth:
                    // Доп хранилище для жилых планет 
                    aPlanet.StorageZone.Buy();
                    /*Engine. TPlanetThread(Engine).ControlStorages.ChangeStorageCount(tmpPlanet, 1, False); */
                    // Имя для подконтрольной планеты
                    if (aPlanet.Owner.ID > 1)
                        aPlanet.Name = aPlanet.Owner.Name;
                    break;
                case PlanetType.Hole:
                    Action.WormHole.Register(aPlanet);
                    break;
            }
            /*if количество жилых планет (tmpPlanet.Owner == TPlanetThread(Engine).Player) then
           Inc(FCapturedColonyCount);*/
            Action.Control.Call(aPlanet, aPlanet.Owner, true, true);
        }

        /// <summary>
        /// Загрузка планетоидов системы
        /// </summary>
        private void LoadPlanets()
        {
            using (var tmpReader = Core.Database.Query("PLLoadPlanets", Engine.Player.ID))
            {
                while (tmpReader.Read())
                {
                    Planet tmpPlanet = new Planet(
                        // Константные поля
                        tmpReader.ReadInt("ID_Planet"),
                        tmpReader.ReadInt("COORD_X"),
                        tmpReader.ReadInt("COORD_Y"),
                        tmpReader.ReadInt("POS_X"),
                        tmpReader.ReadInt("POS_Y"),
                        (PlanetType)tmpReader.ReadInt("ID_TYPE"),
                        (PlanetMode)tmpReader.ReadInt("ID_MODE"))
                    {
                        // Переменные поля
                        Owner = Core.Server.Auth.FindPlayer(tmpReader.ReadInt("ID_OWNER"), true),
                        Name = tmpReader.ReadString("NAME"),
                        ResFactory = (ResourceType)tmpReader.ReadInt("ID_RESOURCE"),
                        State = (PlanetState)tmpReader.ReadInt("ID_STATE"),
                        Level = 10, /**/
                        FuelCapacity = 30, /**/
                    };
                    // Установим параметры
                    SetProps(tmpPlanet);
                    // Добавим в список
                    PlanetList.Add(tmpPlanet);
                }
            }
        }

        /// <summary>
        /// Загрузка путей перелета между планетами
        /// </summary>
        private void LoadLinks()
        {
            // Переберем массив секторов
            for (int tmpSectorY = 0; tmpSectorY < Engine.SizeY; tmpSectorY++)
            {
                // Переберем столбцы
                for (int tmpSectorX = 0; tmpSectorX < Engine.SizeX; tmpSectorX++)
                {
                    // Пропустим пустые сектора
                    Sector tmpSectorFrom = fSectorMatrix[tmpSectorX, tmpSectorY];
                    if (tmpSectorFrom == null)
                        continue;
                    // Найдем соединение с планетами своего сектора
                    foreach (Planet tmpPlanetFrom in tmpSectorFrom.Planets)
                    {
                        foreach (Planet tmpPlanetTo in tmpSectorFrom.Planets)
                            if (tmpPlanetFrom != tmpPlanetTo)
                                tmpPlanetFrom.Links.Add(tmpPlanetTo);
                    }
                    // Переберем соседние сектора
                    for (int tmpX = -1; tmpX <= 1; tmpX++)
                    {
                        for (int tmpY = -1; tmpY <= 1; tmpY++)
                        {
                            // Отсечем выход за пределы, игнорируем ссылку сам на себя 0 -> 0
                            if (((tmpX == 0) && (tmpY == 0))
                                  || (tmpSectorFrom.X + tmpX < 0)
                                  || (tmpSectorFrom.Y + tmpY < 0)
                                  || (tmpSectorFrom.X + tmpX == Engine.SizeX)
                                  || (tmpSectorFrom.Y + tmpY == Engine.SizeY))
                                continue;
                            // Найдем таргетный сектор
                            Sector tmpSectorTo = fSectorMatrix[tmpSectorFrom.X + tmpX, tmpSectorFrom.Y + tmpY];
                            // Найдем соединение с соседними секторами
                            if (tmpSectorTo == null)
                                continue;
                            // Найдем соединение с соседними секторами
                            foreach (Planet tmpPlanetFrom in tmpSectorFrom.Planets)
                            {
                                foreach (Planet tmpPlanetTo in tmpSectorTo.Planets)
                                {
                                    if (IsValidDistance(tmpPlanetFrom, tmpPlanetTo))
                                        tmpPlanetFrom.Links.Add(tmpPlanetTo);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Загрузка порталов
        /// </summary>
        private void LoadPortals()
        {
            using (var tmpReader = Core.Database.Query("PLLoadPortals", Engine.Player.ID))
            {
                while (tmpReader.Read())
                {
                    int tmpPlanetSource = tmpReader.ReadInt("ID_PLANET_SOURCE");
                    int tmpPlanetTarget = tmpReader.ReadInt("ID_PLANET_TARGET");
                    // Определим планеты
                    Action.Portal.Open(PlanetList[tmpPlanetSource], PlanetList[tmpPlanetTarget], Engine.Player, -1, true);
                }
            }
        }

        /// <summary>
        /// Загрузка таймеров
        /// </summary>
        private void LoadTimers()
        {
            using (var tmpReader = Core.Database.Query("SHLoadTimers", 0, Engine.Player.ID, 0))
            {
                while (tmpReader.Read())
                {
                    Planet tmpPlanet = PlanetList[tmpReader.ReadInt("ID_OBJECT")];
                    PlanetTimer tmpTimer = (PlanetTimer)tmpReader.ReadInt("ID_TIMER");
                    int tmpValue = tmpReader.ReadInt("VALUE");
                    // Запустим тайминги
                    switch (tmpPlanet.Type)
                    {
                        case PlanetType.Hole:
                            Action.WormHole.Activate(tmpPlanet, tmpTimer, tmpValue);
                            break;
                        case PlanetType.Pulsar:
                            Action.Pulsar.Activate(tmpPlanet, tmpTimer, tmpValue);
                            break;
                        default:
                            Core.Log.Warn("No action {0} {1}", tmpPlanet.ID, tmpTimer);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Определение доступности перелета между планетами
        /// </summary>
        /// <param name="aSource">Источник</param>
        /// <param name="aTarget">Цель</param>
        /// <returns>Доступность перелета</returns>
        private bool IsValidDistance(Planet aSource, Planet aTarget)
        {
            return (aSource == aTarget)
                || (Math.Sqrt(Math.Pow(aTarget.CoordX - aSource.CoordX, 2) + Math.Pow(aTarget.CoordY - aSource.CoordY, 2)) <= Engine.Radius);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Экземпляр планетарки</param>
        public PlanetController(PlanetaryEngine aEngine) : base(aEngine)
        {
            PlanetList = new List<Planet>();
        }

        public override void Start()
        {
            // Инициализируем массив секторов
            fSectorMatrix = new Sector[Engine.SizeX, Engine.SizeY];
            // Загрузим данные планеты
            LoadPlanets();
            // Загрузим ссылки между ними
            LoadLinks();
            // Загрузим порталы
            LoadPortals();
            // Загрузим таймеры
            LoadTimers();
            // Обработаем черные дыры
            Action.WormHole.Reactivate();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        #region Actions

        private CmdClass fCmd;
        public CmdClass Cmd
        {
            get
            {
                if (fCmd == null)
                    fCmd = new CmdClass(Engine);
                return fCmd;
            }
        }

        private ActionClass fAction;
        public ActionClass Action
        {
            get
            {
                if (fAction == null)
                    fAction = new ActionClass(Engine);
                return fAction;
            }
        }

        internal class CmdClass : PlanetaryAccess
        {
            public CmdClass(PlanetaryEngine aEngine) : base(aEngine)
            {
            }

            private CmdShowDetails fShowDetails;
            public CmdShowDetails ShowDetails
            {
                get
                {
                    if (fShowDetails == null)
                        fShowDetails = new CmdShowDetails(Engine);
                    return fShowDetails;
                }
            }

            private CmdSubscribe fSubscribe;
            public CmdSubscribe Subscribe
            {
                get
                {
                    if (fSubscribe == null)
                        fSubscribe = new CmdSubscribe(Engine);
                    return fSubscribe;
                }
            }

            private CmdPortalClose fPortalClose;
            public CmdPortalClose PortalClose
            {
                get
                {
                    if (fPortalClose == null)
                        fPortalClose = new CmdPortalClose(Engine);
                    return fPortalClose;
                }
            }
        }

        internal class ActionClass : PlanetaryAccess
        {
            public ActionClass(PlanetaryEngine aEngine) : base(aEngine)
            {
            }

            private ActionUtils fUtils;
            public ActionUtils Utils
            {
                get
                {
                    if (fUtils == null)
                        fUtils = new ActionUtils(Engine);
                    return fUtils;
                }
            }

            private Capture fCapture;
            public Capture Capture
            {
                get
                {
                    if (fCapture == null)
                        fCapture = new Capture(Engine);
                    return fCapture;
                }
            }

            private Gravitor fGravitor;
            public Gravitor Gravitor
            {
                get
                {
                    if (fGravitor == null)
                        fGravitor = new Gravitor(Engine);
                    return fGravitor;
                }
            }

            private Control fControl;
            public Control Control
            {
                get
                {
                    if (fControl == null)
                        fControl = new Control(Engine);
                    return fControl;
                }
            }

            private Portals fPortal;
            public Portals Portal
            {
                get
                {
                    if (fPortal == null)
                        fPortal = new Portals(Engine);
                    return fPortal;
                }
            }

            private ShipList fShipList;
            public ShipList ShipList
            {
                get
                {
                    if (fShipList == null)
                        fShipList = new ShipList(Engine);
                    return fShipList;
                }
            }

            private Subscribe fSubscribe;
            public Subscribe Subscribe
            {
                get
                {
                    if (fSubscribe == null)
                        fSubscribe = new Subscribe(Engine);
                    return fSubscribe;
                }
            }

            private WormHole fWormHole;
            public WormHole WormHole
            {
                get
                {
                    if (fWormHole == null)
                        fWormHole = new WormHole(Engine);
                    return fWormHole;
                }
            }

            private Pulsar fPulsar;
            public Pulsar Pulsar
            {
                get
                {
                    if (fPulsar == null)
                        fPulsar = new Pulsar(Engine);
                    return fPulsar;
                }
            }


            #endregion
        }
    }
}