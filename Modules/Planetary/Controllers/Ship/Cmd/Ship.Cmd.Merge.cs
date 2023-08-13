/////////////////////////////////////////////////
//
// Команда игрока - объединение корабликов
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
    /// Класс объединения корабликов
    /// </summary>
    internal class CmdMerge : PlanetaryCommand
    {
        /// <summary>
        /// Объединение корабликов 
        /// </summary>
        /// <param name="aSource">Первый кораблик</param>
        /// <param name="aDestination">Второй кораблик</param>
        /// <param name="aCount">Количество для объединения</param>
        private void Call(Ship aSource, Ship aDestination, int aCount)
        {
            // Перекинем количество
            aSource.Count -= aCount - aDestination.Count;
            aDestination.Count = aCount;
            // Перекинем хп
            aCount = Math.Min(aSource.HP + aDestination.HP, aSource.TechValue(ShipTech.Hp));
            aSource.HP -= aCount - aDestination.HP;
            aDestination.HP = aCount;
            // И если в источнике кораблей больше нет - прибьем объект
            if (aSource.Count == 0)
                Engine.Ships.Action.Relocation.Delete(aSource);
            else
                Engine.SocketWriter.ShipUpdateHP(aSource);
            // Отправить сообщение
            Engine.SocketWriter.ShipUpdateHP(aDestination);
        }

        /// <summary>
        /// Определение количества стеков для переброски
        /// </summary>
        /// <param name="aSource">Источник</param>
        /// <param name="aTarget">Цель</param>
        /// <param name="aCount">Заданное количество для переброски</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckCount(Ship aSource, Ship aTarget, ref int aCount)
        {
            if ((aCount <= 0) || (aCount > aSource.Count))
                aCount = aSource.Count;
            // Перекинем сами кораблики
            aCount = Math.Min(aCount + aTarget.Count, aSource.TechValue(ShipTech.Count));
            // Проверим что есть что перекидывать
            if (aCount == aTarget.Count)
                return Warning("No count to merge");
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
        /// Исходный кораблик не в простое
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckSourceState(Ship aShip)
        {
            if (!aShip.IsOperable)
                return Warning("Source not operable");
            else
                return true;
        }

        /// <summary>
        /// Конечный кораблик не в простое
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckTargetState(Ship aShip)
        {
            if (!aShip.IsOperable)
                return Warning("Target not operable");
            else
                return true;
        }

        /// <summary>
        /// Нельзя объеденять на разных планетах
        /// </summary>
        /// <param name="aSource">Источник</param>
        /// <param name="aTarget">Назначение</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckRange(Ship aSource, Ship aTarget)
        {
            if (aSource.Landing.Planet != aTarget.Landing.Planet)
                return Warning("Long range");
            else
                return true;
        }

        /// <summary>
        /// Нельзя объеденять флот разных владельцев
        /// </summary>
        /// <param name="aSource">Источник</param>
        /// <param name="aTarget">Назначение</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckOwner(Ship aSource, Ship aTarget)
        {
            if (aSource.Owner != aTarget.Owner)
                return Warning("Missed owner");
            else
                return true;
        }

        /// <summary>
        /// Нельзя объединять кораблик сам с собой
        /// </summary>
        /// <param name="aSource">Источник</param>
        /// <param name="aTarget">Назначение</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckSelf(Ship aSource, Ship aTarget)
        {
            if (aSource == aTarget)
                return Warning("Himself");
            else
                return true;
        }

        /// <summary>
        /// Объеденяемые корабли должны быть одной расы, иметь одного владельца и быть не полной пачкой
        /// </summary>
        /// <param name="aSource">Источник</param>
        /// <param name="aTarget">Назначение</param>
        /// <returns>Разрешение операции</returns>
        private bool CheckType(Ship aSource, Ship aTarget)
        {
            if (aSource.ShipType != aTarget.ShipType)
                return Warning("Wrong ship type");
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
        public CmdMerge(PlanetaryEngine aEngine) : base(aEngine)
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
            int tmpCount = aPacket.ReadInt();
            // Основная валидация
            if (!CheckSource(tmpSource))
                return;
            if (!CheckTarget(tmpTarget))
                return;
            if (!CheckType(tmpSource, tmpTarget))
                return;
            if (!CheckSelf(tmpSource, tmpTarget))
                return;
            if (!CheckOwner(tmpSource, tmpTarget))
                return;
            if (!CheckRange(tmpSource, tmpTarget))
                return;
            if (!CheckSourceState(tmpSource))
                return;
            if (!CheckTargetState(tmpTarget))
                return;
            if (!CheckRole(tmpSource, aPacket.Connection.Player))
                return;
            if (!CheckCount(tmpSource, tmpTarget, ref tmpCount))
                return;
            // Отправим команду на исполнение
            Call(tmpSource, tmpTarget, tmpCount);
        }
    }
}