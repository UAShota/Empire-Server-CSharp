/////////////////////////////////////////////////
//
// Команда игрока - привязка кораблика к планете
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
    /// Класс привязки кораблика к планете
    /// </summary>
    internal class CmdAttach : PlanetaryCommand
    {
        /// <summary>
        /// Проверка на аттач к ЧТ
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDestinaton">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckHoleMoving(Ship aShip, Planet aDestination)
        {
            if (!Engine.Ships.Action.Relocation.Move(aDestination, aShip, true, false))
                return Warning("Can't move to hole");
            else
                return true;
        }

        /// <summary>
        /// Нельзя аттачиться к неактивной чт
        /// </summary>
        /// <param name="aDestination">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckHoleActivity(Planet aDestination)
        {
            if (aDestination.State != PlanetState.Active)
                return Warning("Inactive hole");
            else
                return true;
        }

        /// <summary>
        /// Проверка на аттач к ЧТ
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDestinaton">Планета</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckHole(Ship aShip, Planet aDestination)
        {
            if (aDestination.Type != PlanetType.Hole)
                return true;
            else if (!CheckHoleActivity(aDestination))
                return false;
            else if (!CheckHoleMoving(aShip, aDestination))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Стационарные не аттачатся на другие планеты
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckFlyStationary(Ship aShip)
        {
            if (aShip.TechActive(ShipTech.Stationary))
                return Warning("Stationary");
            else
                return true;
        }

        /// <summary>
        /// Нельзя аттачиться на планеты вне радиуса перелета
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDestination">Планета назначения</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckFlyLink(Ship aShip, Planet aDestination)
        {
            if (!aShip.Planet.Links.Contains(aDestination))
                return Warning("Wrong radius");
            else
                return true;
        }

        /// <summary>
        /// Проверка на попытку перелета после аттача
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDestination">Планета назанчения</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckFly(Ship aShip, Planet aDestination)
        {
            if (aShip.Planet == aDestination)
                return true;
            else if (!CheckFlyStationary(aShip))
                return false;
            else if (!CheckFlyLink(aShip, aDestination))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Захватчику нельзя аттачиться к нежилым планетам
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDestination">Планета назначения</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckManned(Ship aShip, Planet aDestination)
        {
            if (aShip.TechActive(ShipTech.Capturer) && (aDestination.Type != PlanetType.Earth))
                return Warning("Manned can't be captured");
            else
                return true;
        }

        /// <summary>
        /// Сброс аттача
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDestination">Планета назначения</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckDestination(Ship aShip, Planet aDestination)
        {
            if (aDestination == null)
            {
                Engine.Ships.Action.Attach.Call(aShip, aDestination, false);
                return false;
            }
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
        /// Проверка повторного аттача на ту же планету
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDestination">Планета назначения</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckAttach(Ship aShip, Planet aDestination)
        {
            // Нельзя повторно атачиться на ту же планету
            if ((aShip.Attach == aDestination) && (!aShip.IsAutoTarget))
                return Warning("Double attach");
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
        /// Проверка корабля источника
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
        public CmdAttach(PlanetaryEngine aEngine) : base(aEngine)
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
            // Основная валидация
            if (!CheckShip(tmpShip))
                return;
            if (!CheckPlanet(tmpPlanet))
                return;
            if (!CheckOperable(tmpShip))
                return;
            if (!CheckAttach(tmpShip, tmpPlanet))
                return;
            if (!CheckRole(tmpShip, aPacket.Connection.Player))
                return;
            if (!CheckDestination(tmpShip, tmpPlanet))
                return;
            if (!CheckManned(tmpShip, tmpPlanet))
                return;
            if (!CheckFly(tmpShip, tmpPlanet))
                return;
            if (!CheckHole(tmpShip, tmpPlanet))
                return;
            Engine.Ships.Action.Attach.Call(tmpShip, tmpPlanet, false);
        }
    }
}