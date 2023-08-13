/////////////////////////////////////////////////
//
// Поиск цели для атаки на текущей планете
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
    /// Класс поиска цели для атаки на текущей планете
    /// </summary>
    internal class ActionTargetLocal : PlanetaryAccess
    {
        /// <summary>
        /// Определение приоритетного кораблика
        /// </summary>
        /// <param name="aLeft">Левый кораблик</param>
        /// <param name="aRight">Правый кораблик</param>
        /// <returns>Левый приоритетней</returns>
        private bool ShipPriority(Ship aLeft, Ship aRight)
        {
            // Если нет правого - левый в приоритете
            if (aRight == null)
                return true;
            // Если нет левого - правый в приоритете
            if (aLeft == null)
                return false;
            // Корабли имеют одинаковый тип
            if (aLeft.ShipType == aRight.ShipType)
                return true;
            // У левого больший приоритет
            return aLeft.TechValue(ShipTech.Priority) <= aRight.TechValue(ShipTech.Priority);
        }

        /// <summary>
        /// Получение расстояния между двумя корабликами
        /// </summary>
        /// <param name="aLeft">Левый кораблик</param>
        /// <param name="aRight">Правый кораблик</param>
        /// <returns>Расстояние</returns>
        private int ShipRange(Ship aLeft, Ship aRight)
        {
            // Если нет правого - левый в приоритете
            if ((aRight == null) || (aLeft == null))
                return 0;
            // Расстояние вперед
            int tmpResult = Math.Abs(aLeft.Landing.Position - aRight.Landing.Position);
            // Расстояние назад
            Landing tmpRevert = aLeft.Landing.Planet.LandingZone[-tmpResult, true];
            // Проверим что короче, идти вперед или назад
            if (tmpRevert.Position < tmpResult)
                tmpResult = tmpRevert.Position;
            // Вернем итоговое расстояние
            return tmpResult;
        }

        /// <summary>
        /// Определение ближайшего корабля к текущему
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        /// <param name="aLeft">Левый кораблик</param>
        /// <param name="aRight">Правый кораблик</param>
        /// <returns>Левый ближе</returns>
        private bool ShipNear(Ship aShip, Ship aLeft, Ship aRight)
        {
            // Если нет правого - левый в приоритете
            if (aRight == null)
                return true;
            // Если нет левого - правый в приоритете
            if (aLeft == null)
                return false;
            // Расстояние до левого ближе чем до правого
            return ShipRange(aShip, aLeft) <= ShipRange(aShip, aRight);
        }

        /// <summary>
        /// Поиск противника для атаки
        /// </summary>
        /// <param name="aShip">Кораблик на основе которого идет поиск</param>
        /// <param name="aIgnoreFriend">Игнорировать союзника для прострела</param>
        /// <param name="aLeft">Поиск слева или справа от кораблика</param>
        /// <param name="aOneStep">Поиск только в соседнем слоте</param>
        /// <param name="aOwner">Владелец кораблика, ищущего цель</param>
        /// <returns>Цель для атаки</returns>
        private Ship TargetShip(Ship aShip, bool aIgnoreFriend, bool aLeft, bool aOneStep, Player aOwner)
        {
            Landing tmpSlot = aShip.Landing;
            do
            {
                // Сдвинем слот
                tmpSlot = aLeft ? tmpSlot.Prev : tmpSlot.Next;
                // Берем кораблик из слота
                if (tmpSlot.Ship == null)
                {
                    // Нужен только соседний кораблик
                    if (aOneStep)
                        break;
                    else
                        continue;
                }
                // Ищем противника
                if (tmpSlot.Ship.Owner.IsRoleFriend(aOwner))
                {
                    if ((!aIgnoreFriend) && (tmpSlot.Ship.IsActive))
                        break;
                    else
                        continue;
                }
                // Если противник активен - стреляем по нему
                if ((tmpSlot.Ship.State != ShipState.Disabled)
                    && (tmpSlot.Ship.Mode == ShipMode.Active))
                {
                    return tmpSlot.Ship;
                }
                // Если не активен, но это последний стек противника, запомним его 
                if ((tmpSlot.Ship.Planet.ShipCount[tmpSlot.Ship.Owner].Active == 0)
                    && (tmpSlot.Ship.TimerValue(ShipTimer.FlightGlobal) == 0))
                {
                    if (tmpSlot.Ship.Mode == ShipMode.Offline)
                        Engine.Ships.Action.StandUp.Call(tmpSlot.Ship);
                    return tmpSlot.Ship;
                }
            } while (tmpSlot != aShip.Landing);
            // Если найден последний кораблик в стеке - поднимаем его
            // Нет кораблика для прицела
            return null;
        }

        /// <summary>
        /// Прицел в лоб
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        /// <param name="aRightShip">Правый кораблик</param>
        /// <param name="aLeftShip">Левый кораблик</param>
        /// <returns>Найденная цель</returns>
        private Ship TargetingToHead(Ship aShip, Ship aRightShip, Ship aLeftShip)
        {
            if (ShipNear(aShip, aLeftShip, aRightShip))
                return aLeftShip;
            else
                return aRightShip;
        }

        /// <summary>
        /// Прицел в лоб в два края
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        /// <param name="aRightShip">Правый кораблик</param>
        /// <param name="aLeftShip">Левый кораблик</param>
        private void TargetingToCorners(Ship aShip, Ship aRightShip, Ship aLeftShip)
        {
            // Применим изменения
            aShip.TargetRight = aRightShip;
            aShip.TargetLeft = aLeftShip;
            // Переключение батареи на другую если нет цели
            if (aLeftShip == null)
                aShip.TargetLeft = aRightShip;
            if (aRightShip == null)
                aShip.TargetRight = aLeftShip;
        }

        /// <summary>
        /// Прицел в лоб с прострелом через цель
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        /// <param name="aRightShip">Правый кораблик</param>
        /// <param name="aLeftShip">Левый кораблик</param>
        /// <param name="aRightShipInner">Правый кораблик через один</param>
        /// <param name="aLeftShipInner">Левый кораблик через один</param>
        private void TargetingToInner(Ship aShip, Ship aRightShip, Ship aLeftShip, Ship aRightShipInner, Ship aLeftShipInner)
        {
            bool tmpUsePriority = false;
            // Сперва цель второй прострел слева
            if ((aLeftShipInner != null) && (aLeftShipInner.TechActive(ShipTech.OvershotTarget)))
            {
                if (ShipPriority(aLeftShipInner, aLeftShip))
                {
                    aLeftShip = aLeftShipInner;
                    tmpUsePriority = true;
                }
            }
            // Теперь смотрим приоритет прострела правой цели
            if ((aRightShipInner != null) && (aRightShipInner.TechActive(ShipTech.OvershotTarget)))
            {
                if (ShipPriority(aRightShipInner, aRightShip))
                {
                    aRightShip = aRightShipInner;
                    tmpUsePriority = true;
                }
            }
            // Если нет поиска по приоритету - проверим не на равном ли они расстоянии
            if (!tmpUsePriority)
                tmpUsePriority = ShipRange(aLeftShip, aRightShip) == 1;
            // Если учитывается по приоритету
            if (tmpUsePriority)
            {
                if (ShipPriority(aLeftShip, aRightShip))
                    aShip.TargetCenter = aLeftShip;
                else
                    aShip.TargetCenter = aRightShip;
            }
            // Если учет по растоянию
            else
            {
                if (ShipNear(aShip, aLeftShip, aRightShip))
                    aShip.TargetCenter = aLeftShip;
                else
                    aShip.TargetCenter = aRightShip;
            }
        }

        /// <summary>
        /// Прицел в две ближние цели
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        /// <param name="aRightShip">Правый кораблик</param>
        /// <param name="aLeftShip">Левый кораблик</param>
        /// <param name="aRightShipInner">Правый кораблик через один</param>
        /// <param name="aLeftShipInner">Левый кораблик через один</param>
        private void TargetingToDouble(Ship aShip, Ship aRightShip, Ship aLeftShip, Ship aRightShipInner, Ship aLeftShipInner)
        {
            // Перевод второго орудия на прострел
            if (ShipNear(aShip, aLeftShipInner, aRightShip))
                aRightShip = aLeftShipInner;
            else
            if (ShipNear(aShip, aRightShipInner, aLeftShip))
                aLeftShip = aRightShipInner;
            // Переключение батареи на другую если нет цели
            if (aRightShip == null)
                aRightShip = aLeftShip;
            if (aLeftShip == null)
                aLeftShip = aRightShip;
            // Применим изменения
            aShip.TargetRight = aRightShip;
            aShip.TargetLeft = aLeftShip;
        }

        /// <summary>
        /// Прицеливание прострельным орудием
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        private void WeaponOvershot(Ship aShip)
        {
            Ship tmpShipRight = TargetShip(aShip, false, true, false, aShip.Owner);
            Ship tmpShipLeft = TargetShip(aShip, false, false, false, aShip.Owner);
            Ship tmpShipInnerRight = null;
            Ship tmpShipInnerLeft = null;
            // Проверим прострел справа
            if ((tmpShipRight != null) && (!tmpShipRight.TechActive(ShipTech.OvershotBlocker)))
            {
                if (ShipRange(aShip, tmpShipRight) == 1)
                    tmpShipInnerRight = TargetShip(tmpShipRight, false, true, true, aShip.Owner);
            }
            // Проверим прострел слева
            if ((tmpShipLeft != null) && (!tmpShipLeft.TechActive(ShipTech.OvershotBlocker)))
            {
                if (ShipRange(aShip, tmpShipLeft) == 1)
                    tmpShipInnerLeft = TargetShip(tmpShipLeft, false, false, true, aShip.Owner);
            }
            // Определим нужную цель
            TargetingToInner(aShip, tmpShipRight, tmpShipLeft, tmpShipInnerRight, tmpShipInnerLeft);
        }

        /// <summary>
        /// Прицеливание лазерами и ракетами
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        private Ship WeaponOverFriends(Ship aShip)
        {
            Ship tmpShipRight = TargetShip(aShip, true, true, false, aShip.Owner);
            Ship tmpShipLeft = TargetShip(aShip, true, false, false, aShip.Owner);
            // Прицелим в ближнего
            return TargetingToHead(aShip, tmpShipRight, tmpShipLeft);
        }

        /// <summary>
        /// Прицеливание двойными лазерами
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        private void WeaponDoubleLaser(Ship aShip)
        {
            Ship tmpShipRight = TargetShip(aShip, true, true, false, aShip.Owner);
            Ship tmpShipLeft = TargetShip(aShip, true, false, false, aShip.Owner);
            Ship tmpShipInnerRight = null;
            Ship tmpShipInnerLeft = null;
            // Проверим прострел
            if (tmpShipRight != null)
                tmpShipInnerRight = TargetShip(tmpShipRight, true, true, false, aShip.Owner);
            if (tmpShipLeft != null)
                tmpShipInnerLeft = TargetShip(tmpShipLeft, true, false, false, aShip.Owner);
            // Определим нужную цель
            TargetingToDouble(aShip, tmpShipRight, tmpShipLeft, tmpShipInnerRight, tmpShipInnerLeft);
        }

        /// <summary>
        /// Прицеливание двойными пулями
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        private void WeaponDoubleBullet(Ship aShip)
        {
            Ship tmpShipRight = TargetShip(aShip, false, true, false, aShip.Owner);
            Ship tmpShipLeft = TargetShip(aShip, false, false, false, aShip.Owner);
            // Определим нужную цель, корвет имеет прострел
            TargetingToCorners(aShip, tmpShipRight, tmpShipLeft);
        }

        /// <summary>
        /// Прицеливание одинарной пулей
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        private void WeaponBullet(Ship aShip)
        {
            Ship tmpShipRight = TargetShip(aShip, false, true, false, aShip.Owner);
            Ship tmpShipLeft = TargetShip(aShip, false, false, false, aShip.Owner);
            // Определим нужную цель, корвет имеет прострел
            aShip.TargetCenter = TargetingToHead(aShip, tmpShipRight, tmpShipLeft);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public ActionTargetLocal(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        ///  Базовое выполнение прицеливания
        /// </summary>
        /// <param name="aShip">Текущий кораблик</param>
        public bool Call(Ship aShip)
        {
            // Сохраним текущий прицел
            Ship tmpTargetLeft = aShip.TargetLeft;
            Ship tmpTargetRight = aShip.TargetRight;
            Ship tmpTargetCenter = aShip.TargetCenter;
            Ship tmpTargetRocket = aShip.TargetRocket;
            // Поищем цели на планете
            if (aShip.IsActive)
            {
                // Прострел через противника не зависит от орудия
                if (aShip.TechActive(ShipTech.WeaponOvershot))
                    WeaponOvershot(aShip);
                // Лазер в крайнего через своих
                if (aShip.TechActive(ShipTech.WeaponLaser))
                    aShip.TargetCenter = WeaponOverFriends(aShip);
                // Ракета стреляют в крайнего через своих
                if (aShip.TechActive(ShipTech.WeaponRocket))
                    aShip.TargetRocket = WeaponOverFriends(aShip);
                // Двойной простреливающий лазер
                if (aShip.TechActive(ShipTech.WeaponDoubleLaser))
                    WeaponDoubleLaser(aShip);
                // Двойной патрон
                if (aShip.TechActive(ShipTech.WeaponDoubleBullet))
                    WeaponDoubleBullet(aShip);
                // Транспорт и прочие стреляют одним патроном в лоб
                if (aShip.TechActive(ShipTech.WeaponBullet))
                    WeaponBullet(aShip);
            }
            else
                aShip.IsTargeted = false;
            // Отправим смену состояния
            if (tmpTargetLeft != aShip.TargetLeft)
                Engine.SocketWriter.ShipRetarget(aShip, aShip.TargetLeft, ShipWeaponType.Left);
            if (tmpTargetRight != aShip.TargetRight)
                Engine.SocketWriter.ShipRetarget(aShip, aShip.TargetRight, ShipWeaponType.Right);
            if (tmpTargetCenter != aShip.TargetCenter)
                Engine.SocketWriter.ShipRetarget(aShip, aShip.TargetCenter, ShipWeaponType.Center);
            if (tmpTargetRocket != aShip.TargetRocket)
                Engine.SocketWriter.ShipRetarget(aShip, aShip.TargetRocket, ShipWeaponType.Rocket);
            // Вернем признак прицела
            return aShip.IsTargeted;
        }
    }
}