/////////////////////////////////////////////////
//
// Команда игрока - закрытие портала
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс закрытия портала
    /// </summary>
    internal class CmdPortalClose : PlanetaryCommand
    {
        /// <summary>
        /// Проверка разрешения закрытия портала
        /// </summary>
        /// <param name="aPortal">Портал</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPortalClosing(Portal aPortal)
        {
            if (!aPortal.Breakable)
                return Warning("Can't close unbreakable portal");
            else
                return true;
        }

        /// <summary>
        /// Проверка наличия портала
        /// </summary>
        /// <param name="aPortal">Портал</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPortal(Portal aPortal)
        {
            if (aPortal == null)
                return Warning("No portal");
            else
                return true;
        }

        /// <summary>
        /// Проверка роли закрывателя
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Planet aPlanet, Player aPlayer)
        {
            if (aPlanet.Portal.Owner != aPlayer)
                return Warning("Wrong role");
            else
                return true;
        }

        /// <summary>
        /// Проверка планеты назначения
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPlanet(Planet aPlanet)
        {
            if (aPlanet == null)
                return Warning("Invalid source");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdPortalClose(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Planet tmpPlanet = ReadPlanet(aPacket);
            // Основная валидация
            if (!CheckPlanet(tmpPlanet))
                return;
            if (!CheckRole(tmpPlanet, aPacket.Connection.Player))
                return;
            if (!CheckPortal(tmpPlanet.Portal))
                return;
            if (!CheckPortalClosing(tmpPlanet.Portal))
                return;
            // Отправим команду на исполнение
            Engine.Planets.Action.Portal.Close(tmpPlanet, aPacket.Connection.Player);
        }
    }
}