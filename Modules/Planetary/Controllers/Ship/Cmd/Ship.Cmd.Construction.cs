/////////////////////////////////////////////////
//
// Команда игрока - постройка флота
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс постройки флота
    /// </summary>
    internal class CmdConstruction : PlanetaryCommand
    {
        /// <summary>
        /// Дабы не захламлять
        /// </summary>
        /// <param name="aCount">Количество юнитов для постройки</param>
        /// <param name="aTech">Технологии игрока</param>
        /// <param name="aKeys">Купленные игрока</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckCost(int aCount, ShipTechUnit aTech, ShipTechKeys aKeys, out int aCost)
        {
            int tmpCount = Math.Min(aCount, aTech.Value(ShipTech.Count, aKeys[ShipTech.Count]));
            aCost = tmpCount * aTech.Value(ShipTech.Cost, aKeys[ShipTech.Cost]);
            // Проверить наличие ресурсов для постройки
            /*if (aPlanet.ResAvailIn[resModules] < TmpCost) 
            {
              Log.Do.Warn("Modules");
              return null;
                }*/
            return true;
        }

        /// <summary>
        /// Найдем свободный слот
        /// </summary>
        /// <param name="aPlanet"></param>
        /// <param name="aLowOrbit"></param>
        /// <param name="aLanding"></param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLanding(Planet aPlanet, bool aLowOrbit, out Landing aLanding)
        {
            if (!Engine.Ships.Action.Utils.GetSlot(aPlanet, null, aLowOrbit, false, out aLanding))
                return Warning("No free landing");
            else
                return true; ;
        }

        /// <summary>
        /// Проверка постройки нестационарного флота
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckStationaryPlanet(Planet aPlanet)
        {
            // На нежилых можно строить только стационарки
            if (aPlanet.Type != PlanetType.Earth)
                return Warning("Not earth");
            else
                return true;
        }

        /// <summary>
        /// Проверка постройки нестационарнго флота
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aTech">Технологии юнита</param>
        /// <param name="aKeys">Купленные технологии</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckStationary(Planet aPlanet, ShipTechUnit aTech, ShipTechKeys aKeys)
        {
            if (aTech.IsSupported(ShipTech.Stationary, aKeys[ShipTech.Stationary]))
                return true;
            if (!CheckStationaryPlanet(aPlanet))
                return false;
            else
                return true;
            // Не стационарные строятся только если есть верфь
            /*    if (!aPlanet.Constructors.ContainsKey(aPlayer))
                {
                    Log.Do.Warn("No shipyards");
                    return;
                }*/
        }

        /// <summary>
        /// Проверка наличия технологии постройки
        /// </summary>
        /// <param name="aShipType">Тип корабля</param>
        /// <param name="aTech">Технологии юнита</param>
        /// <param name="aKeys">Купленные технологии</param>
        /// <param name="aPlayer">Владелец постройки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTech(ShipType aShipType, out ShipTechUnit aTech, out ShipTechKeys aKeys, Player aPlayer)
        {
            aTech = aPlayer.Planetary.TechShipProfile[aShipType];
            aKeys = aPlayer.Planetary.TechShipValues[aShipType];
            // Нельзя строить если технология закрыта
            if (!aTech.IsSupported(ShipTech.Active, aKeys[ShipTech.Active]))
                return Warning("Tech closed");
            else
                return true;
        }
        /// <summary>
        /// Попытка высадить ожидаемый кораблик в указанный слот
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckArrival(Planet aPlanet, Player aPlayer)
        {
            if (!Engine.Ships.Action.Utils.CheckArrival(aPlanet, aPlayer, true))
                return Warning("Arrival");
            else
                return true;
        }

        /// <summary>
        /// Нельзя строить если планета в бою
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
        /// Нельзя строить непонятное количество корабликов
        /// </summary>
        /// <param name="aCount">Указанное количество</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckCount(int aCount)
        {
            if (aCount <= 0)
                return Warning("Invalid count");
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
        public CmdConstruction(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Planet tmpPlanet = ReadPlanet(aPacket);
            ShipType tmpShipType = (ShipType)aPacket.ReadInt();
            int tmpCount = aPacket.ReadInt();
            // Проверки
            if (!CheckPlanet(tmpPlanet))
                return;
            if (!CheckCount(tmpCount))
                return;
            if (!CheckBattle(tmpPlanet))
                return;
            if (!CheckArrival(tmpPlanet, aPacket.Connection.Player))
                return;
            if (!CheckTech(tmpShipType, out ShipTechUnit tmpTech, out ShipTechKeys tmpKeys, aPacket.Connection.Player))
                return;
            if (!CheckStationary(tmpPlanet, tmpTech, tmpKeys))
                return;
            if (!CheckLanding(tmpPlanet, tmpTech.IsSupported(ShipTech.LowOrbit, tmpKeys[ShipTech.LowOrbit]), out Landing tmpLanding))
                return;
            if (!CheckCost(tmpCount, tmpTech, tmpKeys, out int tmpCost))
                return;
            // Отправим команду на исполнение
            Engine.Ships.Action.Construction.Call(tmpPlanet, tmpShipType, tmpLanding, tmpCount, tmpCost, aPacket.Connection.Player);
        }
    }
}