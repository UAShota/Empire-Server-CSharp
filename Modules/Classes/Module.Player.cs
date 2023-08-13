/////////////////////////////////////////////////
//
// Класс описания игрока
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using Empire.EngineSpace;
using Empire.Planetary;
using Empire.Players.Classes;
using Empire.Sockets;

namespace Empire.Modules.Classes
{
    /// <summary>
    /// Типы игровых рас
    /// </summary>
    internal enum PlayerRace
    {
        /// <summary>
        /// Заглушка
        /// </summary>
        Empty,
        /// <summary>
        /// Люди
        /// </summary>
        Human,
        /// <summary>
        /// Малоки
        /// </summary>
        Maloc,
        /// <summary>
        /// Пеленги
        /// </summary>
        Peleng,
        /// <summary>
        /// Гаальцы
        /// </summary>
        Gaal,
        /// <summary>
        /// Фэяне
        /// </summary>
        Feyan,
        /// <summary>
        /// Клисане
        /// </summary>
        Klisan
    }

    /// <summary>
    /// Роли игроков относительно друг-друга
    /// </summary>
    internal enum PlayerRole
    {
        /// <summary>
        /// Свой
        /// </summary>
        Self,
        /// <summary>
        /// Враг
        /// </summary>
        Enemy,
        /// <summary>
        /// Союзник
        /// </summary>
        Friends,
        /// <summary>
        /// Нейтральный
        /// </summary>
        Neutral
    }

    /// <summary>
    /// Описание игрока
    /// </summary>
    internal class Player
    {
        /// <summary>
        /// Признак активности модулей игрока
        /// </summary>
        private bool fActive { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Имя игрока
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Хеш пароля
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Количество кредитов
        /// </summary>
        public int Credits { get; set; }

        /// <summary>
        /// Количество доната
        /// </summary>
        public int Quasi { get; set; }

        /// <summary>
        /// Количество топлива
        /// </summary>
        public int Fuel { get; set; }

        /// <summary>
        /// Признак бота
        /// </summary>
        public bool IsBot { get; set; }

        /// <summary>
        /// Раса игрока
        /// </summary>
        public PlayerRace Race;

        /// <summary>
        /// Хранилище игрока
        /// </summary>
        public HoldingZone HoldingZone { get; private set; }

        /// <summary>
        /// Информация о соединении
        /// </summary>
        public SocketConnection Connection { get; private set; }

        /// <summary>
        /// Планетарный профиль
        /// </summary>
        public PlanetaryProfile Planetary;

        /// <summary>
        /// Галактический профиль
        /// </summary>
        public GalaxyProfile Galaxy;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Player()
        {
            Planetary = new PlanetaryProfile(this);
            Galaxy = new GalaxyProfile(this);
            HoldingZone = new HoldingZone();
        }

        public void LoadHolding(int aSize)
        {
            HoldingZone.Resize(aSize);
            using (var tmpReader = Core.Database.Query("SHLoadHolding", ID))
            {
                while (tmpReader.Read())
                {
                    Holding tmpHolding = HoldingZone.Slots[tmpReader.ReadInt("POSITION")];
                    tmpHolding.Change(tmpReader.ReadInt("COUNT"), (ResourceType)tmpReader.ReadInt("ID_ITEM"));
                }
            }
        }

        /// <summary>
        /// Запуск модулей игрока
        /// </summary>
        public void Accept(SocketConnection aConnection)
        {
            Connection = aConnection.AcceptPlayer(this);

            if (!fActive)
            {
                Planetary.Start();
                fActive = true;
            }
            else
                fActive = true;
            /*        else
                        Planet->Subscribe(UID);*/
        }

        /// <summary>
        /// Остановка модулей игрока
        /// </summary>
        public void Stop()
        {
            if (fActive)
            {
                /*Planet->Disconnect();*/
                Planetary.Stop();
                /*Galaxy->Stop();*/
                fActive = false;
            }
        }

        /**/
        public void Decline()
        {
            /*        Connection = null; */
        }

        /// <summary>
        /// Загрузка параметров игрока в модулях
        /// </summary>
        public void Load()
        {
            Planetary.Load();
        }

        /// <summary>
        /// Роль двух игроков по отношению друг к другу с учетом сокрытия противника
        /// </summary>
        /// <param name="aVersus">Сопоставляемый игрок</param>
        /// <param name="aHideEnemy">Необходимость скрыть противника</param>
        /// <returns>Роль сопоставляемого игрока</returns>
        public PlayerRole Role(Player aVersus, bool aHideEnemy = false)
        {
            PlayerRole tmpRole;
            // Нейтрал всегда 1
            if ((ID == 1) || (aVersus.ID == 1))
                tmpRole = PlayerRole.Neutral;
            // Равен - значит свой
            else if (this == aVersus)
                tmpRole = PlayerRole.Self;
            // Иначе враг
            else
                tmpRole = PlayerRole.Enemy;
            // Прячем врагов под нейтралов если нет прав на идентификацию
            if ((aHideEnemy) && (tmpRole == PlayerRole.Enemy))
                tmpRole = PlayerRole.Neutral;
            // Вернем
            return tmpRole;
        }

        /// <summary>
        /// Быстрое получение роли союзника
        /// </summary>
        /// <param name="aVersus">Сопоставляемый игрок</param>
        /// <returns>Признак союзника</returns>
        public bool IsRoleFriend(Player aVersus)
        {
            PlayerRole tmpRole = Role(aVersus);
            return (tmpRole == PlayerRole.Self) || (tmpRole == PlayerRole.Friends);
        }

        // Отправка команды неуправляемый буфер
        /*void Send(transport::TSocketBuffer* aBuffer, bool autoCommit = true);
        // Отправка команды управляемый буфер
        void Send(transport::TSocketBuffer & aBuffer);*/
    }
}