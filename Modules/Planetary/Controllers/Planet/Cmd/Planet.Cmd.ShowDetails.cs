/////////////////////////////////////////////////
//
// Команда игрока - запрос отображения деталей планеты
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
    /// Класс запроса отображения деталей планеты
    /// </summary>
    internal class CmdShowDetails : PlanetaryCommand
    {
        private void Call(Planet aPlanet, Player aPlayer)
        {
            Engine.SocketWriter.PlanetStorageResize(aPlanet, true, aPlayer);
            // Отправим данные слотов
            foreach (Storage tmpStorage in aPlanet.StorageZone.Slots)
                Engine.SocketWriter.PlanetStorageUpdate(aPlanet, tmpStorage);
            // Отправим данные строений
            /*for TmpB in APlanet.Buildings do
                                Engine.SocketWriter.PlanetBuildingUpdate(TmpB.Value, APlayer);
            // Отправим количество модулей
            /*Engine.SocketWriter.PlanetModulesUpdate(APlanet, APlayer);*/
            // Отправим количество энергии
            Engine.SocketWriter.PlanetEnergyUpdate(aPlanet, aPlayer);
            // Отправим сообщение что можно показать окно деталей
            Engine.SocketWriter.PlanetDetailsShow(aPlanet, aPlayer);
        }

        /// <summary>
        /// Нельзя увидеть свойства т, пульсаров и других активных планет
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckActivity(Planet aPlanet)
        {
            if (aPlanet.TimerEnabled(PlanetTimer.Activity))
                return Warning("Activity planet");
            else
                return true;
        }

        /// <summary>
        /// Нельзя увидеть свойства невидимой планеты
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckVisibility(Planet aPlanet, Player aPlayer)
        {
            if (!aPlanet.VisibleByPlayer(aPlayer, false))
                return Warning("Invisible planet");
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
                return Warning("Invalid planet");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdShowDetails(PlanetaryEngine aEngine) : base(aEngine)
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
            if (!CheckVisibility(tmpPlanet, aPacket.Connection.Player))
                return;
            if (!CheckActivity(tmpPlanet))
                return;
            // Отправим команду на исполнение
            Call(tmpPlanet, aPacket.Connection.Player);
        }
    }
}