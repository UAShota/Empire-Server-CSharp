/////////////////////////////////////////////////
//
// Навык разборки корабликов на запчасти
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
    /// Класс обработки разборки корабликов на запчасти
    /// </summary>
    internal class CmdConstructor : PlanetaryCommand
    {
        /// <summary>
        /// Таймер скилла
        /// </summary>
        /// <param name="aShip">Инициирующий кораблик</param>
        private int OnTimer(TimerObject aShip)
        {
            return 0;
        }

        /// <summary>
        /// Разбор вражеского кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aMount">Количество структуры разбора</param>
        private void ConstructEnemy(Ship aShip, ref int aMount)
        {
            if ((aMount == 0) || (!aShip.TechActive(ShipTech.SkillConstructorEnemy)))
                return;
            // Если есть элитка доберем значение с других вражеских корабликов
            foreach (Ship tmpShip in aShip.Planet.Ships)
            {
                // Нельзя разбирать стационарки
                // Нельзя разбирать неразборные
                if (tmpShip.TechActive(ShipTech.SolidBody))
                    continue;
                // Нельзя разбирать дружеские
                if (tmpShip.Owner.IsRoleFriend(aShip.Owner))
                    continue;
                // Определим количество структуры
                aMount -= Engine.Ships.Action.Utils.DealDamage(tmpShip, aMount, false);
                if (aMount == 0)
                    break;
            }
        }

        /// <summary>
        /// Починка дружественного кораблика
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aMount">Количество структуры починки</param>
        private void RepairFriends(Ship aShip, ref int aMount)
        {
            if ((aMount == 0) || (!aShip.TechActive(ShipTech.SkillConstructorFriend)))
                return;
            // Если есть элитка то починим дружественные корабликы
            foreach (Ship tmpShip in aShip.Planet.Ships)
            {
                // Пропускаем себя
                if (tmpShip == aShip)
                    continue;
                if (!tmpShip.Owner.IsRoleFriend(aShip.Owner))
                    continue;
                // Определим количество структуры
                aMount -= Engine.Ships.Action.Repair.Call(tmpShip, aMount);
                if (aMount == 0)
                    break;
            }
        }

        /// <summary>
        /// Выполнение скилла разбора
        /// </summary>
        /// <param name="aShip">Истоник</param>
        /// <param name="aTarget">Цель</param>
        private void Call(Ship aShip, Ship aTarget)
        {
            // Количество структуры для сбора
            int tmpCount = aShip.TechValue(ShipTech.SkillConstructor);
            int tmpDamage = tmpCount - Engine.Ships.Action.Utils.DealDamage(aTarget, tmpCount, false);
            // Проверим элитку дополнительного разбора
            ConstructEnemy(aShip, ref tmpDamage);
            // Количество структуры для починки
            tmpCount -= tmpDamage;
            tmpCount -= Engine.Ships.Action.Repair.Call(aShip, tmpCount);
            // Проверим элитку дополнительной починки
            RepairFriends(aShip, ref tmpCount);
            // Обновим ХП на планете
            Engine.Ships.Action.Utils.WorkShipHP(aShip.Planet);
            // Отправим откат навыка
            Engine.Ships.Action.Utils.TimerAdd(aShip, ShipTimer.Constructor, OnTimer, aShip.TechCooldown(ShipTech.SkillConstructor));
        }

        /// <summary>
        /// Проверка активности таймера скилла
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTimer(Ship aShip)
        {
            if (!aShip.TimerEnabled(ShipTimer.Construction))
                return Warning("Timer already active");
            else
                return true;
        }

        /// <summary>
        /// Проверка наличия скилла
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTech(Ship aShip)
        {
            if (!aShip.TechActive(ShipTech.SkillConstructor))
                return Warning("No skill");
            else
                return true;
        }

        /// <summary>
        /// Проверка ролей
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlayer">Управляющий игрок</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRole(Ship aShip, Player aPlayer)
        {
            if (!aShip.Owner.IsRoleFriend(aPlayer))
                return Warning("Role");
            else
                return true;
        }

        /// <summary>
        /// Проверка на возможность разбора
        /// </summary>
        /// <param name="aTarget">Корабль цель</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckSolid(Ship aTarget)
        {
            if (aTarget.TechActive(ShipTech.SolidBody))
                return Warning("SolidBody");
            else
                return true;
        }

        /// <summary>
        /// Проверка планеты действия скилла
        /// </summary>
        /// <param name="aSource">Корабль источник</param>
        /// <param name="aTarget">Корабль цель</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckPlanet(Ship aSource, Ship aTarget)
        {
            if (aSource.Planet != aTarget.Planet)
                return Warning("Planets not equal");
            else
                return true;
        }

        /// <summary>
        /// Проверка вызывателя
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckSource(Ship aShip)
        {
            if (aShip == null)
                return Warning("Invalid source");
            else
                return true;
        }

        /// <summary>
        /// Проверка приемника
        /// </summary>
        /// <param name="aShip">Приемник</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTarget(Ship aShip)
        {
            if (aShip == null)
                return Warning("Invalid target");
            else
                return true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public CmdConstructor(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Команда обработки пакета
        /// </summary>
        /// <param name="aPacket">Пакет обработки</param>
        public override void Process(SocketPacket aPacket)
        {
            Ship tmpSource = ReadShip(aPacket);
            Ship tmpTarget = ReadShip(aPacket);
            // Основная валидация
            if (!CheckSource(tmpSource))
                return;
            if (!CheckTarget(tmpTarget))
                return;
            if (!CheckPlanet(tmpSource, tmpTarget))
                return;
            if (!CheckSolid(tmpTarget))
                return;
            if (!CheckRole(tmpSource, aPacket.Connection.Player))
                return;
            if (!CheckTech(tmpSource))
                return;
            if (!CheckTimer(tmpSource))
                return;
            // Отправим команду на исполнение
            Call(tmpSource, tmpTarget);
        }
    }
}