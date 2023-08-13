/////////////////////////////////////////////////
//
// Базовый класс для планетарных протоколов
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2020.02.29
//
/////////////////////////////////////////////////

using System;
using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Players.Classes;
using Empire.Sockets;

namespace Empire.Planetary.Protocol
{
    /// <summary>
    /// Отправка команд клиенту
    /// </summary>
    internal class ProtocolWriter : CustomProtocol
    {
        /// <summary>
        /// Доступные команды
        /// </summary>
        public enum Commands
        {
            // Команда оповещение о загрузке созвездия
            PlanetStarted = 0x1000, /*непонятно пока*/

            // Команда запуска загрузки созвездия клиенту
            PlanetLoadBegin = 0x1001,
            // Команда перемещения корабля
            ShipFlyTo = 0x1002,
            // Команда создания корабля
            ShipCreate = 0x1003,
            // Команда удаления корабля
            ShipDelete = 0x1005,
            // Команда обновления таймера планеты
            PlanetUpdateTimer = 0x1006,
            // Команда обновления HP корабля
            ShipUpdateHP = 0x1007,
            // Команда смены аттача корабля
            ShipChangeAttach = 0x1008,
            // Команда смены цели корабля
            ShipChangeTarget = 0x1009,
            // Команда обновления состояния корабля
            ShipUpdateState = 0x100A,
            // Команда открытия портала
            PlanetPortalOpen = 0x100B,
            // Команда настройки хранилища игрока
            PlayerStorageResize = 0x100C,
            // Команда обновления данных панели флота
            Player_HANGAR_UPDATE = 0x100D,
            // Команда показа окна деталей планеты
            Planet_DETAILS_SHOW = 0x100E,
            /// <summary>
            /// Команда обновления данных хранилища планеты
            /// </summary>
            PlanetStorageUpdate = 0x100F,
            /// <summary>
            /// Покупка слота ангара
            /// </summary>
            HangarBuy = 0x1010,
            // Команда очистки слота хранилища планеты
            Planet_STORAGE_CLEAR = 0x1011,
            // Команда обновления данных хранилища игрока
            PlayerStorageUpdate = 0x1012,
            // Команда загрузки технологий корабликов
            PlayerTechWarshipCreate = 0x1013,
            // Команда обновления технологий корабликов
            Player_TECH_WARShip_UPDATE = 0x1014,
            // Команда завершения загрузки созвездия клиенту
            SystemLoadComplete = 0x1015,
            // Команда загрузки технологий строений
            Player_TECH_BUILDING_CREATE = 0x1016,
            // Команда обновления данных строения
            Planet_BUILDING_UPDATE = 0x1017,
            // Команда обновления технологий строений
            Player_TECH_BUILDING_UPDATE = 0x1018,
            // Обновление данных пользователя
            PlayerInfoUpdate = 0x1019,
            // Команда загрузки данных строений
            Connection_BUILDINGS_LOAD = 0x101A,
            // Команда загрузки данных корабликов
            Connection_WARShipS_LOAD = 0x101B,
            // Команда обновления состояние планетоида
            PlanetStateUpdate = 0x101C,
            // Команда установки времени таймера переключения состояния
            _free_101d = 0x101D,
            // Команда обновления уровня видимости планетоида
            PlanetVisibilityUpdate = 0x101E,
            // Команда смены состояния подписки на планетоид
            PlanetSubscriptionChanged = 0x101F,
            // Команда смены владельца планетоида
            PlanetOwnerChanged = 0x1020,
            // Команда обновления типа покрытия планетоида
            PlanetCoverageUpdate = 0x1021,
            // Команда смены направления торгового пути
            Planet_TRADEPATH_UPDATE = 0x1022,
            // Команда закрытия портала
            PlanetPortalClose = 0x1023,
            // Команда обновления наличия электроэнергии
            Planet_ELECTRO_UPDATE = 0x1024,
            // Команда смены значения захвата планетоида
            PlanetCaptureUpdate = 0x1025,
            // Команда результата подписки на планетарку
            /*CMD_SYSTEM_LOAD_END = 0x1026,*/
            // Команда смены размера хранилища планетоида
            PlanetStorageResize = 0x1027,
            // Команда обновления количества модулей на планетоиде
            Planet_MODULES_UPDATE = 0x1028,
            // Команда обновления значения таймера кораблика
            ShipUpdateTimer = 0x1029,
            // Команда обновления параметров портала
            Planet_PORTAL_UPDATE = 0x102A,
            // Команда обновления признака низкой гравитации
            ___free_4 = 0x102B,
            // Команда моментального перемещения корабля
            ShipJumpTo = 0x102C,
            // Команда обновления уровня топлива
            _free_3 = 0x102D,
            // Загрузка планетоидов системы
            PlanetLoadPlanets = 0x102E,

            ActionError = 0x102F,
        }

        public ProtocolWriter(PlanetaryEngine aCore) : base(aCore)
        {
        }

        public bool Prepare(Commands aCommand, out SocketPacket aPacket)
        {
            if (!Engine.Available)
            {
                aPacket = null;
                return false;
            }
            else
            {
                aPacket = new SocketPacket(aCommand);
                return true;
            }
        }

        public void Send(SocketPacket aPacket, Planet aPlanet)
        {
            /*aPlayer.Connection.Push(fBuffer);*/
        }

        public void s()
        {

        }

        public bool SendError(string aError, Player aPlayer)
        {
            Core.Log.Warn(aError);

            fPacket = new SocketPacket(Commands.ActionError);
            fPacket.WriteString(aError);

            aPlayer.Connection.Push(fPacket);

            return false;
        }

        // Команда обновления ангара игрока
        public void PlayerHangarUpdate(Hangar aHangar, Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.Player_HANGAR_UPDATE);
            fPacket.WriteInt(aHangar.Position);
            fPacket.WriteInt(aHangar.ShipType);
            fPacket.WriteInt(aHangar.Count);
            fPacket.WriteBool(aHangar.Locked);

            aPlayer.Connection.Push(fPacket);
        }

        public void PlayerInfoUpdate(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlayerInfoUpdate);
            fPacket.WriteInt(aPlayer.Quasi);
            fPacket.WriteInt(aPlayer.Credits);
            fPacket.WriteInt(aPlayer.Fuel);


            aPlayer.Connection.Push(fPacket);
        }

        public void PlayerTechWarShipLoad(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlayerTechWarshipCreate);
            // Закинем все данные технологий
            foreach (ShipType tmpType in Enum.GetValues(typeof(ShipType)))
            {
                if (tmpType == ShipType.Empty)
                    continue;
                foreach (ShipTech tmpTech in Enum.GetValues(typeof(ShipTech)))
                {
                    if (tmpTech == ShipTech.Empty)
                        continue;
                    TechItem tmpItem = aPlayer.Planetary.TechShipProfile[tmpType][tmpTech];
                    fPacket.WriteBool(tmpItem != null);
                    if (tmpItem == null)
                        continue;
                    // Закинем текущее значение и количество уровней
                    fPacket.WriteInt(aPlayer.Planetary.TechShipValues[tmpType][tmpTech]);
                    fPacket.WriteInt(tmpItem.Count);
                    // Закинем сами значения уровней
                    for (int tmpIndex = 0; tmpIndex <= tmpItem.Count; tmpIndex++)
                        fPacket.WriteInt(tmpItem.Levels[tmpIndex]);
                }
            }
            aPlayer.Connection.Push(fPacket);
        }

        /// <summary>
        /// Уведомление о результате подписки на планетарку
        /// </summary>
        /// <param name="aPlayer">Подписанный игрок</param>
        public void PlanetLoadBegin(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlanetLoadBegin);
            // Запись размеров созвездия, в секторах
            fPacket.WriteInt(Engine.SizeX);
            fPacket.WriteInt(Engine.SizeY);
            // Отправим данные игроку
            aPlayer.Connection.Push(fPacket);
        }

        public void PlanetLoadEnd(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            // Команда завершения загрузки
            fPacket = new SocketPacket(Commands.SystemLoadComplete);
            // Отправим данные игроку
            aPlayer.Connection.Push(fPacket);
        }

        public void PlanetLoadPlanets(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlanetLoadPlanets);
            // Запись количества передаваемых планет
            fPacket.WriteInt(Engine.Planets.PlanetList.Count);
            // Передача данных планеты
            foreach (var tmpPlanet in Engine.Planets.PlanetList)
            {
                // Видимость ядра планеты - видно только соседние планеты
                bool tmpHardLight = tmpPlanet.VisibleByPlayer(aPlayer, true);
                bool tmpSoftLight = tmpPlanet.VisibleByPlayer(aPlayer, false);
                bool tmpHaveSelf = tmpPlanet.CoverageByPlayer(aPlayer, true, out bool tmpHaveFriends, out bool tmpHaveEnemy);
                PlanetState tmpState = tmpPlanet.StateByVisible(tmpSoftLight);
                // И запишем в стрим
                fPacket.WriteInt(tmpPlanet.ID);
                fPacket.WriteInt(tmpPlanet.CoordX);
                fPacket.WriteInt(tmpPlanet.CoordY);
                fPacket.WriteInt(tmpPlanet.Type);
                fPacket.WriteInt(tmpPlanet.Mode);
                fPacket.WriteInt(tmpPlanet.Owner == aPlayer ? tmpPlanet.Owner.ID : 1);
                fPacket.WriteInt(tmpState);
                fPacket.WriteBool(tmpHardLight);
                fPacket.WriteBool(tmpSoftLight);
                fPacket.WriteBool(tmpHaveSelf);
                fPacket.WriteBool(tmpHaveFriends);
                fPacket.WriteBool(tmpHaveEnemy);
            }
            // Передача данных ссылок каждой планеты
            foreach (var tmpPlanet in Engine.Planets.PlanetList)
            {
                fPacket.WriteInt(tmpPlanet.Links.Count);
                foreach (Planet tmpLink in tmpPlanet.Links)
                    fPacket.WriteInt(tmpLink.ID);
            }
            // Отправим данные игроку
            aPlayer.Connection.Push(fPacket);
        }

        public void PlanetLoadHangar(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            // Отправим ангар
            foreach (Hangar tmpSlot in aPlayer.Planetary.HangarZone.Slots)
                PlayerHangarUpdate(tmpSlot, aPlayer);
        }

        public void PlayerStorageResize(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlayerStorageResize);
            fPacket.WriteInt(aPlayer.HoldingZone.Slots.Count);
            aPlayer.Connection.Push(fPacket);
        }

        public void PlayerStorageUpdate(Player aPlayer, Holding aSlot)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlayerStorageUpdate);
            fPacket.WriteInt(aSlot.Position);
            fPacket.WriteInt(aSlot.ResourceType);
            fPacket.WriteInt(aSlot.Count);

            aPlayer.Connection.Push(fPacket);
        }

        public void PlayerLoadStorage(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            // Отправим размер
            PlayerStorageResize(aPlayer);
            // Отправим слоты
            foreach (Holding tmpSlot in aPlayer.HoldingZone.Slots)
                PlayerStorageUpdate(aPlayer, tmpSlot);
        }

        /// <summary>
        /// Разрешение клиенту на подключение к созвездию
        /// </summary>
        /// <param name="aPlayer">Игрок</param>
        public void PlanetStarted(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlanetStarted);
            fPacket.WriteInt(aPlayer.ID);
            aPlayer.Connection.Push(fPacket);
        }

        /// <summary>
        /// Открытие портала с планеты
        /// </summary>
        /// <param name="aPortal">Объект портала</param>
        /// <param name="aPlayer">Инициирующий игрок</param>
        public void PlanetPortalOpen(Portal aPortal, Player aPlayer = null)
        {
            if (!Engine.Available)
                return;
            // Запишем новый буффер
            fPacket = new SocketPacket(Commands.PlanetPortalOpen);
            fPacket.WriteInt(aPortal.In.ID);
            fPacket.WriteInt(aPortal.Out.ID);
            fPacket.WriteBool(aPortal.Breakable);
            fPacket.WriteInt(aPortal.Limit);

            Engine.Player.Connection.Push(fPacket);
            // Отправим рассылку всем подписавшимся на планету или всем для чт
            //if (aSource.Type == PlanetType.Hole) ;
            /*SendBuffer()
          else
            SendBuffer(ASource, APlayer);*/
        }

        /// <summary>
        /// Закрытие портала планеты
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Инициирующий игрок</param>
        public void PlanetPortalClose(Planet aPlanet, Player aPlayer = null)
        {
            if (!Engine.Available)
                return;
            // Запишем новый буффер
            fPacket = new SocketPacket(Commands.PlanetPortalClose);
            fPacket.WriteInt(aPlanet.ID);


            // Отправим рассылку всем подписавшимся на планету или всем для чт
            //   if (aPlanet.Type == PlanetType.Hole) ;

            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer()
          else
            SendBuffer(aPlanet);*/
        }

        /// <summary>
        /// Обновление контроля игрока над планетой
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aIncrement">Получение или потеря контроля</param>
        public void PlanetCoverageUpdate(Planet aPlanet, Player aPlayer, bool aIncrement)
        {
            if (!Engine.Available)
                return;
            // Запишем новый буффер
            fPacket = new SocketPacket(Commands.PlanetCoverageUpdate);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteBool(aIncrement);


            /* роль определяется зендером */
            fPacket.WriteInt(PlayerRole.Friends);
            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer(nil, nil, nil, APlayer);*/
        }

        /// <summary>
        /// Обновление видимости планетоида
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aHardLight">Текущая или окраина</param>
        /// <param name="aIncrement">Получение или потеря видимости</param>
        public void PlanetVisibilityUpdate(Planet aPlanet, Player aPlayer, bool aHardLight, bool aIncrement)
        {
            if (!Engine.Available)
                return;
            // Запишем новый буффер
            fPacket = new SocketPacket(Commands.PlanetVisibilityUpdate);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteBool(aHardLight);
            fPacket.WriteBool(aIncrement);

            Engine.Player.Connection.Push(fPacket);
            // Выключение только подписавшимся
            /*if (aIncrement) ;
            SendBuffer(nil, APlayer)
            else;
            SendBuffer(aPlanet, APlayer)*/
        }

        /// <summary>
        /// Обновление состояния планетоида
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок назначения</param>
        public void PlanetStateUpdate(Planet aPlanet, Player aPlayer)
        {
            if (!Engine.Available)
                return;
            // Запишем новый буффер
            fPacket = new SocketPacket(Commands.PlanetStateUpdate);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteInt(aPlanet.State);
            /*SendBuffer(APlayer);*/
            // Обновим время для подписавшихся
            //      fBuffer = new SocketBuffer(Commands.PlanetStateTime);
            //      fBuffer.WriteInt(aPlanet.ID);
            //      fBuffer.WriteInt(aPlanet.TimerValue(PlanetTimer.Activity));

            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer(aPlanet, APlayer);*/
        }

        /// <summary>
        /// Обновление владельца планетоида
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок назначения</param>
        public void PlanetOwnerChanged(Planet aPlanet, Player aPlayer = null)
        {
            if (!Engine.Available)
                return;
            // Запишем новый буффер
            fPacket = new SocketPacket(Commands.PlanetOwnerChanged);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteInt(aPlanet.Owner.ID);

            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer(aPlanet, APlayer);*/
        }

        /// <summary>
        /// Обновление состояния подписки
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aSubscribed">Признак подписки</param>
        /// <param name="aPlayer">Игрок назначения</param>
        public void PlanetSubscriptionChange(Planet aPlanet, bool aSubscribed, Player aPlayer)
        {
            if (!Engine.Available)
                return;
            // Запишем новый буффер
            fPacket = new SocketPacket(Commands.PlanetSubscriptionChanged);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteBool(aSubscribed);

            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer(aPlanet, APlayer);*/
        }

        public void PlanetCaptureUpdate(Planet aPlanet)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlanetCaptureUpdate);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteInt(aPlanet.CaptureValue);

            fPacket.WriteInt(PlayerRole.Self);
            Engine.Player.Connection.Push(fPacket);
        }

        public void PlanetStorageResize(Planet aPlanet, bool aClear, Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlanetStorageResize);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteInt(aPlanet.StorageZone.Slots.Count);
            fPacket.WriteBool(aClear);

            Engine.Player.Connection.Push(fPacket);
        }

        public void PlanetStorageUpdate(Planet aPlanet, Storage aStorage)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.PlanetStorageUpdate);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteInt(aStorage.Position);
            fPacket.WriteInt(aStorage.ResourceType);
            fPacket.WriteInt(aStorage.Count);
            fPacket.WriteInt(0/*flags*/);
            fPacket.WriteBool(/*active*/true);

            Engine.Player.Connection.Push(fPacket);
        }

        public void PlanetEnergyUpdate(Planet aPlanet, Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.Planet_ELECTRO_UPDATE);
            fPacket.WriteInt(aPlanet.ID);
            fPacket.WriteInt(aPlanet.Energy);

            Engine.Player.Connection.Push(fPacket);
        }

        public void PlanetDetailsShow(Planet aPlanet, Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.Planet_DETAILS_SHOW);

            Engine.Player.Connection.Push(fPacket);
        }

        /// <summary>
        /// Обновление статуса корабля
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        public void ShipUpdateState(Ship aShip)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.ShipUpdateState);
            fPacket.WriteInt(aShip.ID);
            fPacket.WriteInt(aShip.State);
            fPacket.WriteInt(aShip.Mode);
            fPacket.WriteInt(aShip.Fuel);

            Engine.Player.Connection.Push(fPacket);
        }

        /// <summary>
        /// Обновление параметров таймера
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aTimer">Тип таймера</param>
        /// <param name="aSeconds">Время отсчета</param>
        public void ShipUpdateTimer(Timer aTimer)
        {
            if (!Engine.Available)
                return;

            fPacket = new SocketPacket(Commands.ShipUpdateTimer);
            fPacket.WriteInt(((Ship)aTimer.Object).ID);
            fPacket.WriteInt(aTimer.ID);
            fPacket.WriteInt((int)TimeSpan.FromTicks(aTimer.Time - DateTime.Now.Ticks).TotalMilliseconds);

            Engine.Player.Connection.Push(fPacket);
        }

        /// <summary>
        /// Обновление параметров таймера
        /// </summary>
        /// <param name="aShip">Планета</param>
        /// <param name="aTimer">Тип таймера</param>
        /// <param name="aSeconds">Время отсчета</param>
        public void PlanetUpdateTimer(Timer aTimer)
        {
            if (!Engine.Available)
                return;

            fPacket = new SocketPacket(Commands.PlanetUpdateTimer);
            fPacket.WriteInt(((Planet)aTimer.Object).ID);
            fPacket.WriteInt(aTimer.ID);
            fPacket.WriteInt((int)TimeSpan.FromTicks(aTimer.Time - DateTime.Now.Ticks).TotalMilliseconds);

            Engine.Player.Connection.Push(fPacket);
        }

        /// <summary>
        /// Перелет корабля
        /// </summary>
        /// <param name="aShip">Корабль</param>
        /// <param name="aLanding">Посадочный слот</param>
        public void ShipMoveTo(Ship aShip, Landing aLanding)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.ShipFlyTo);
            fPacket.WriteInt(aShip.ID);
            fPacket.WriteInt(aLanding.Planet.ID);
            fPacket.WriteInt(aLanding.Position);

            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer(AShip.Planet);*/
        }

        /// <summary>
        /// Смена аттача корабля
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        public void ShipChangeAttach(Ship aShip)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.ShipChangeAttach);
            fPacket.WriteInt(aShip.ID);
            fPacket.WriteInt(aShip.Attach != null ? aShip.Attach.ID : -1);
            fPacket.WriteBool(aShip.IsCapture);
            fPacket.WriteBool(aShip.IsAutoTarget);

            Engine.Player.Connection.Push(fPacket);
        }

        /// <summary>
        /// Прыжок корабля
        /// </summary>
        /// <param name="aShip">Корабль</param>
        /// <param name="aLanding">Посадочный слот</param>
        public void ShipJumpTo(Ship aShip, Landing aLanding)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.ShipJumpTo);
            fPacket.WriteInt(aShip.ID);
            fPacket.WriteInt(aLanding.Planet.ID);
            fPacket.WriteInt(aLanding.Position);

            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer(AShip.Planet);*/
        }

        /// <summary>
        /// Удаление кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aExplosive">Эффект взрыва</param>
        public void ShipDelete(Ship aShip)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.ShipDelete);
            fPacket.WriteInt(aShip.ID);
            fPacket.WriteBool(aShip.DestroyMode == ShipDestroyMode.Explose);

            Engine.Player.Connection.Push(fPacket);
            /*  SendBuffer(AShip.Planet);*/
        }

        /// <summary>
        /// Переприцеливание кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aTarget">Цель</param>
        /// <param name="aWeaponType">Тип прицеливаемого вооружения</param>
        public void ShipRetarget(Ship aShip, Ship aTarget, ShipWeaponType aWeaponType)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.ShipChangeTarget);
            fPacket.WriteInt(aShip.ID);
            fPacket.WriteInt(aWeaponType);
            fPacket.WriteInt(aTarget != null ? aTarget.ID : -1);

            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer(AShip.Planet, APlayer);*/
        }

        /// <summary>
        /// Обновление ХП кораблика
        /// </summary>
        /// <param name="aSource">Кораблик</param>
        public void ShipUpdateHP(Ship aShip)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.ShipUpdateHP);
            fPacket.WriteInt(aShip.ID);
            fPacket.WriteInt(aShip.Count);
            fPacket.WriteInt(aShip.HP);
            fPacket.WriteInt(aShip.Destructed);

            Engine.Player.Connection.Push(fPacket);
            /*SendBuffer(aShip.Planet);*/
        }

        /// <summary>
        /// Создание кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        public void ShipCreate(Ship aShip, Player aPlayer = null)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.ShipCreate);
            fPacket.WriteInt(aShip.ID);
            fPacket.WriteInt(aShip.Owner.ID);
            fPacket.WriteInt(aShip.ShipType);
            fPacket.WriteInt(aShip.State);
            fPacket.WriteInt(aShip.Mode);
            fPacket.WriteInt(aShip.Attach != null ? aShip.Attach.ID : -1);
            fPacket.WriteInt(aShip.Count);
            fPacket.WriteInt(aShip.HP);
            fPacket.WriteInt(aShip.Fuel);
            fPacket.WriteBool(aShip.IsCapture);
            fPacket.WriteBool(aShip.IsAutoTarget);

            /**/
            Engine.Player.Connection.Push(fPacket);
        }

        public void PlayerHangarBuy(Player aPlayer)
        {
            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.HangarBuy);
            Engine.Player.Connection.Push(fPacket);
        }

        public void PlayerShipTechBuy(ShipType aShipType, ShipTech aShipTech)
        {

            if (!Engine.Available)
                return;
            fPacket = new SocketPacket(Commands.Player_TECH_WARShip_UPDATE);
            fPacket.WriteInt(aShipType);
            fPacket.WriteInt(aShipTech);
            Engine.Player.Connection.Push(fPacket);
        }
    }
}