/////////////////////////////////////////////////
//
// Команда игрока - высадка из ангара
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.HangarSpace
{
    /// <summary>
    /// Класс отправки в ангар
    /// </summary>
    internal class CmdDeparture : PlanetaryCommand
    {
        /// <summary>
        /// Количество топлива для высадки из ангара
        /// </summary>
        private int ciFuelCost => 10;

        /// <summary>
        /// Высадка с ангара
        /// </summary>
        /// <param name="aHangar">Слот ангара</param>
        /// <param name="aLanding">Слот высадки</param>
        /// <param name="aCount">Количество ожидаемых корабликов</param>
        /// <param name="aPlayer">Инициирующий игрок</param>
        private void LandShip(Hangar aHangar, Landing aLanding, int aCount, Player aPlayer)
        {
            // Построим кораблик для выкладки
            Ship tmpShip = Engine.Ships.Action.Utils.CreateShip(aHangar.ShipType, aCount, aPlayer);
            // Добавить на планету назначения
            Engine.Ships.Action.Relocation.Add(tmpShip, aLanding, true, true);
            // Уберем с ангара
            aHangar.Change(-aCount);
            // Уведомим ангар
            Engine.SocketWriter.PlayerHangarUpdate(aHangar, aPlayer);
        }

        /// <summary>
        /// Проверка своего контроля
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Владелец</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckSelfControl(Planet aPlanet, Player aPlayer)
        {
            foreach (var tmpShipCount in aPlanet.ShipCount)
            {
                if (!tmpShipCount.Key.IsRoleFriend(aPlayer))
                {
                    Warning("Enemy lock");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка вражеского покрытия
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Владелец</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckEnemyControl(Planet aPlanet, Player aPlayer)
        {
            foreach (var tmpCover in aPlanet.PlayerCoverage)
            {
                if (!tmpCover.Key.IsRoleFriend(aPlayer))
                {
                    Warning("Enemy cover");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка контроля высадки
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Владелец</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckControl(Planet aPlanet, Player aPlayer)
        {
            // На бчт можно высаживаться всегда
            if (aPlanet.IsBigHole)
                return true;
            // На свою планету можно высаживаться если там нет враждебных корабликов        
            if (aPlanet.Owner.IsRoleFriend(aPlayer))
            {
                if (!CheckSelfControl(aPlanet, aPlayer))
                    return false;
            }
            // Переберем области закраски
            else
            {
                if (!CheckEnemyControl(aPlanet, aPlayer))
                    return false;
            }
            // Если все хорошо - высаживаемся
            return true;
        }

        /// <summary>
        /// Учет разрешения высадки
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckArrival(Planet aPlanet, Player aPlayer)
        {
            if (!Engine.Ships.Action.Utils.CheckArrival(aPlanet, aPlayer, false))
                return Warning("No arrival");
            else
                return true;
        }

        /// <summary>
        /// Проверка слота высадки
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPosition">Позиция слота</param>
        /// <param name="aLowOrbit">Возможность высадки на торговой орбите</param>
        /// <param name="aLanding">Найденный слот высадки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLanding(Planet aPlanet, int aPosition, bool aLowOrbit, out Landing aLanding)
        {
            // Найдем слот высадки
            if (!aPlanet.LandingZone.TryGet(aPosition, out aLanding))
            {
                // Если указанного слота не существует - поищем свободный
                if (!Engine.Ships.Action.Utils.GetSlot(aPlanet, null, aLowOrbit, false, out aLanding))
                    return Warning("No free slot");
            }
            // Проверим выгрузку в торговый слот
            else
            {
                if ((aLanding.IsLowOrbit) && (!aLowOrbit))
                    return Warning("Cant move to low orbit");
            }
            return true;
        }

        /// <summary>
        /// Проверка количества корабликов в ангаре
        /// </summary>
        /// <param name="aHangar">Слот ангара</param>
        /// <param name="aCount">Количество кораблей для высадки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckHangarCount(Hangar aHangar, int aCount)
        {
            if (aHangar.Count < aCount)
                return Warning("Not enough ships");
            else
                return true;
        }

        /// <summary>
        /// Проверка количества корабликов в ангаре
        /// </summary>
        /// <param name="aTechCount">Максимальное количество корабликов с технологии</param>
        /// <param name="aCount">Количество кораблей для высадки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTechCount(int aTechCount, int aCount)
        {
            if (aTechCount < aCount)
                return Warning("Not enough tech");
            else
                return true;
        }

        /// <summary>
        /// Получение ангара по его номеру
        /// </summary>
        /// <param name="aPosition">Позиция слота</param>
        /// <param name="aPlayer">Владелец ангара</param>
        /// <param name="aHangar">Найденный ангар</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckHangar(int aPosition, Player aPlayer, out Hangar aHangar)
        {
            if (!aPlayer.Planetary.HangarZone.TryGetSlot(aPosition, out aHangar))
                return Warning("Invalid hangar slot");
            else
                return true;
        }

        /// <summary>
        /// Проверка количества выгружаемых корабликов
        /// </summary>
        /// <param name="aCount">Количество</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckDepartureCount(int aCount)
        {
            if (aCount <= 0)
                return Warning("Count not set");
            else
                return true;
        }

        /// <summary>
        /// Проверка боя на планете
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckBattle(Planet aPlanet)
        {
            if (aPlanet.TimerEnabled(PlanetTimer.Battle))
                return Warning("In battle");
            else
                return true;
        }

        /// <summary>
        /// Проверка на пульсирующие планетоиды
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckActivity(Planet aPlanet)
        {
            if (aPlanet.IsBigHole)
                return true;
            if (aPlanet.TimerEnabled(PlanetTimer.Activity))
                return Warning("Activity landing");
            else
                return true;
        }

        /// <summary>
        /// Проверка состояния планеты
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckState(Planet aPlanet)
        {
            // Проверка на активность планеты
            if (aPlanet.State != PlanetState.Active)
                return Warning("Inactive Planet");
            else
                return true;
        }

        /// <summary>
        /// Проверка наличия планеты
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPlanet(Planet aPlanet)
        {
            if (aPlanet == null)
                return Warning("Unknown planet");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdDeparture(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            int tmpHangarIndex = aPacket.ReadInt();
            Planet tmpPlanet = ReadPlanet(aPacket);
            int tmpLandingIndex = aPacket.ReadInt();
            int tmpCount = aPacket.ReadInt();
            // Проверки
            if (!CheckPlanet(tmpPlanet))
                return;
            if (!CheckState(tmpPlanet))
                return;
            if (!CheckActivity(tmpPlanet))
                return;
            if (!CheckBattle(tmpPlanet))
                return;
            if (!CheckDepartureCount(tmpCount))
                return;
            if (!CheckHangar(tmpHangarIndex, aPacket.Connection.Player, out Hangar tmpHangar))
                return;
            if (!CheckHangarCount(tmpHangar, tmpCount))
                return;
            // Получим техи
            ShipTechUnit tmpTech = aPacket.Connection.Player.Planetary.TechShipProfile[tmpHangar.ShipType];
            ShipTechKeys tmpKeys = aPacket.Connection.Player.Planetary.TechShipValues[tmpHangar.ShipType];
            // Доп проверки
            if (!CheckTechCount(tmpTech.Value(ShipTech.Count, tmpKeys[ShipTech.Count]), tmpCount))
                return;
            if (!CheckLanding(tmpPlanet, tmpLandingIndex, tmpTech.IsSupported(ShipTech.LowOrbit, tmpKeys[ShipTech.LowOrbit]), out Landing tmpLanding))
                return;
            if (!CheckArrival(tmpPlanet, aPacket.Connection.Player))
                return;
            if (!CheckControl(tmpPlanet, aPacket.Connection.Player))
                return;
            // Отправим команду на исполнение
            LandShip(tmpHangar, tmpLanding, tmpCount, aPacket.Connection.Player);
        }
    }
}