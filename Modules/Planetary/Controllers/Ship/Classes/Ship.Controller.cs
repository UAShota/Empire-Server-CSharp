/////////////////////////////////////////////////
//
// Контроллер корабликов
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.02.29
//
/////////////////////////////////////////////////

using Empire.EngineSpace;
using Empire.Planetary.Classes;
using Empire.Planetary.HangarSpace;
using Empire.Planetary.ShipSpace.Skills;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс контроллера корабликов
    /// </summary>
    internal class ShipController : PlanetaryController
    {
        /// <summary>
        /// Класс команд контроллера кораблей
        /// </summary>
        internal class Commands : PlanetaryAccess
        {
            public Commands(PlanetaryEngine aEngine) : base(aEngine)
            {
            }

            private CmdActivity fActivity;
            public CmdActivity Activity
            {
                get
                {
                    if (fActivity == null)
                        fActivity = new CmdActivity(Engine);
                    return fActivity;
                }
            }

            private CmdConstruction fConstruction;
            public CmdConstruction Construction
            {
                get
                {
                    if (fConstruction == null)
                        fConstruction = new CmdConstruction(Engine);
                    return fConstruction;
                }
            }

            private CmdAttach fAttach;
            public CmdAttach Attach
            {
                get
                {
                    if (fAttach == null)
                        fAttach = new CmdAttach(Engine);
                    return fAttach;
                }
            }

            private CmdDestroy fDestroy;
            public CmdDestroy Destroy
            {
                get
                {
                    if (fDestroy == null)
                        fDestroy = new CmdDestroy(Engine);
                    return fDestroy;
                }
            }

            private CmdMerge fMerge;
            public CmdMerge Merge
            {
                get
                {
                    if (fMerge == null)
                        fMerge = new CmdMerge(Engine);
                    return fMerge;
                }
            }

            private CmdSeparate fSeparate;
            public CmdSeparate Separate
            {
                get
                {
                    if (fSeparate == null)
                        fSeparate = new CmdSeparate(Engine);
                    return fSeparate;
                }
            }

            private CmdDeparture fMoveFromHangar;
            public CmdDeparture ToHangar
            {
                get
                {
                    if (fMoveFromHangar == null)
                        fMoveFromHangar = new CmdDeparture(Engine);
                    return fMoveFromHangar;
                }
            }

            private CmdDeparture fMoveToHangar;
            public CmdDeparture FromHangar
            {
                get
                {
                    if (fMoveToHangar == null)
                        fMoveToHangar = new CmdDeparture(Engine);
                    return fMoveToHangar;
                }
            }

            private CmdAnnihilation fAnnihilation;
            public CmdAnnihilation Annihilation
            {
                get
                {
                    if (fAnnihilation == null)
                        fAnnihilation = new CmdAnnihilation(Engine);
                    return fAnnihilation;
                }
            }

            private CmdConstructor fConstructor;
            public CmdConstructor Constructor
            {
                get
                {
                    if (fConstructor == null)
                        fConstructor = new CmdConstructor(Engine);
                    return fConstructor;
                }
            }

            private CmdHypodispersion fHypodispersion;
            public CmdHypodispersion Hypodispersion
            {
                get
                {
                    if (fHypodispersion == null)
                        fHypodispersion = new CmdHypodispersion(Engine);
                    return fHypodispersion;
                }
            }

            private CmdPortalJump fPortalJump;
            public CmdPortalJump PortalJump
            {
                get
                {
                    if (fPortalJump == null)
                        fPortalJump = new CmdPortalJump(Engine);
                    return fPortalJump;
                }
            }

            private CmdPortalOpen fPortalOpen;
            public CmdPortalOpen PortalOpen
            {
                get
                {
                    if (fPortalOpen == null)
                        fPortalOpen = new CmdPortalOpen(Engine);
                    return fPortalOpen;
                }
            }

            private CmdMove fMove;
            public CmdMove Move
            {
                get
                {
                    if (fMove == null)
                        fMove = new CmdMove(Engine);
                    return fMove;
                }
            }

            private CmdPath fPath;
            public CmdPath Path
            {
                get
                {
                    if (fPath == null)
                        fPath = new CmdPath(Engine);
                    return fPath;
                }
            }
        }

        /// <summary>
        /// Класс действий контроллера кораблей
        /// </summary>
        internal class Actions : PlanetaryAccess
        {
            public Actions(PlanetaryEngine aEngine) : base(aEngine)
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

            private ActionTravel fTravel;
            public ActionTravel Travel
            {
                get
                {
                    if (fTravel == null)
                        fTravel = new ActionTravel(Engine);
                    return fTravel;
                }
            }



            private ActionAttach fAttach;
            public ActionAttach Attach
            {
                get
                {
                    if (fAttach == null)
                        fAttach = new ActionAttach(Engine);
                    return fAttach;
                }
            }

            private ActionFuel fFuel;
            public ActionFuel Fuel
            {
                get
                {
                    if (fFuel == null)
                        fFuel = new ActionFuel(Engine);
                    return fFuel;
                }
            }

            private ActionBattle fBattle;
            public ActionBattle Battle
            {
                get
                {
                    if (fBattle == null)
                        fBattle = new ActionBattle(Engine);
                    return fBattle;
                }
            }

            private ActionConstruction fConstruction;
            public ActionConstruction Construction
            {
                get
                {
                    if (fConstruction == null)
                        fConstruction = new ActionConstruction(Engine);
                    return fConstruction;
                }
            }


            private ActionFly fFly;
            public ActionFly Fly
            {
                get
                {
                    if (fFly == null)
                        fFly = new ActionFly(Engine);
                    return fFly;
                }
            }

            private ActionHypodispersion fHypodispersion;
            public ActionHypodispersion Hypodispersion
            {
                get
                {
                    if (fHypodispersion == null)
                        fHypodispersion = new ActionHypodispersion(Engine);
                    return fHypodispersion;
                }
            }

            private ActionPortaling fPortaling;
            public ActionPortaling Portaling
            {
                get
                {
                    if (fPortaling == null)
                        fPortaling = new ActionPortaling(Engine);
                    return fPortaling;
                }
            }

            private ActionRelocation fRelocation;
            public ActionRelocation Relocation
            {
                get
                {
                    if (fRelocation == null)
                        fRelocation = new ActionRelocation(Engine);
                    return fRelocation;
                }
            }

            private ActionRepair fRepair;
            public ActionRepair Repair
            {
                get
                {
                    if (fRepair == null)
                        fRepair = new ActionRepair(Engine);
                    return fRepair;
                }
            }

            private ActionStandDown fStandDown;
            public ActionStandDown StandDown
            {
                get
                {
                    if (fStandDown == null)
                        fStandDown = new ActionStandDown(Engine);
                    return fStandDown;
                }
            }

            private ActionStandUp fStandUp;
            public ActionStandUp StandUp
            {
                get
                {
                    if (fStandUp == null)
                        fStandUp = new ActionStandUp(Engine);
                    return fStandUp;
                }
            }

            private ActionTargetLocal fTargetLocal;
            public ActionTargetLocal TargetLocal
            {
                get
                {
                    if (fTargetLocal == null)
                        fTargetLocal = new ActionTargetLocal(Engine);
                    return fTargetLocal;
                }
            }

            private ActionTargetRange fTargetMarker;
            public ActionTargetRange TargetMarker
            {
                get
                {
                    if (fTargetMarker == null)
                        fTargetMarker = new ActionTargetRange(Engine);
                    return fTargetMarker;
                }
            }

            private ActionPath fPath;
            public ActionPath Path
            {
                get
                {
                    if (fPath == null)
                        fPath = new ActionPath(Engine);
                    return fPath;
                }
            }
        }

        /// <summary>
        /// Команды игрока
        /// </summary>
        public Commands Cmd { get; private set; }

        /// <summary>
        /// Действия кораблей
        /// </summary>
        public Actions Action { get; private set; }

        /// <summary>
        /// Максимальное количество кораблей одного игрока на орбите
        /// </summary>
        public int MaxShipCount => 6;

        /// <summary>
        /// Максимальное количество активных кораблей одного игрока на орбите
        /// </summary>
        public int MaxShipActive => MaxShipCount - 1;

        /// <summary>
        /// Загрузка кораблей планетарной системы
        /// </summary>
        private void LoadShips()
        {
            using (var tmpReader = Core.Database.Query("PLLoadShips", Engine.Player.ID))
            {
                while (tmpReader.Read())
                {
                    // Создадим кораблик
                    Ship tmpShip = Action.Utils.CreateShip(
                        (ShipType)tmpReader.ReadInt("ID_TYPE"),
                        tmpReader.ReadInt("COUNT"),
                        Core.Server.Auth.FindPlayer(tmpReader.ReadInt("ID_OWNER"), true)
                    );
                    tmpShip.Mode = (ShipMode)tmpReader.ReadInt("MODE");
                    tmpShip.HP = tmpReader.ReadInt("HP");
                    // Добавим на планету                    
                    Planet tmpPlanet = Engine.Planets.PlanetList[tmpReader.ReadInt("ID_PLANET")];
                    Action.Relocation.Add(tmpShip, tmpPlanet.LandingZone[tmpReader.ReadInt("ID_SLOT")], true, false);
                    // Пропишем аттач
                    int tmpAttachID = tmpReader.ReadInt("ID_PLANET_ATTACH");
                    if (tmpAttachID > 0)
                    {
                        Planet tmpAttachPlanet = Engine.Planets.PlanetList[tmpAttachID];
                        Action.Attach.Call(tmpShip, tmpAttachPlanet, false);
                    }
                }
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Экземпляр планетарки</param>
        public ShipController(PlanetaryEngine aEngine) : base(aEngine)
        {
            Action = new Actions(aEngine);
            Cmd = new Commands(aEngine);
        }

        public override void Start()
        {
            LoadShips();
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}