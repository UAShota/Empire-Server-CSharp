/////////////////////////////////////////////////
//
// Модуль авторизации
//
// Copyright(c) 2016 UAShota
//
// Rev I  2020.05.01
//
/////////////////////////////////////////////////

using System.Collections.Concurrent;
using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Sockets;

namespace Empire.Modules
{
    /// <summary>
    /// Управление авторизацией
    /// </summary>
    internal class AuthorizationModule : CustomModule
    {
        /// <summary>
        /// Список ошибок
        /// </summary>
        private enum Errors
        {
            /// <summary>
            /// Неверно указан логин или пароль
            /// </summary>
            InvalidCredentials = 0x0001
        }

        /// <summary>
        /// Команды авторизации
        /// </summary>
        private enum Commands
        {
            /// <summary>
            /// Сообщение чата
            /// </summary>
            ChatMessage = 0x0001,
            /// <summary>
            /// Запрос авторизации
            /// </summary>
            LoginAuth = 0x0002,
            /// <summary>
            /// Авторизация не пройдена
            /// </summary>
            LoginFailed = 0x0003,
            /// <summary>
            /// Авторизация успешна
            /// </summary>
            LoginAccept = 0x0004,
            /// <summary>
            /// Повторное подключение
            /// </summary>
            LoginRelog = 0x0005
        }

        /// <summary>
        /// Последний номер обработанного игрока
        /// </summary>
        private int fLastPlayerUID { get; set; }

        /// <summary>
        /// Последний обработанный игрок
        /// </summary>
        private Player fLastPlayer { get; set; }

        /// <summary>
        /// Кэш клиентов
        /// </summary>
        private ConcurrentDictionary<int, Player> fClients { get; set; }

        /// <summary>
        /// Уведомление об успешном логине
        /// </summary>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aConnection">Соединение игрока</param>
        private void ResponseLoginAccept(Player aPlayer, SocketConnection aConnection)
        {
            SocketPacket tmpBuffer;
            // Отсоединим предыдущего игрока
            if (aPlayer.Connection != null)
            {
                tmpBuffer = new SocketPacket(Commands.LoginRelog);
                aPlayer.Connection.Push(tmpBuffer);
                Core.Server.Kick(aPlayer);
            }
            // 
            // Установим соединение клиенту
            aPlayer.Accept(aConnection);
            // Отправим сообщение об успешной авторизации для кэша пароля
            tmpBuffer = new SocketPacket(Commands.LoginAccept);
            tmpBuffer.WriteString(aPlayer.Password);
            tmpBuffer.WriteInt(aPlayer.ID);
            tmpBuffer.WriteInt(aPlayer.Race);
            aConnection.Push(tmpBuffer);
            // Залогируем
            Core.Log.Info("Accepted {0} from {1}", aPlayer.Name, aPlayer.Connection.IP);
        }

        /// <summary>
        /// Уведомление о неудачном логине
        /// </summary>
        /// <param name="aConnection">Соединение игрока</param>
        private void ResponseLoginFailed(SocketConnection aConnection)
        {
            // Отправим сообщение об ошибке авторизации
            SocketPacket tmpBuffer = new SocketPacket(Commands.LoginFailed);
            tmpBuffer.WriteInt(Errors.InvalidCredentials);
            aConnection.Push(tmpBuffer);
        }

        /// <summary>
        /// Загрузка игрока из базы
        /// </summary>
        /// <param name="aUID">Идентификатор клиента</param>
        /// <returns>Загруженный с базы игрок</returns>
        private Player LoadPlayer(int aUID)
        {
            // Загрузим профиль игрока
            using (var tmpReader = Core.Database.Query("SHLoadProfile", aUID))
            {
                // Проверим наличие загруженного профиля
                if (!tmpReader.Read())
                {
                    Core.Log.Error("Profile {0} not found", aUID);
                    return null;
                }
                // Сохраним параметры игрока                
                Player tmpPlayer = new Player
                {
                    ID = aUID,
                    Name = tmpReader.ReadString("LOGIN"),
                    Quasi = tmpReader.ReadInt("MONEY_GOLD"),
                    Credits = tmpReader.ReadInt("MONEY_CREDITS"),
                    Fuel = tmpReader.ReadInt("MONEY_FUEL"),
                    Race = (PlayerRace)tmpReader.ReadInt("ID_RACE"),
                    Password = tmpReader.ReadString("PWD_HASH")
                };
                // Загрузим технологии игрока
                tmpPlayer.LoadHolding(tmpReader.ReadInt("STORAGE_SIZE"));
                tmpPlayer.Load();
                // Добавим его в словарь
                fClients.GetOrAdd(aUID, tmpPlayer);
                // Вернем игрока
                return tmpPlayer;
            }
        }

        /// <summary>
        /// Авторизация клиента
        /// </summary>
        /// <param name="aPacket">Буфер команды</param>
        private bool CmdLogin(SocketPacket aPacket)
        {
            // Получим сведения после соединения клиента
            string tmpLogin = aPacket.ReadString();
            string tmpPassword = aPacket.ReadString();
            // Запросим корректность авторизации
            using (var tmpReader = Core.Database.Query("SHLoadPlayer", tmpLogin, tmpPassword))
            {
                // Проверим наличие игрока с такими параметрами
                if (tmpReader.Read())
                {
                    // Попробуем загрузить его профиль
                    int tmpPlayerID = tmpReader.ReadInt("UID");
                    var tmpPlayer = FindPlayer(tmpPlayerID, true);
                    // Если нашли - одобрим авторизацию
                    if (tmpPlayer != null)
                    {
                        ResponseLoginAccept(tmpPlayer, aPacket.Connection);
                        return true;
                    }
                }
                ResponseLoginFailed(aPacket.Connection);
                // Залогируем
                Core.Log.Info("Login {0}:{1} failed from {2}", tmpLogin, tmpPassword, aPacket.Connection.IP);
            }
            return true;
        }

        /// <summary>
        /// Запуск модуля
        /// </summary>
        protected override void DoStart()
        {
            fClients = new ConcurrentDictionary<int, Player>();
        }

        /// <summary>
        /// Остановка модуля
        /// </summary>
        protected override void DoStop()
        {
            fClients = null;
        }

        /// <summary>
        /// Обработчик потока
        /// </summary>
        /// <param name="aPacket">Буфер данных</param>
        protected override bool DoExecute(SocketPacket aPacket)
        {
            // Определим команду
            Commands tmpCmd = (Commands)aPacket.ReadCommand();
            // Определим команду
            switch (tmpCmd)
            {
                case Commands.LoginAuth:
                    return CmdLogin(aPacket);
                default:
                    Core.Log.Error("Unknown command 0x{0:X}", tmpCmd);
                    return true;
            }
        }

        /// <summary>
        /// Поиск и загрузка игрока по его идентификатору
        /// </summary>
        /// <param name="aUID">Идентификатор клиента</param>
        /// <param name="aLoad">Необходимость загрузки клиента с базы</param>
        /// <returns>Найденный или загруженный игрок</returns>
        public Player FindPlayer(int aUID, bool aLoad)
        {
            // При повторном запросе того-же персонажа, вернем данные с кеша
            if (fLastPlayerUID == aUID)
                return fLastPlayer;
            // Иначе поищем в списке 
            if (fClients.TryGetValue(aUID, out Player tmpPlayer))
                return tmpPlayer;
            // Или загрузим с базы
            if (aLoad)
                tmpPlayer = LoadPlayer(aUID);
            // Если нету - залогируем
            if (tmpPlayer == null)
                Core.Log.Info("Invalid player {0}", aUID);
            // Сохраним в кэше
            else
            {
                fLastPlayer = tmpPlayer;
                fLastPlayerUID = aUID;
            }
            // Вернем
            return fLastPlayer;
        }
    }
}