/////////////////////////////////////////////////
//
// Обработка планетарного боя
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System.Collections.Generic;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.ShipSpace
{
    /// <summary>
    /// Класс обработки планетарного боя
    /// </summary>
    internal class ActionBattle : PlanetaryAccess
    {
        /// <summary>
        /// Время пересчета боя
        /// </summary>
        private const int ciTimerInterval = 1000;

        /// <summary>
        /// Поправка урона бонусами
        /// </summary>
        /// <param name="aShip">Атакующий кораблик</param>
        /// <param name="aDamage">Предпоалагаемый дамаг</param>
        /// <returns>Сбалансированный дамаг</returns>
        private int CalcDamage(Ship aShip, int aDamage)
        {
#if DEBUG1
            return 10;
#else
            // Стационарки стреляют константным дамагом
            if (!aShip.TechActive(ShipTech.Stationary))
                return System.Math.Min(2000, aShip.Count * aDamage);
            else
                return aDamage;
#endif
        }

        /// <summary>
        /// Переприцел корабля в зависимости от его способностей
        /// </summary>
        /// <param name="aShip">Кораблик</param>
        /// <param name="aExternal">Признак поиска нацеленной арты</param>
        /// <returns>Успешность прицеливания</returns>
        private bool Retarget(Ship aShip, bool aExternal)
        {
            // Пропуск внутренних слотов
            if (aShip.Landing.IsLowOrbit)
                return false;
            // Пропуск дев нацеленных на другую планету
            if (aShip.IsRangeAttach != aExternal)
            {
                // Автоназначенные сбрасывают таргет для поиска цели на своей планете
                if (aShip.IsAutoTarget)
                    Engine.Ships.Action.Attach.Call(aShip, null, true);
                else
                    return false;
            }
            // Закэшируем теху
            bool tmpRanged = aShip.TechActive(ShipTech.WeaponRocket);
            // Если проверка внешних - то проверим существует ли аттач
            if (aExternal && tmpRanged)
                Engine.Ships.Action.TargetMarker.Highlight(aShip, aShip.Attach, aShip.IsAutoTarget);
            else
            {
                // Попробуем найти локальную цель
                if (Engine.Ships.Action.TargetLocal.Call(aShip))
                    return true;
                else if (tmpRanged)
                    Engine.Ships.Action.TargetMarker.SearchTargets(aShip);
            }
            // Вернем итоговый флаг успешного прицеливания
            return false;
        }

        /// <summary>
        /// Переприцел коараблей на планете
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aExternal">Нацеленные с другой планеты или локальные</param>
        /// <returns>Успешность прицеливания</returns>
        private bool Retarget(Planet aPlanet, bool aExternal)
        {
            List<Ship> tmpList;
            bool tmpTargeted = false;
            // Выберем список
            if (aExternal)
                tmpList = aPlanet.RangeAttackers;
            else
                tmpList = aPlanet.Ships;
            // Переприцелим корабли, при прицеле могут удалить из списка
            for (int tmpI = tmpList.Count - 1; tmpI >= 0; tmpI--)
            {
                if (Retarget(tmpList[tmpI], aExternal))
                    tmpTargeted = true;
            }
            // Прицелим свободные ранжевики
            if (aExternal)
                Engine.Ships.Action.TargetMarker.SearchRangers(aPlanet);
            else
                Retarget(aPlanet, true);
            // Вернем флаг прицеливания
            return tmpTargeted;
        }

        /// <summary>
        /// Нанесение урона противника
        /// </summary>
        /// <param name="aShip">Атакующий кораблик</param>
        private void AttackTarget(Ship aShip)
        {
            // Двойные пули
            int tmpDmg = aShip.TechValue(ShipTech.WeaponDoubleBullet);
            if (tmpDmg > 0)
            {
                tmpDmg = CalcDamage(aShip, tmpDmg);
                Engine.Ships.Action.Utils.DealDamage(aShip.TargetLeft, tmpDmg);
                Engine.Ships.Action.Utils.DealDamage(aShip.TargetRight, tmpDmg);
            }
            // Двойные лазеры
            tmpDmg = aShip.TechValue(ShipTech.WeaponDoubleLaser);
            if (tmpDmg > 0)
            {
                tmpDmg = CalcDamage(aShip, tmpDmg);
                Engine.Ships.Action.Utils.DealDamage(aShip.TargetLeft, tmpDmg);
                Engine.Ships.Action.Utils.DealDamage(aShip.TargetRight, tmpDmg);
            }
            // Одиночный патрон
            tmpDmg = aShip.TechValue(ShipTech.WeaponBullet);
            if (tmpDmg > 0)
            {
                tmpDmg = CalcDamage(aShip, tmpDmg);
                Engine.Ships.Action.Utils.DealDamage(aShip.TargetCenter, tmpDmg);
            }
            // Прострелочный патрон
            tmpDmg = aShip.TechValue(ShipTech.WeaponOvershot);
            if (tmpDmg > 0)
            {
                tmpDmg = CalcDamage(aShip, tmpDmg);
                Engine.Ships.Action.Utils.DealDamage(aShip.TargetCenter, tmpDmg);
            }
            // Одиночный лазер
            tmpDmg = aShip.TechValue(ShipTech.WeaponLaser);
            if (tmpDmg > 0)
            {
                tmpDmg = CalcDamage(aShip, tmpDmg);
                Engine.Ships.Action.Utils.DealDamage(aShip.TargetCenter, tmpDmg);
            }
            // Ракета
            tmpDmg = aShip.TechValue(ShipTech.WeaponRocket);
            if (tmpDmg > 0)
            {
                tmpDmg = CalcDamage(aShip, tmpDmg);
                Engine.Ships.Action.Utils.DealDamage(aShip.TargetRocket, tmpDmg);
            }
        }

        /// <summary>
        /// Таймер боя
        /// </summary>
        /// <param name="aPlanet">Обрабатываемая планета</param>
        /// <returns>Время продления таймера</returns>
        private int OnTimer(TimerObject aPlanet)
        {
            Planet tmpPlanet = (Planet)aPlanet;
            bool tmpInBattle = false;
            // Атака локальных корабликов
            foreach (Ship tmpShip in tmpPlanet.Ships)
            {
                // Приаттаченные к планете обсчитываются отдельно
                if ((tmpShip.IsTargeted) && (!tmpShip.IsRangeAttach))
                {
                    tmpInBattle = true;
                    AttackTarget(tmpShip);
                }
            }
            // Если есть прицел - прокрутим атаку внешних корабликов
            if (tmpInBattle)
            {
                foreach (Ship tmpShip in tmpPlanet.RangeAttackers)
                {
                    if (tmpShip.IsTargeted)
                        AttackTarget(tmpShip);
                }
            }
            // Усредним количество привязанных корабликов
            Engine.Ships.Action.Hypodispersion.Call(tmpPlanet);
            // Отправим сообщение о смене ХП нуждающимся корабликам
            Engine.Ships.Action.Utils.WorkShipHP(tmpPlanet);
            // Вернем результат
            return tmpInBattle ? ciTimerInterval : 0;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый контроллер</param>
        public ActionBattle(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Определение возможности включить бой
        /// </summary>
        /// <param name="aShip">Анализируемый кораблик</param>
        public bool Call(Ship aShip)
        {
            // Если нет прицела - сразу выходим
            if (!Retarget(aShip.Planet, false))
                return false;
            // Если боя нет - запустим и сразу прокрутим урон
            if (!aShip.Planet.TimerEnabled(PlanetTimer.Battle))
                Engine.Planets.Action.Utils.TimerAdd(aShip.Planet, PlanetTimer.Battle, OnTimer, OnTimer(aShip.Planet));
            // Если бой есть или запущен - всегда вернем успех
            return true;
        }
    }
}