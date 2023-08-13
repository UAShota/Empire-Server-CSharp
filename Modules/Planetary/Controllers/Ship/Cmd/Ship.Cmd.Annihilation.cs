/////////////////////////////////////////////////
//
// Навык аннигиляции
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using Empire.Modules.Classes;
using Empire.Planetary.Classes;
using Empire.Sockets;

namespace Empire.Planetary.ShipSpace.Skills
{
    /// <summary>
    /// Класс обработки навыка аннигиляции
    /// </summary>
    internal class CmdAnnihilation : PlanetaryCommand
    {
        /// <summary>
        /// Время аннигиляции
        /// </summary>
        private int ciTimerInterval => 10000;

        /// <summary>
        /// Нанесение урона кораблю
        /// </summary>
        /// <param name="aShip">Корабль</param>
        /// <param name="aDamage">Урон</param>
        private void DealDamage(Ship aShip, int aDamage)
        {
            // Для стационарок используется дополнительный урон
            if (aShip.TechActive(ShipTech.Stationary))
                aDamage *= 2;
            // Нанесм урон
            Engine.Ships.Action.Utils.DealDamage(aShip, aDamage);
        }

        /// <summary>
        /// Нанесение урона всем кораблям на планете
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aDamage">Урон</param>
        private void DamagePlanet(Planet aPlanet, int aDamage)
        {
            foreach (Ship tmpShip in aPlanet.Ships)
                DealDamage(tmpShip, aDamage);
            Engine.Ships.Action.Utils.WorkShipHP(aPlanet);
        }

        /// <summary>
        /// Обработка аннигиляции корабля
        /// </summary>
        /// <param name="aShip">Аннигилирующийся кораблик</param>
        private void Annihilate(Ship aShip)
        {
            int tmpDamage = aShip.Count * aShip.TechValue(ShipTech.Annihilation);
            // Нанесем урон корабля на орбите
            DamagePlanet(aShip.Planet, tmpDamage);
            // Нанесем урон всем кораблям вне орбиты для бчт
            if (aShip.Planet.IsBigHole)
            {
                foreach (Planet tmpPlanet in aShip.Planet.Links)
                    DamagePlanet(tmpPlanet, tmpDamage);
            }
        }

        /// <summary>
        /// Таймер аннигиляции
        /// </summary>
        /// <param name="aShip">Объект тайминга</param>
        /// <returns>Время продления таймера</returns>
        private int OnTimer(TimerObject aShip)
        {
            Ship tmpShip = (Ship)aShip;
            // Выполним подрыв
            Annihilate(tmpShip);
            // Уведомим планету что был подрыв
            Engine.Planets.Action.Gravitor.Call(tmpShip);
            // Убьем таймер
            return 0;
        }

        /// <summary>
        /// Запуск аннигиляции командой игрока
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlayer">Инициирующий игрок</param>
        private void Call(Ship aShip, Player aPlayer)
        {
            // Уберем в пассив и выставим тайминг
            Engine.Ships.Action.StandDown.Call(aShip, aShip.Mode, true);
            Engine.Ships.Action.Utils.TimerAdd(aShip, ShipTimer.Annihilation, OnTimer, ciTimerInterval);
            // Обновляем состояние
            Engine.SocketWriter.ShipUpdateState(aShip);
        }

        /// <summary>
        /// Нельзя управлять чужими
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlayer">Игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Ship aShip, Player aPlayer)
        {
            if (!aShip.Owner.IsRoleFriend(aPlayer))
                return Warning("Role");
            else
                return true;
        }

        /// <summary>
        /// Нельзя взрывать если недостаточно кораблей
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckCount(Ship aShip)
        {
            if (aShip.Count < aShip.TechValue(ShipTech.Count) / 2)
                return Warning("Not enough Count");
            else
                return true;
        }

        /// <summary>
        /// Нельзя взрывать повторно
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTimer(Ship aShip)
        {
            if (aShip.TimerEnabled(ShipTimer.Annihilation))
                return Warning("Timer already active");
            else
                return true;
        }

        /// <summary>
        /// Нельзя взрывать без технологии
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTech(Ship aShip)
        {
            if (!aShip.TechActive(ShipTech.Annihilation))
                return Warning("Tech not active");
            else
                return true;
        }

        /// <summary>
        /// Проверка вызывателя
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckShip(Ship aShip)
        {
            if (aShip == null)
                return Warning("Invalid source");
            else
                return true;
        }

        /// <summary>
        /// Нельзя управлять неюзабельными
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckState(Ship aShip)
        {
            if (!aShip.IsOperable)
                return Warning("Not operable");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый контроллер</param>
        public CmdAnnihilation(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Ship tmpShip = ReadShip(aPacket);
            // Основная валидация
            if (!CheckShip(tmpShip))
                return;
            if (!CheckState(tmpShip))
                return;
            if (!CheckTech(tmpShip))
                return;
            if (!CheckTimer(tmpShip))
                return;
            if (!CheckCount(tmpShip))
                return;
            if (!CheckRole(tmpShip, aPacket.Connection.Player))
                return;
            // Отправим команду на исполнение
            Call(tmpShip, aPacket.Connection.Player);
        }
    }
}