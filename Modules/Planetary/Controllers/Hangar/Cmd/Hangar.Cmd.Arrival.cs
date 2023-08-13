/////////////////////////////////////////////////
//
// Команда игрока - отправка в ангар
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
    internal class CmdArrival : PlanetaryCommand
    {
        /// <summary>
        /// Попытка отправить кораблик в ангар
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aHangar">Ангар</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckArrival(Ship aShip, Hangar aHangar)
        {
            if (!Engine.Hangar.Action.Arrival.Call(aShip, aHangar))
                return Warning("Explicit mass");
            else
                return true;
        }

        /// <summary>
        /// Проверка типа флота в слоте ангара
        /// </summary>
        /// <param name="aHangar">Слот ангара</param>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckHangar(Hangar aHangar, Ship aShip)
        {
            // В ангаре в этом слоту корабль этого-же типа
            if (aHangar.ShipType == aShip.ShipType)
                return CheckArrival(aShip, aHangar);
            // В ангаре в этом слоту пусто
            else if (aHangar.ShipType == ShipType.Empty)
                return CheckArrival(aShip, aHangar);
            // Слот занят
            else
                return Warning("Different ship types");
        }

        /// <summary>
        /// Проверка наличия топлива
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckFuel(Ship aShip)
        {
            if (!Engine.Hangar.Action.Arrival.HasFuel(aShip))
                return Warning("Not enough fuel");
            else
                return true;
        }

        /// <summary>
        /// Проверка состояния кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckOperable(Ship aShip)
        {
            if (!aShip.IsOperable)
                return Warning("Not operable");
            else
                return true;
        }

        /// <summary>
        /// Проверка на наличие боя
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckBattle(Ship aShip)
        {
            if (aShip.Planet.TimerEnabled(PlanetTimer.Battle))
                return Warning("In battle");
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
        /// Проверка существования слота
        /// </summary>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aPosition">Позиция слота</param>
        /// <param name="aHangar">Найденный слот ангара</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckHangar(Player aPlayer, int aPosition, out Hangar aHangar)
        {
            if (aPlayer.Planetary.HangarZone.TryGetSlot(aPosition, out aHangar))
                return true;
            else
                return Warning("Invalid hangar slot");
        }

        /// <summary>
        /// Проверка принадлежности кораблика
        /// </summary>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Player aPlayer, Ship aShip)
        {
            if (aShip.Owner != aPlayer)
                return Warning("Wrong owner");
            else
                return true;
        }

        /// <summary>
        /// Проверка стационарности кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckStationary(Ship aShip)
        {
            if (aShip.TechActive(ShipTech.Stationary))
                return Warning("Stationary");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdArrival(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Ship tmpShip = ReadShip(aPacket);
            int tmpPosition = aPacket.ReadInt();
            // Проверки
            if (!CheckShip(tmpShip))
                return;
            if (!CheckOperable(tmpShip))
                return;
            if (!CheckHangar(tmpShip.Owner, tmpPosition, out Hangar aHangarSlot))
                return;
            if (!CheckRole(aPacket.Connection.Player, tmpShip))
                return;
            if (!CheckStationary(tmpShip))
                return;
            if (!CheckBattle(tmpShip))
                return;
            if (!CheckFuel(tmpShip))
                return;
            if (!CheckHangar(aHangarSlot, tmpShip))
                return;
        }
    }
}