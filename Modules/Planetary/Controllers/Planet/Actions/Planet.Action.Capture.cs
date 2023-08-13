/////////////////////////////////////////////////
//
// Захват контроля над планетой
//
// Copyright(c) 2016 UaShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс захвата контроля над планетой
    /// </summary>
    internal class Capture : PlanetaryAccess
    {
        /// <summary>
        /// Интервал тайминга захвата
        /// </summary>
        private int ciTimerInterval => 1000;

        /// <summary>
        /// Скорость захвата планеты
        /// </summary>
        private int ciCaptureSpeed => 50;

        /// <summary>
        /// Потолок для захвата
        /// </summary>
        private int ciCaptureMax => 100000;

        /// <summary>
        /// Таймер захвата планеты
        /// </summary>
        /// <param name="aPlanet">Обрабатываемая планета</param>
        private int OnTimer(TimerObject aPlanet)
        {
            Planet tmpPlanet = (Planet)aPlanet;
            bool tmpIsBattle = tmpPlanet.TimerEnabled(PlanetTimer.Battle);
            // Таймер обновляется если есть корабль захвата или планета имеет контроль
            return ByShip(tmpPlanet, tmpIsBattle)
                || ByPlanet(tmpPlanet, tmpIsBattle) ? ciTimerInterval : 0;
        }

        /// <summary>
        /// Обновление параметров захвата планеты от корабля
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aInBattle">Признак боя</param>
        /// <returns>Признак активного захвата</returns>
        private bool ByShip(Planet aPlanet, bool aInBattle)
        {
            aPlanet.CaptureAttackers = 0;
            aPlanet.CaptureDefenders = 0;
            bool tmpIsCapture = false;
            double tmpCoeff;
            // Переберем всех захватчиков
            for (int tmpI = aPlanet.Ships.Count - 1; tmpI >= 0; tmpI--)
            {
                Ship tmpShip = aPlanet.Ships[tmpI];
                // Пропустим не атакующие
                if (!tmpShip.IsCapture)
                    continue;
                // Определим атакующего при инициации захвата
                if (aPlanet.CapturePlayer == null)
                {
                    if (tmpShip.Owner == aPlanet.Owner)
                        continue;
                    else
                        aPlanet.CapturePlayer = tmpShip.Owner;
                }
                // Если кораблик есть - значит итерацию не прерываем
                tmpIsCapture = true;
                // Определим скорость захвата
                int tmpShipSpeed = tmpShip.TechValue(ShipTech.Capturer);
                // Подсчитаем коэффициент текущего кораблика, атакующие на 50% мощнее
                if (aPlanet.CapturePlayer == tmpShip.Owner)
                {
                    aPlanet.CaptureAttackers++;
                    tmpCoeff = 2 / Math.Pow(2, aPlanet.CaptureAttackers);
                }
                else
                {
                    aPlanet.CaptureDefenders++;
                    tmpCoeff = 1 / Math.Pow(2, aPlanet.CaptureDefenders);
                }
                // Определим итоговое значение захвата, 1 * 50 * 200 * (-0.9 * 1 + 998.9) / 998 = 1000..10000
                tmpCoeff = tmpCoeff * ciCaptureSpeed * tmpShipSpeed * (-0.9 * aPlanet.Level + 998.9) / 998;
                // Для нейтралок скорость в 5 раз быстрее
                if (aPlanet.Owner == null)
                    tmpCoeff *= 5;
                // Обрежем лишнее значение
                if (aPlanet.CapturePlayer == tmpShip.Owner)
                    tmpCoeff = +Math.Min(tmpCoeff, ciCaptureMax - aPlanet.CaptureValue);
                else
                    tmpCoeff = -Math.Min(tmpCoeff, aPlanet.CaptureValue);
                // Установим значение
                aPlanet.CaptureValue += (int)tmpCoeff;
                tmpShip.Count -= (int)Math.Min(tmpShip.Count, tmpCoeff / tmpShipSpeed);
                // Если планета не в бою, отправим значения
                if (!aInBattle)
                {
                    if (tmpShip.Count == 0)
                        Engine.Ships.Action.Relocation.Delete(tmpShip);
                    else
                        Engine.SocketWriter.ShipUpdateHP(tmpShip);
                }
                // Проверим на захват
                if (aPlanet.CaptureValue == ciCaptureMax)
                {
                    Call(aPlanet, tmpShip);
                    break;
                }
            }
            // Обновим циферьки захвата планеты
            if (tmpIsCapture)
                Engine.SocketWriter.PlanetCaptureUpdate(aPlanet);
            // Вернем признак чтобы убить или продлить таймер
            return tmpIsCapture;
        }

        /// <summary>
        /// Обновление параметров захвата планеты от простоя
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aInBattle">Признак боя</param>
        /// <returns>Признак активного захвата</returns>
        private bool ByPlanet(Planet aPlanet, bool aInBattle)
        {
            // В бою контроль не возвращается
            if (aInBattle)
                return true;
            // Уменьшим уровень захвата
            aPlanet.CaptureValue -= Math.Min(aPlanet.CaptureValue, ciCaptureSpeed);
            // Проверим не обнулен ли захват
            if (aPlanet.CaptureValue == 0)
                aPlanet.CapturePlayer = null;
            // Обновим параметры контроля
            Engine.SocketWriter.PlanetCaptureUpdate(aPlanet);
            // Если контроль еще есть, перекинем таймер
            return (aPlanet.CapturePlayer != null);
        }

        /// <summary>
        /// Смена владельца планеты
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aShip">Кораблик, финализирующий захват</param>
        private void Call(Planet aPlanet, Ship aShip)
        {
            // Уберем бонусы противника
            foreach (Ship tmpShip in aPlanet.Ships)
            {
                /* Capture Уберем бонусы противника if (tmpShip.Owner == aShip.Owner)
                    Controller.ShipList.Call(tmpShip, tmpShip.Count);
                else
                    Controller.ShipList.Call(tmpShip, -tmpShip.Count);*/
            }
            // Уберем контроль противника
            Engine.Planets.Action.Control.Call(aPlanet, aPlanet.Owner, false, true);
            // Заменим параметры
            aPlanet.Owner = aShip.Owner;
            aPlanet.CaptureValue = 0;
            aPlanet.Name = aShip.Owner.Name;
            // Добавим свой контроль
            Engine.Planets.Action.Control.Call(aPlanet, aPlanet.Owner, true, true);
            // Отправим сообщение о владельце
            Engine.SocketWriter.PlanetOwnerChanged(aPlanet);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public Capture(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Включение захвата планеты
        /// </summary>
        /// <param name="aShip">Корабль захватчика</param>
        public void Call(Ship aShip)
        {
            // Добавим захват для инвейдера
            if (!aShip.IsCapture)
                return;
            // Если таймер уже есть, то нчиего не делаем
            if (!aShip.Attach.TimerEnabled(PlanetTimer.Capture))
                Engine.Planets.Action.Utils.TimerAdd(aShip.Attach, PlanetTimer.Capture, OnTimer, ciTimerInterval);
        }
    }
}