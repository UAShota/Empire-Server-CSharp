/////////////////////////////////////////////////
//
// Модуль обработки планетарок
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary;
using Empire.Sockets;

namespace Empire.Modules
{
    /// <summary>
    /// Класс планетарной обработки
    /// </summary>
    internal class PlanetaryModule : CustomModule
    {
        /// <summary>
        /// Ошибки
        /// </summary>
        private enum Errors
        {
            /// <summary>
            /// Ошибок нет
            /// </summary>
            Success = 0,

            /// <summary>
            /// Система не загружена
            /// </summary>
            SystemUnloaded = 0x1F01,

            /// <summary>
            /// Система не активна
            /// </summary>
            SystemUnavailable = 0x1F02
        }

        /// <summary>
        /// Комманды
        /// </summary>
        private enum Commands
        {
            /// <summary>
            /// Aктивность созвездия
            /// </summary>
            Available = 0x1F01,

            /// <summary>
            /// Подписка на созвездие
            /// </summary>
            Subscribe = 0x1F02
        }

        /// <summary>
        /// Словарь технологий
        /// </summary>
        public PlanetaryDictionary Dictionary { get; set; }

        /// <summary>
        /// Поиск игрока с указанным идентификатором
        /// </summary>
        /// <param name="aBuffer">Буфер клиента</param>
        /// <param name="aPlanetID">Идентификатор планетарки</param>
        /// <param name="aError">Код ошибки</param>
        /// <returns>Экземпляр игрока</returns>
        private Player FindPlayer(SocketPacket aBuffer, out int aPlanetID, out Errors aError)
        {
            // Считаем код планетарки
            aPlanetID = aBuffer.ReadInt();
            // Попробуем найти игрока для этой планетарки
            Player tmpPlayer = Core.Server.Auth.FindPlayer(aPlanetID, false);
            if (tmpPlayer == null)
            {
                aError = Errors.SystemUnloaded;
                return null;
            }
            // Определим доступность
            else if (!tmpPlayer.Planetary.Available())
            {
                aError = Errors.SystemUnavailable;
                return null;
            }
            // Все хорошо
            else
            {
                aError = Errors.Success;
                return tmpPlayer;
            }
        }

        /// <summary>
        /// Отправка сообщения о доступности созвездия
        /// </summary>
        /// <param name="aConnection">Соединение клиента</param>
        /// <param name="aPlanetID">Идентификатор планетарки</param>
        /// <param name="aError">Код ошибки</param>
        private void SendAvailable(SocketConnection aConnection, int aPlanetID, Errors aError)
        {
            SocketPacket tmpBuffer = new SocketPacket(Commands.Available);
            tmpBuffer.WriteInt(aPlanetID);
            tmpBuffer.WriteInt(aError);
            // Отправим ответ
            aConnection.Push(tmpBuffer);
        }

        /// <summary>
        /// Определение доступности планетарки
        /// </summary>
        /// <param name="aBuffer">Буфер клиента</param>
        /// <returns>Успешность обработки команды</returns>
        private bool CheckAvailable(SocketPacket aBuffer)
        {
            // Поищем профиль
            FindPlayer(aBuffer, out int tmpPlanetID, out Errors tmpError);
            // Соберем ответ
            SendAvailable(aBuffer.Connection, tmpPlanetID, tmpError);
            // Разрешим прибить буфер
            return true;
        }

        /// <summary>
        /// Подключение к планетарке
        /// </summary>
        /// <param name="aBuffer">Буфер команды</param>
        /// <returns>Запрет на уничтожение буфера</returns>
        private bool Subscribe(SocketPacket aBuffer)
        {
            // Поищем профиль к которому нужно подключиться
            Player tmpPlayer = FindPlayer(aBuffer, out int tmpPlanetID, out Errors tmpError);
            // Если профиля нет или подписка не удалась, вернем ответ
            if ((tmpError != Errors.Success) || (!tmpPlayer.Planetary.Subscribe(aBuffer)))
            {
                SendAvailable(aBuffer.Connection, tmpPlanetID, tmpError);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Пересылка команды в созвездие
        /// </summary>
        /// <param name="aBuffer">Буфер данных</param>
        /// <returns>Запрет на уничтожение буфера</returns>
        private bool Retranslate(SocketPacket aBuffer)
        {
            return aBuffer.Connection.Player.Planetary.ActiveEngine.Command(aBuffer);
        }

        /// <summary>
        /// Обработчик потока
        /// </summary>
        /// <param name="aBuffer">Буфер команды</param>
        protected override bool DoExecute(SocketPacket aBuffer)
        {
            // Определим команду
            Commands tmpCmd = (Commands)aBuffer.ReadCommand();

            // Определим команду
            switch (tmpCmd)
            {
                case Commands.Available:
                    return CheckAvailable(aBuffer);
                case Commands.Subscribe:
                    return Subscribe(aBuffer);
                default:
                    return Retranslate(aBuffer);
            }
        }

        /// <summary>
        /// Запуск модуля
        /// </summary>
        protected override void DoStart()
        {
            Core.StartAction("PlanetaryDictionary", () => Dictionary = new PlanetaryDictionary());
        }

        /// <summary>
        /// Остановка модуля
        /// </summary>
        protected override void DoStop()
        {
            Dictionary.Dispose();
        }
    }
}