/////////////////////////////////////////////////
//
// Контроллер ангара
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

namespace Empire.Planetary.HangarSpace
{
    /// <summary>
    /// Класс контроллера корабликов
    /// </summary>
    internal class HangarController : PlanetaryController
    {
        /// <summary>
        /// Класс команд контроллера ангара
        /// </summary>
        internal class Commands : PlanetaryAccess
        {
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="aEngine">Экземпляр планетарки</param>
            public Commands(PlanetaryEngine aEngine) : base(aEngine)
            {
            }

            /// <summary>
            /// Отправка в ангар
            /// </summary>
            public CmdArrival Arrival
            {
                get
                {
                    if (fArrival == null)
                        fArrival = new CmdArrival(Engine);
                    return fArrival;
                }
            }
            private CmdArrival fArrival;

            /// <summary>
            /// Высадка с ангара
            /// </summary>
            public CmdDeparture Departure
            {
                get
                {
                    if (fDeparture == null)
                        fDeparture = new CmdDeparture(Engine);
                    return fDeparture;
                }
            }
            private CmdDeparture fDeparture;

            /// <summary>
            /// Обмен слотов
            /// </summary>
            public CmdSwap Swap
            {
                get
                {
                    if (fSwap == null)
                        fSwap = new CmdSwap(Engine);
                    return fSwap;
                }
            }
            private CmdSwap fSwap;
        }

        /// <summary>
        /// Класс действий контроллера ангара
        /// </summary>
        internal class Actions : PlanetaryAccess
        {
            public Actions(PlanetaryEngine aEngine) : base(aEngine)
            {
            }

            private ActionArrival fArrival;
            public ActionArrival Arrival
            {
                get
                {
                    if (fArrival == null)
                        fArrival = new ActionArrival(Engine);
                    return fArrival;
                }
            }
        }

        /// <summary>
        /// Команды игрока
        /// </summary>
        public Commands Cmd { get; private set; }

        /// <summary>
        /// Действия ангара
        /// </summary>
        public Actions Action { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Экземпляр планетарки</param>
        public HangarController(PlanetaryEngine aEngine) : base(aEngine)
        {
            Action = new Actions(aEngine);
            Cmd = new Commands(aEngine);
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}