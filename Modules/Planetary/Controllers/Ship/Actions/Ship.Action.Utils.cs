/////////////////////////////////////////////////
//
// Вспомогательные методы
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс саморемонта стека
    /// </summary>
    internal class ActionUtils : PlanetaryAccess
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый контроллер</param>
        public ActionUtils(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Проверка на возможность влета в тыл
        /// </summary>
        /// <param name="aLanding">Слот посадки</param>
        /// <param name="aOwner">Владелец кораблика</param>
        /// <returns>Разрешение на влет</returns>
        public bool CheckBackZone(Landing aLanding, Player aOwner)
        {
            // Левый слот тыла, быстрая проверка на свой кораблик
            if (!CheckShipSide(aLanding.Prev, ShipMode.Active, out Ship tmpLeft)
                || tmpLeft.Owner.IsRoleFriend(aOwner))
                return true;
            // Правый слот тыла, быстрая проверка на свой кораблик
            if (!CheckShipSide(aLanding.Next, ShipMode.Active, out Ship tmpRight)
                || tmpRight.Owner.IsRoleFriend(aOwner))
                return true;
            // Если крайние кораблики не союзные друг другу, то тыла нет
            return !tmpRight.Owner.IsRoleFriend(tmpLeft.Owner);
        }

        /// <summary>
        /// Поиск соседнего корабля, который имеет свойства блокирующего
        /// </summary>
        /// <param name="aSlot">Слот</param>
        /// <param name="aWantMode">Поиск кораблика в определенном режиме</param>
        /// <param name="aBlocker">Кораблик, который блокирует</param>
        /// <returns>Кораблик является блокирующим</returns>
        public bool CheckShipSide(Landing aSlot, ShipMode aWantMode, out Ship aBlocker)
        {
            aBlocker = aSlot.Ship;
            return (aBlocker != null)
                && (aBlocker.State == ShipState.Available)
                && (aBlocker.Mode == aWantMode);
        }

        /// <summary>
        /// Проверка разрешения приземления на планету
        /// </summary>
        /// <param name="aPlanet">Планета назначения</param>
        /// <param name="aOwner">Владелец кораблика</param>
        /// <param name="aSkipState">Не проверять состояние планетоида</param>
        /// <returns>Разрешение на посадку в указанный слот</returns>
        public bool CheckArrival(Planet aPlanet, Player aOwner, bool aSkipState)
        {
            // Нельзя прилетать на неактивные планеты
            if (!aSkipState)
            {
                if (aPlanet.State != PlanetState.Active)
                    return false;
            }
            // Проверим есть ли свои войска на целевой планете
            if (!aPlanet.ShipCount.TryGetValue(aOwner, out ShipCount tmpCount))
                return true;
            // Если войска есть - проверим на свободный слот
            return (tmpCount.Exist < Engine.Ships.MaxShipCount);
        }

        /// <summary>
        /// Проверка на возможность слета с планеты
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aPlanet">Планета назначения</param>
        /// <returns>Разрешение на вылет</returns>
        public bool CheckDeparture(Ship aShip, Planet aPlanet)
        {
            // Проверка наличия топлива
            if (aShip.Fuel == 0)
                return false;
            // Вылет на ту же планету всегда разрешен
            if (aShip.Planet == aPlanet)
                return true;
            // С ненаселенных слетать можно всегда
            if (aShip.Planet.Type != PlanetType.Earth)
                return true;
            // С населенных - всегда если нет боя
            if (!aShip.Planet.TimerEnabled(PlanetTimer.Battle))
                return true;
            // Если на планете работает потенциал - то слетать можно
            if (aShip.Planet.TimerEnabled(PlanetTimer.LowGravity))
                return true;
            // Иначе если не корвет и не дева - слетать нельзя
            if (!aShip.TechActive(ShipTech.Faster))
                return false;
            // Поиск враждебной военной базы, которая стопорит скоростных
            foreach (Ship tmpShip in aShip.Planet.Ships)
            {
                if (!tmpShip.IsActive)
                    continue;
                if (!tmpShip.TechActive(ShipTech.FasterBlocker))
                    continue;
                if (!tmpShip.Owner.IsRoleFriend(aShip.Owner))
                    return false;
            }
            // Если блокировок нет - слет успешен
            return true;
        }

        /// <summary>
        /// Создание нового кораблика
        /// </summary>
        /// <param name="aType">Тип кораблика</param>
        /// <param name="aCount">Количество</param>
        /// <param name="aOwner">Владелец</param>
        /// <returns>Кораблик</returns>
        public Ship CreateShip(ShipType aType, int aCount, Player aOwner)
        {
            Ship tmpShip = new Ship(aOwner, aType)
            {
                Count = aCount,
                State = ShipState.Disabled
            };
            tmpShip.Fuel = Engine.Ships.Action.Fuel.Capacity(tmpShip);
            tmpShip.HP = tmpShip.TechValue(ShipTech.Hp);
            // Вернем
            return tmpShip;
        }

        /// <summary>
        /// Поиск свободного слота для перелета
        /// </summary>
        /// <param name="aPlanet">Планета назначения</param>
        /// <param name="aOwner">Владелей, для проверки тыла</param>
        /// <param name="aLowOrbit">Возможность использования нижней орбиты</param>
        /// <param name="aRandomSlot">Начинать искать со случайного слота</param>
        /// <param name="aLanding">Найденный слот</param>
        /// <returns>Признак нахождения свободного слота</returns>
        public bool GetSlot(Planet aPlanet, Player aOwner, bool aLowOrbit, bool aRandomSlot, out Landing aLanding)
        {
            if (aRandomSlot)
                aLanding = aPlanet.LandingZone.Random(aLowOrbit);
            else
                aLanding = aPlanet.LandingZone[0];
            // Запомним позицию, с которой начали поиск
            int tmpPosition = aLanding.Position;
            // Переберем все слоты
            do
            {
                if (aLowOrbit)
                    aLanding = aLanding.Prev;
                else
                    aLanding = aLanding.Next;
                // Если слоты кончились и нельзя лететь в нижнюю орбиту
                if (!aLowOrbit && aLanding.IsLowOrbit)
                    continue;
                // Проверим чтобы слот был пустым
                if (aLanding.Ship != null)
                    continue;
                // Для боевого слота проверим тыл
                if (aLanding.IsLowOrbit || (aOwner == null) || CheckBackZone(aLanding, aOwner))
                    return true;
                // Продолжим цикл пока не дойдем до исходного слота
            } while (aLanding.Position != tmpPosition);
            /// Свободного слота нету
            return false;
        }

        /// <summary>
        /// Поиск свободного слота для перелета
        /// </summary>
        /// <param name="aPlanet">Планета назначения</param>
        /// <param name="aShip">Кораблик</param>        
        /// <param name="aRandomSlot">Начинать искать со случайного слота</param>
        /// <param name="aLanding">Найденный слот</param>
        /// <returns>Признак нахождения свободного слота</returns>
        public bool GetSlot(Planet aPlanet, Ship aShip, bool aRandomSlot, out Landing aLanding)
        {
            return GetSlot(aPlanet, aShip.TechActive(ShipTech.BackzoneIntruder) ? null : aShip.Owner,
                                    aShip.TechActive(ShipTech.LowOrbit), aRandomSlot, out aLanding);
        }

        /// <summary>
        /// Определение, является ли кораблик блокирующим
        /// </summary>
        /// <param name="aShip">Кораблик проверки</param>
        /// <param name="aSlot">Соседний слот</param>
        /// <param name="aEnemy">Поиск только противника</param>
        /// <param name="aWantMode">Поиск кораблика в определенном режиме</param>
        /// <param name="aBlocked">Кораблик, который нужно заблокировать</param>
        /// <returns>Кораблик является блокирующим</returns>
        public bool CheckShipBlocker(Ship aShip, Landing aSlot, bool aEnemy, ShipMode aWantMode, out Ship aBlocked)
        {
            aBlocked = null;
            // Корабль должен быть активной военкой, крейсером или дредом
            if (!aShip.TechActive(ShipTech.CornerBlock))
                return false;
            // Проверим указанную сторну от кораблика
            if (!CheckShipSide(aSlot, aWantMode, out aBlocked))
                return false;
            // Корабль должен быть по центру врагом, с края - союзным
            return (!aEnemy == aShip.Owner.IsRoleFriend(aBlocked.Owner));
        }

        /// <summary>
        /// Нанесение урона кораблику
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aDamage">Прилетающий урон</param>
        /// <param name="aExplose">Признак взрыва корабля</param>
        /// <returns>Количество принятого урона</returns>
        public int DealDamage(Ship aShip, int aDamage, bool aExplose = true)
        {
            int tmpResult = 0;
            // Пропуск ненацеленных стволов
            if (aShip == null)
                return tmpResult;
            // Отключение атаки вторым стволом и ретаргет внешних
            if (aShip.Count == 0)
                return tmpResult;
            // Посчитаем количество убитых и общий урон
            int tmpHP = aShip.TechValue(ShipTech.Hp);
            int tmpKilled = Math.Min(aShip.Count, aDamage / tmpHP);
            int tmpDamage = tmpKilled * tmpHP;
            // Учтем итоговый урон, убавим остаточный
            tmpResult += tmpDamage;
            aDamage -= tmpDamage;
            aShip.Count -= tmpKilled;
            aShip.IsChanged = true;
            // Уберем убитые
            if (aExplose)
                aShip.Destructed += tmpKilled;
            // Проверим наличие корабликов
            if (aShip.Count > 0)
            {
                tmpDamage = Math.Min(aShip.HP, aDamage);
                aShip.HP -= tmpDamage;
                tmpResult += aDamage;
                // Замена корабля при хп в ноле
                if (aShip.HP == 0)
                {
                    aShip.HP = tmpHP - (aDamage - tmpDamage);
                    aShip.Count--;
                    // Убитые только если нужно
                    if (aExplose)
                        aShip.Destructed++;
                }
            }
            // Если кораблик убит - обновим данные планеты
            /*if (tmpKilled > 0)
                Engine.Planets.ShipList.Call(aShip, -tmpKilled);*/
            // Обновим параметры корабля, если он удален - взрываем
            if (aExplose)
                aShip.DestroyMode = ShipDestroyMode.Explose;
            // Вернем урон
            return tmpResult;
        }

        /// <summary>
        /// Обработка изменения XP корабликов на планете
        /// </summary>
        /// <param name="aPlanet">Планета обработки</param>
        public void WorkShipHP(Planet aPlanet)
        {
            for (int tmpI = aPlanet.Ships.Count - 1; tmpI >= 0; tmpI--)
            {
                Ship tmpShip = aPlanet.Ships[tmpI];
                if (!tmpShip.IsChanged)
                    continue;
                else
                    tmpShip.IsChanged = false;
                // Если ХП есть - обновим данные
                if (tmpShip.Count > 0)
                {
                    tmpShip.DestroyMode = ShipDestroyMode.None;
                    Engine.Ships.Action.Repair.Check(tmpShip);
                    Engine.SocketWriter.ShipUpdateHP(tmpShip);
                }
                // Удалим с планеты
                else
                    Engine.Ships.Action.Relocation.Delete(tmpShip, ShipDestroyMode.Explose);
            }
        }

        /// <summary>
        /// Добавление таймера
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aTimer">Тип таймера</param>
        /// <param name="aOnTimer">Каллбак срабатывания</param>
        /// <param name="aTime">Время тайминга</param>
        public void TimerAdd(Ship aShip, ShipTimer aTimer, Func<TimerObject, int> aOnTimer, int aTime)
        {
            Engine.Timers.Add(aShip, (int)aTimer, aOnTimer, Engine.SocketWriter.ShipUpdateTimer, aTime);
        }

        /// <summary>
        /// Удаление таймера
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aTimer">Тип таймера</param>
        public void TimerRemove(Ship aShip, ShipTimer aTimer)
        {
            Engine.Timers.Remove(aShip.Timers[(int)aTimer]);
        }
    }
}