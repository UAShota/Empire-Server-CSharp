/////////////////////////////////////////////////
//
// Команда игрока - перемещение
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс перемещения кораблика между планетами
    /// </summary>
    internal class CmdMove : PlanetaryCommand
    {
        /// <summary>
        /// Запрещено быть на нижней орбите без технологии нижней орбиты
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLowOrbitTech(Ship aShip)
        {
            if (!aShip.TechActive(ShipTech.LowOrbit))
                return Warning("No low tech");
            else
                return true;
        }

        /// <summary>
        /// Запрещено перелетать на нижнюю орбиту если идет бой
        /// </summary>
        /// <param name="aLanding">Слот посадки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLowOrbitBattle(Landing aLanding)
        {
            if (aLanding.Planet.TimerEnabled(PlanetTimer.Battle))
                return Warning("Low in battle");
            else
                return true;
        }

        /// <summary>
        /// Нельзя перемещаться на нижнюю орбиту на враждебной планете
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aLanding">Слот посадки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLowOrbitRole(Ship aShip, Landing aLanding)
        {
            if (!aLanding.Planet.Owner.IsRoleFriend(aShip.Owner))
                return Warning("Low not friend");
            else
                return true;
        }

        /// <summary>
        /// Проверка перемещения на нижнюю орбиту
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aLanding">Слот посадки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLowOrbit(Ship aShip, Landing aLanding)
        {
            // При смене типа орбиты дополнительная проверка
            if (aShip.Landing.IsLowOrbit || !aLanding.IsLowOrbit)
                return true;
            else if (!CheckLowOrbitRole(aShip, aLanding))
                return false;
            else if (!CheckLowOrbitBattle(aLanding))
                return false;
            else if (!CheckLowOrbitTech(aShip))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Нельзя управлять чужими корабликами
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlayer">Игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Ship aShip, Player aPlayer)
        {
            if (!aShip.Owner.IsRoleFriend(aPlayer))
                return Warning("Wrong role");
            else
                return true;
        }

        /// <summary>
        /// Проверка на свободный слот
        /// </summary>
        /// <param name="aLanding"></param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLanding(Landing aLanding)
        {
            if (aLanding.Ship != null)
                return Warning("Slot already aquired");
            else
                return true;
        }

        /// <summary>
        /// Проверка на корректность слота
        /// </summary>
        /// <param name="aPlanet"></param>
        /// <param name="aPosition"></param>
        /// <param name="aLanding"></param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLandingZone(Planet aPlanet, int aPosition, out Landing aLanding)
        {
            if (!aPlanet.LandingZone.TryGet(aPosition, out aLanding))
                return Warning("Invalid Slot");
            else
                return true;
        }

        /// <summary>
        /// Существование планеты
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
        /// Существование корабля
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckShip(Ship aShip)
        {
            if (aShip == null)
                return Warning("Invalid ship");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdMove(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Ship tmpShip = ReadShip(aPacket);
            Planet tmpPlanet = ReadPlanet(aPacket);
            int tmpPosition = aPacket.ReadInt();
            // Основная валидация
            if (!CheckShip(tmpShip))
                return;
            if (!CheckPlanet(tmpPlanet))
                return;
            if (!CheckLandingZone(tmpPlanet, tmpPosition, out Landing tmpLanding))
                return;
            if (!CheckLanding(tmpLanding))
                return;
            if (!CheckLowOrbit(tmpShip, tmpLanding))
                return;
            if (!CheckRole(tmpShip, aPacket.Connection.Player))
                return;
            // Попробуем отправить кораблик
            if (!Engine.Ships.Action.Relocation.Move(tmpShip, tmpLanding, false))
                Warning("Can't move ship");
        }
    }
}