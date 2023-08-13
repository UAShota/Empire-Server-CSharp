/////////////////////////////////////////////////
//
// Словари планетарных технологий
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

namespace Empire.Planetary
{
    /// <summary>
    /// Словари планетарных технологий
    /// </summary>
    internal class PlanetaryDictionary : IDisposable
    {
        /// <summary>
        /// Словарь технологий корабликов
        /// </summary>
        public ShipTechRace ShipTechList { get; private set; }

        /// <summary>
        /// Словарь технологий строений
        /// </summary>
        public BuildingTechRace BuildingTechList { get; private set; }

        /// <summary>
        /// Загрузка технологий корабликов
        /// </summary>
        private void LoadShipTechList()
        {
            ShipTechList = new ShipTechRace();
            var tmpRaces = Enum.GetValues(typeof(PlayerRace));
            // Загрузим с базы
            using (var tmpReader = Core.Database.Query("PLLoadDictTechShips"))
            {
                while (tmpReader.Read())
                {
                    ShipType tmpShipType = (ShipType)tmpReader.ReadInt("ID_OBJECT");
                    ShipTech tmpTechType = (ShipTech)tmpReader.ReadInt("ID_TECH");
                    PlayerRace tmpTechRace = (PlayerRace)tmpReader.ReadInt("ID_RACE");
                    // Загрузим техи для каждой расы отдельно
                    foreach (PlayerRace tmpRace in tmpRaces)
                    {
                        // Откидваем пустой тип
                        if (tmpRace == PlayerRace.Empty)
                            continue;
                        // При разницы расы грузим только общие техи для всех рас
                        if ((tmpTechRace != tmpRace) && (tmpTechRace != PlayerRace.Empty))
                            continue;
                        // Создадим объекты, 0-я теха есть всегда
                        int tmpCount = tmpReader.ReadInt("LEVEL_COUNT");
                        TechItem tmpTechInfo = new TechItem(tmpCount);
                        // Пропишем теху
                        for (int tmpI = 0; tmpI <= tmpCount; tmpI++)
                        {
                            string tmpIndex = tmpI.ToString();
                            tmpTechInfo.Levels[tmpI] = tmpReader.ReadInt("LEVEL_" + tmpIndex);
                            tmpTechInfo.Cooldowns[tmpI] = tmpReader.ReadInt("CD_" + tmpIndex);
                        }
                        ShipTechList[tmpRace][tmpShipType][tmpTechType] = tmpTechInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Загрузка технологий строений
        /// </summary>
        private void LoadBuildingTechList()
        {
            BuildingTechList = new BuildingTechRace();
            var tmpRaces = Enum.GetValues(typeof(PlayerRace));
            // Загрузим с базы
            using (var tmpReader = Core.Database.Query("PLLoadDictTechBuilding"))
            {
                while (tmpReader.Read())
                {
                    BuildingType tmpBuildingType = (BuildingType)tmpReader.ReadInt("ID_OBJECT");
                    BuildingTech tmpTechType = (BuildingTech)tmpReader.ReadInt("ID_TECH");
                    PlayerRace tmpTechRace = (PlayerRace)tmpReader.ReadInt("ID_RACE");
                    // Загрузим техи для каждой расы отдельно
                    foreach (PlayerRace tmpRace in tmpRaces)
                    {
                        // Если раса не указана - значит теха общая, либо если указана - только для указанной расы
                        if ((tmpTechRace != PlayerRace.Empty) && (tmpRace != tmpTechRace))
                            continue;
                        // Создадим объекты
                        BuildingTechItem tmpTechInfo = new BuildingTechItem()
                        {
                            Count = tmpReader.ReadInt("LEVEL_COUNT"),
                            Levels = new int[5],/**/
                        };
                        // Пропишем теху
                        for (int tmpI = 0; tmpI < 5/**/; tmpI++)
                        {
                            string tmpIndex = tmpI.ToString();
                            tmpTechInfo.Levels[tmpI] = tmpReader.ReadInt("LEVEL_" + tmpIndex);
                        }
                        // Запишем в наши структуры
                        BuildingTechList[tmpRace][tmpBuildingType][tmpTechType] = tmpTechInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public PlanetaryDictionary()
        {
            LoadShipTechList();
            LoadBuildingTechList();
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        public void Dispose()
        {
        }
    }
}