/////////////////////////////////////////////////
//
// Команда игрока - разделение стеков 
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
    /// Класс разделения стеков 
    /// </summary>
    internal class CmdSeparate : PlanetaryCommand
    {
        /// <summary>
        /// Разделение стеков
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        /// <param name="aLanding">Слот для разделенного кораблика</param>
        /// <param name="aCount">Количество для разделения</param>
        private void Call(Ship aShip, Landing aLanding, int aCount)
        {
            // Если количество не указано или превышает - разделяем половину
            if ((aCount <= 0) || (aCount >= aShip.Count))
                aCount = aShip.Count / 2;
            // Убавить в текущем стеке
            aShip.Count -= aCount;
            // Создать и расположить новый стек
            bool tmpInBattle = aShip.Planet.TimerEnabled(PlanetTimer.Battle);
            Ship tmpShip = Engine.Ships.Action.Utils.CreateShip(aShip.ShipType, aCount, aShip.Owner);
            tmpShip.Fuel = aShip.Fuel;
            // Добавить в общий стек кораблей
            Engine.Ships.Action.Relocation.Add(tmpShip, aLanding, !tmpInBattle, true);
            // Отправить сообщение
            Engine.SocketWriter.ShipUpdateHP(aShip);
            // Для планеты с боем кораблик при разделении получает штраф
            if (tmpInBattle)
                Engine.Ships.Action.Fly.Call(tmpShip, ShipFlyType.Parking);
        }

        /// <summary>
        /// Проверка на доступность высадки
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckArrival(Ship aShip)
        {
            if (!Engine.Ships.Action.Utils.CheckArrival(aShip.Planet, aShip.Owner, true))
                return Warning("Check arrival failed");
            return true;
        }

        /// <summary>
        /// Проверка разделения на нижнюю орбиту
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aLanding">Возвращаемый слот посадки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLowOrbit(Ship aShip, Landing aLanding)
        {
            if ((aLanding.IsLowOrbit) && (!aShip.TechActive(ShipTech.LowOrbit)))
                return Warning("Can't separate to low orbit");
            else
                return true;
        }

        /// <summary>
        /// Попытка получения слота посадки по его индексу
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPosition">Позиция</param>
        /// <param name="aLanding">Возвращаемый слот посадки</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckLanding(Ship aShip, int aPosition, out Landing aLanding)
        {
            if (!aShip.Planet.LandingZone.TryGet(aPosition, out aLanding))
                return Warning("Invalid slot");
            else
                return true;
        }

        /// <summary>
        /// Нельзя отправить чужой кораблик
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlayer">Инициирующий игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Ship aShip, Player aPlayer)
        {
            if (aShip.Owner != aPlayer)
                return Warning("Wrong owner");
            else
                return true;
        }

        /// <summary>
        /// Количество разделяемых кораблей
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckCount(Ship aShip)
        {
            if (aShip.Count < 2)
                return Warning("Not enough ships for separate");
            else
                return true;
        }

        /// <summary>
        /// Кораблик должен быть доступен
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckOperable(Ship aShip)
        {
            if (!aShip.IsOperable)
                return Warning("Ship is not operable");
            else
                return true;
        }

        /// <summary>
        /// Проверка кораблика
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
        /// <param name="aEngine">Базовый контроллер</param>
        public CmdSeparate(PlanetaryEngine aEngine) : base(aEngine)
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
            int tmpCount = aPacket.ReadInt();
            // Основная валидация
            if (!CheckShip(tmpShip))
                return;
            if (!CheckOperable(tmpShip))
                return;
            if (!CheckCount(tmpShip))
                return;
            if (!CheckLanding(tmpShip, tmpPosition, out Landing tmpLanding))
                return;
            if (!CheckLowOrbit(tmpShip, tmpLanding))
                return;
            if (!CheckRole(tmpShip, aPacket.Connection.Player))
                return;
            if (!CheckArrival(tmpShip))
                return;
            // Отправим команду на исполнение
            Call(tmpShip, tmpLanding, tmpCount);
        }
    }
}