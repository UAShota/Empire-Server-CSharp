/////////////////////////////////////////////////
//
// Движок планетарной обработки
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Planetary.HangarSpace;
using Empire.Planetary.PlanetSpace;
using Empire.Planetary.Protocol;
using Empire.Planetary.ShipSpace;
using Empire.Sockets;

namespace Empire.Planetary
{
    /// <summary>
    /// Класс пользовательской команды
    /// </summary>
    internal abstract class PlanetaryCommand : PlanetaryAccess
    {
        /// <summary>
        /// Чтение планеты по ее идентификатору
        /// </summary>
        /// <returns>Планета по ее идентификатору</returns>
        protected Planet ReadPlanet(SocketPacket aPacket, int AUID = -1)
        {
            if (AUID == -1)
                AUID = aPacket.ReadInt();
            if ((AUID >= Engine.Planets.PlanetList.Count) || (AUID < 0))
                return null;
            else
                return Engine.Planets.PlanetList[AUID];
        }

        /// <summary>
        /// Чтение корабля по ее идентификатору
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        /// <returns>Корабль по ее идентификатору</returns>
        protected Ship ReadShip(SocketPacket aPacket)
        {
            int tmpID = aPacket.ReadInt();
            Planet tmpPlanet = ReadPlanet(aPacket, tmpID >> 16);
            if ((tmpPlanet != null) && (tmpPlanet.LandingZone.TryGet(tmpID & 0xFFFF, out Landing tmpLanding)))
                return tmpLanding.Ship;
            else
                return null;
        }

        /// <summary>
        /// Уведомление о недопустимости операции
        /// </summary>
        /// <param name="aMessage">Сообщение уведомления</param>
        /// <returns>Отмена операции</returns>
        protected bool Warning(string aMessage)
        {
            Core.Log.Warn(aMessage);
            return false;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Контроллер команд</param>
        public PlanetaryCommand(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public abstract void Process(SocketPacket aPacket);
    }

    /// <summary>
    /// Базовый класс контроллера
    /// </summary>
    internal abstract class PlanetaryController : PlanetaryAccess
    {
        /// <summary>
        /// Запуск
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Остановка
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Контроллер команд</param>
        public PlanetaryController(PlanetaryEngine aEngine) : base(aEngine)
        {
        }
    }

    /// <summary>
    /// Базовый класс доступа
    /// </summary>
    internal abstract class PlanetaryAccess
    {
        /// <summary>
        /// Доступ к движку
        /// </summary>
        protected PlanetaryEngine Engine { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Контроллер команд</param>
        public PlanetaryAccess(PlanetaryEngine aEngine)
        {
            Engine = aEngine;
        }
    }

    /// <summary>
    /// Класс планетарной обработки
    /// </summary>
    internal class PlanetaryEngine : IDisposable
    {
        /// <summary>
        /// Поток выполнения планетарки
        /// </summary>
        private Task fTask { get; set; }

        /// <summary>
        /// Сокет чтения
        /// </summary>
        private ProtocolReader fSocketReader { get; set; }

        /// <summary>
        /// Сокет записи
        /// </summary>
        public ProtocolWriter SocketWriter { get; private set; }

        /// <summary>
        /// Контроллер планет
        /// </summary>
        public PlanetController Planets { get; private set; }

        /// <summary>
        /// Контроллер корабликов
        /// </summary>
        public ShipController Ships { get; private set; }

        /// <summary>
        /// Контроллер ангара
        /// </summary>
        public HangarController Hangar { get; private set; }

        /// <summary>
        /// Контроллер таймеров
        /// </summary>
        public Timers Timers { get; private set; }

        /// <summary>
        /// Владелец планетарки
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Размер созвездия по вертикали
        /// </summary>
        public int SizeX { get; private set; }

        /// <summary>
        /// Размер созвездия по горизонтали
        /// </summary>
        public int SizeY { get; private set; }

        /// <summary>
        /// Расстояние перелета между планетами
        /// </summary>
        public int Radius { get; private set; }

        /// <summary>
        /// Время активности пульсара
        /// </summary>
        public int TimePulsarActive { get; private set; }

        /// <summary>
        /// Время активности ЧТ
        /// </summary>
        public int TimeWormholeActive { get; private set; }

        /// <summary>
        /// Время открытия ЧТ
        /// </summary>
        public int TimeWormholeOpen { get; private set; }

        /// <summary>
        /// Возвращение загруженного созвездия
        /// </summary>
        public bool Available { get; private set; }

        public int MannedCount = 0;/**/

        /// <summary>
        /// Загрузка параметров созвездия
        /// </summary>
        /// <returns>Доступность созвездия</returns>
        private bool LoadParams()
        {
            using (var tmpReader = Core.Database.Query("PLLoadPlanetar", Player.ID))
            {
                if (tmpReader.Read())
                {
                    SizeX = tmpReader.ReadInt("WIDTH");
                    SizeY = tmpReader.ReadInt("HEIGHT");
                    Radius = tmpReader.ReadInt("RADIUS");
                    TimeWormholeOpen = 1000;// tmpReader.ReadInt("WORMHOLE_TIME_OPEN");
                    TimeWormholeActive = 25000;// tmpReader.ReadInt("WORMHOLE_TIME_ACTIVE");
                    TimePulsarActive = 25000;// tmpReader.ReadInt("PULSAR_TIME_ACTIVE");
                    return true;
                }
                else
                {
                    Core.Log.Warn("Unknown planetar {0}", Player.ID);
                    return false;
                }
            }
        }

        /// <summary>
        /// Поточная обработка планетарки
        /// </summary>
        private void Execute()
        {
            // Загрузка параметров созвездия
            if (!LoadParams())
                return;
            // Контроллеры
            Planets.Start();
            Ships.Start();
            Hangar.Start();
            // Установим флаг доступности
            Available = true;
            // Уведомим о загрузке созвздия
            if (!Player.IsBot)
                SocketWriter.PlanetStarted(Player);
            // Время на обработку - 50мсек
            Stopwatch tmpWatch = new Stopwatch();
            // Запускаем поток обработки
            while (!fTask.IsCanceled)
            {
                // Перезапустим таймер
                tmpWatch.Restart();
                // Обработаем
                fSocketReader.Work();
                Timers.Work();
                // Обновим червоточины
                Planets.Action.WormHole.Reactivate();
                // Прокрутим оставшееся время
                while (tmpWatch.ElapsedMilliseconds < Timers.TimeDelta)
                    Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aPlayer">Владелец планетарки</param>
        public PlanetaryEngine(Player aPlayer)
        {
            Player = aPlayer;
            // Контроллеры
            Timers = new Timers();
            Planets = new PlanetController(this);
            Ships = new ShipController(this);
            Hangar = new HangarController(this);
            // Сокеты
            SocketWriter = new ProtocolWriter(this);
            fSocketReader = new ProtocolReader(this);
            // Запустим
            fTask = Core.StartTask(Execute);
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {
            fTask.Wait();
            fSocketReader.Dispose();
            SocketWriter.Dispose();
        }

        /// <summary>
        /// Обработка команды игрока
        /// </summary>
        /// <param name="aBuffer">Буфер операции</param>
        /// <returns>Успешность операции</returns>
        public bool Command(SocketPacket aBuffer)
        {
            if (Available)
            {
                fSocketReader.Command(aBuffer);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Подписка игрока на созвездие
        /// </summary>
        /// <param name="aPlayer">Игрок</param>
        public void Subscribe(Player aPlayer)
        {
            SocketWriter.PlanetLoadBegin(aPlayer);
            SocketWriter.PlanetLoadPlanets(aPlayer);
            SocketWriter.PlanetLoadHangar(aPlayer);
            SocketWriter.PlayerLoadStorage(aPlayer);
            SocketWriter.PlayerInfoUpdate(aPlayer);
            SocketWriter.PlayerTechWarShipLoad(aPlayer);
            SocketWriter.PlanetLoadEnd(aPlayer);
        }

        /// <summary>
        /// Отключение от системы
        /// </summary>
        /// <param name="aPlayer">Игрок отключения</param>
        public void Unsubscribe(Player aPlayer)
        {
            /**/
            aPlayer.Stop();
        }
    }
}