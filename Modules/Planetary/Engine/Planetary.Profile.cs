/////////////////////////////////////////////////
//
// Планетарный профиль игрока
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System;
using Empire.EngineSpace;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary
{
    /// <summary>
    /// Класс управления планетарным профилем
    /// </summary>
    internal class PlanetaryProfile : IDisposable
    {
        /// <summary>
        /// Игрок владеющий профилем
        /// </summary>
        private Player fPlayer { get; set; }

        /// <summary>
        /// Ссылка на свое созвездие
        /// </summary>
        private PlanetaryEngine SelfEngine { get; set; }

        /// <summary>
        /// Купленные техи корабликлв
        /// </summary>
        public ShipTechValues TechShipValues { get; private set; }

        /// <summary>
        /// Купленные техи строений
        /// </summary>
        public BuildingTechValues TechBuildingValues { get; private set; }

        /// <summary>
        /// Ссылка на расовые техи корабликов
        /// </summary>
        public ShipTechProfile TechShipProfile { get; private set; }

        /// <summary>
        /// Ссылка на расовые техи строений
        /// </summary>
        public BuildingTechProfile TechBuildingProfile { get; private set; }

        /// <summary>
        /// Планетарный ангар профиля
        /// </summary>
        public HangarZone HangarZone { get; private set; }

        /// <summary>
        /// Ссылка на созвездие подписки
        /// </summary>
        public PlanetaryEngine ActiveEngine { get; private set; }

        /// <summary>
        /// Загрузка технологий корабликов
        /// </summary>
        private void LoadTechWarShips()
        {
            TechShipProfile = Core.Server.Planetary.Dictionary.ShipTechList[fPlayer.Race];
            // Обновим технологии для конкретного пользователя
            using (var tmpReader = Core.Database.Query("PLLoadDataTechShips", fPlayer.ID))
            {
                while (tmpReader.Read())
                {
                    ShipType tmpType = (ShipType)tmpReader.ReadInt("ID_OBJECT");
                    ShipTech tmpTech = (ShipTech)tmpReader.ReadInt("ID_TECH");
                    int tmpLevel = tmpReader.ReadInt("LEVEL");
                    // Установим значение
                    TechShipValues[tmpType][tmpTech] = tmpLevel;
                }
            }
        }

        /// <summary>
        /// Загрузка технологий строений
        /// </summary>
        private void LoadTechBuildings()
        {
            TechBuildingProfile = Core.Server.Planetary.Dictionary.BuildingTechList[fPlayer.Race];
            // Обновим технологии для конкретного пользователя
            using (var tmpReader = Core.Database.Query("PLLoadDataTechBuildings", fPlayer.ID))
            {
                while (tmpReader.Read())
                {
                    BuildingType tmpType = (BuildingType)tmpReader.ReadInt("ID_OBJECT");
                    BuildingTech tmpTech = (BuildingTech)tmpReader.ReadInt("ID_TECH");
                    int tmpLevel = tmpReader.ReadInt("LEVEL");
                    // Установим значение
                    TechBuildingValues[tmpType][tmpTech] = tmpLevel;
                }
            }
        }

        /// <summary>
        /// Загрузка ангара
        /// </summary>
        private void LoadHangar()
        {
            // Определим размер ангара
            HangarZone = new HangarZone();
            HangarZone.Resize(/**/5);
            // Загрузим профиль игрока
            using (var tmpReader = Core.Database.Query("PLLoadHangar", fPlayer.ID))
            {
                // Проверим наличие ангара
                if (!tmpReader.Read())
                {
                    Core.Log.Error("Hangar {0} not found", fPlayer.ID);
                    return;
                }
                // Сохраним слоты
                for (int tmpID = 0; tmpID < HangarZone.Slots.Count; tmpID++)
                {
                    HangarZone.Slots[tmpID].Change(tmpReader.ReadInt("COUNT_" + tmpID.ToString()),
                                     (ShipType)tmpReader.ReadInt("ID_TYPE_" + tmpID.ToString()));
                }
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aPlayer">Экземпляр игрока</param>
        public PlanetaryProfile(Player aPlayer)
        {
            fPlayer = aPlayer;
            TechShipValues = new ShipTechValues();
            TechBuildingValues = new BuildingTechValues();
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Загрузка профиля
        /// </summary>
        public void Start()
        {
            SelfEngine = new PlanetaryEngine(fPlayer);
        }

        /// <summary>
        /// Выгрузка профиля
        /// </summary>
        public void Stop()
        {
            SelfEngine.Dispose();
        }

        /// <summary>
        /// Возвращение доступности созвездия
        /// </summary>
        /// <returns>Доступность созвездия</returns>
        public bool Available()
        {
            return (SelfEngine != null) && (SelfEngine.Available);
        }

        /// <summary>
        /// Подписка указанного игрока к текущей планетарке
        /// </summary>
        /// <param name="aBuffer">Ьуфер команды</param>
        /// <returns>Успешность подписки</returns>
        public bool Subscribe(SocketPacket aBuffer)
        {
            if ((SelfEngine != null) && (SelfEngine.Command(aBuffer)))
            {
                aBuffer.Connection.Player.Planetary.ActiveEngine = SelfEngine;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Отписка текущего игрока
        /// </summary>
        public void Unsubscribe()
        {
            ActiveEngine.Unsubscribe(fPlayer);
            ActiveEngine = null;
        }

        /// <summary>
        /// Загрузка параметров профиля
        /// </summary>
        public void Load()
        {
            LoadTechWarShips();
            LoadTechBuildings();
            LoadHangar();
        }

        /// <summary>
        /// Покупка технологии кораблика
        /// </summary>
        /// <param name="aShipType">Тип кораблика</param>
        /// <param name="aTech">Тип технологии</param>
        public void BuyTech(ShipType aShipType, ShipTech aTech)
        {
            // Проверим наличие технологии для типа корабля
            var tmpTech = TechShipProfile[aShipType][aTech];
            if (tmpTech == null)
            {
                Core.Log.Warn("No tech {0} for {1}", aTech.ToString(), aShipType.ToString());
                return;
            }
            // Проверим на потолок покупки
            if (TechShipValues[aShipType][aTech] == tmpTech.Count)
            {
                Core.Log.Warn("Overload tech {0} for {1}", aTech.ToString(), aShipType.ToString());
                return;
            }
            // Купим теху
            TechShipValues[aShipType][aTech]++;
            // Уведомим клиентов
            ActiveEngine.SocketWriter.PlayerShipTechBuy(aShipType, aTech);
        }
    }
}