/////////////////////////////////////////////////
//
// Обработка червоточин
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс обработки черных дыр
    /// </summary>
    internal class WormHole : PlanetaryAccess
    {
        /// <summary>
        /// Минимальное расстояние между открываемыми ЧТ
        /// </summary>
        private int ciMinHoleRange => 3;

        /// <summary>
        /// Количество планет для открытия очередной пары ЧТ
        /// </summary>
        private int ciSmallHoleRange => 50;

        /// <summary>
        /// Количество планет для открытия очередной пары БЧТ
        /// </summary>
        private int ciBigHoleRange => 1000;

        /// <summary>
        /// Количество БЧТ в системе
        /// </summary>
        private int fBigHolesCount { get; set; }

        /// <summary>
        /// Количество ЧТ в системе
        /// </summary>
        private int fSmallHolesCount { get; set; }

        /// <summary>
        /// Список доступных ЧТ в системе
        /// </summary>
        private List<Planet> fWormholesList { get; set; }

        /// <summary>
        /// Проверка расстояния между открываемыми чт
        /// </summary>
        /// <param name="aIn">Вход</param>
        /// <param name="aOut">Выход</param>
        /// <returns>Разрешение на открытие</returns>
        private bool CheckRange(Planet aIn, Planet aOut)
        {
            return (Math.Abs(aIn.PosX - aOut.PosX) > ciMinHoleRange)
                && (Math.Abs(aIn.PosY - aOut.PosY) > ciMinHoleRange);
        }

        /// <summary>
        /// Поиск пары для чт
        /// </summary>
        /// <param name="aRandom">Объект случайности</param>
        /// <param name="aBigHole">Признак БЧТ</param>
        /// <param name="aPlanet">Вход ЧТ, если не указан - будет создан</param>
        private void ActivatePair(Random aRandom, bool aBigHole, ref Planet aPlanet)
        {
            Planet tmpPlanet = null;
            int tmpIndex = 0;
            // Найдем конец чт, не должно быть ближе трех квадратов
            do
            {
                tmpIndex = aRandom.Next(0, fWormholesList.Count - 1);
                tmpPlanet = fWormholesList[tmpIndex];
            }
            while ((aPlanet != null) && !CheckRange(aPlanet, tmpPlanet));
            // Удалим из свободных
            fWormholesList.RemoveAt(tmpIndex);
            // Определим БЧТ
            SetHoleEdge(tmpPlanet, true, aBigHole);
            // Выставим параметры
            tmpPlanet.State = PlanetState.Activation;
            // Для входа планеты задаем таймер
            if (aPlanet == null)
            {
                tmpIndex = (int)Math.Round(Engine.TimeWormholeOpen / 1.5);
                Engine.Planets.Action.Utils.TimerAdd(tmpPlanet, PlanetTimer.Activity, OnTimer, aRandom.Next(tmpIndex, Engine.TimeWormholeOpen));
                aPlanet = tmpPlanet;
            }
            else
            {
                /*Engine.Planets.Action.Utils.TimerAdd(tmpPlanet, PlanetTimer.Activity, OnTimer, aPlanet.TimerValue(PlanetTimer.Activity));*/
                Engine.Planets.Action.Portal.Open(aPlanet, tmpPlanet, null, -1, false);
                aPlanet = null;
            }
            // Отправим сообщение о смене состояния
            Engine.SocketWriter.PlanetStateUpdate(tmpPlanet, null);
        }

        /// <summary>
        /// Установка свойства соседа БЧТ
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aEnable">Активация или деактивация</param>
        /// <param name="aBigHole">Обработка БЧТ</param>
        private void SetHoleEdge(Planet aPlanet, bool aEnable, bool aBigHole)
        {
            // Проставим связи
            if (aBigHole)
            {
                foreach (Planet tmpLink in aPlanet.Links)
                    tmpLink.IsBigEdge = aEnable;
                // Сменим тип
                if (aEnable)
                {
                    aPlanet.Mode = PlanetMode.Big;
                    fBigHolesCount++;
                }
                else
                {
                    aPlanet.Mode = PlanetMode.Normal;
                    fBigHolesCount--;
                }
            }
            else if (aEnable)
                fSmallHolesCount++;
            else
                fSmallHolesCount--;
            // При удалении поместим планету в список доступных чт
            if (!aEnable)
                fWormholesList.Add(aPlanet);
        }

        /// <summary>
        /// Активация червоточины
        /// </summary>
        /// <param name="aPlanet">Червоточина</param>
        /// <returns>Время активности</returns>
        private int Activate(Planet aPlanet)
        {
            aPlanet.State = PlanetState.Active;
            return Engine.TimeWormholeActive;
        }

        /// <summary>
        /// Деактивация червоточины
        /// </summary>
        /// <param name="aPlanet">Червоточина</param>
        /// <returns>Время активности</returns>
        private int Deactivate(Planet aPlanet)
        {
            // Раскидаем корабли с орбиты
            Engine.Ships.Action.Relocation.Escape(aPlanet);
            // Выставим параметры
            SetHoleEdge(aPlanet, false, aPlanet.IsBigHole);
            // Выключаем статус
            aPlanet.State = PlanetState.Inactive;
            // Закроем портал
            if (aPlanet.Portal != null)
                Engine.Planets.Action.Portal.Close(aPlanet.Portal, true);
            // Убьем текущий таймер
            return 0;
        }

        /// <summary>
        /// Каллбак события
        /// </summary>
        /// <param name="aPlanet">Планета срабатывания</param>
        /// <returns>Время продления</returns>
        private int OnTimer(TimerObject aPlanet)
        {
            Planet tmpPlanet = (Planet)aPlanet;
            int tmpTime;
            // Выключим активную ЧТ или включим активирующуюся
            if (tmpPlanet.State == PlanetState.Activation)
                tmpTime = Activate(tmpPlanet);
            else
                tmpTime = Deactivate(tmpPlanet);
            // Отправим сообщение о смене состояния
            Engine.SocketWriter.PlanetStateUpdate(tmpPlanet, null);
            // Вернем время продления
            return tmpTime;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public WormHole(PlanetaryEngine aEngine) : base(aEngine)
        {
            fWormholesList = new List<Planet>();
        }

        /// <summary>
        /// Активация недостающих ЧТ
        /// </summary>
        public void Reactivate()
        {
            return;
            /**/
            Planet tmpPlanet = null;
            Random tmpRandom = new Random(DateTime.Now.Millisecond);
            // Активируем БЧТ
            int tmpBigCount = Math.Max(2, Engine.Planets.PlanetList.Count / ciBigHoleRange) * 2 - fBigHolesCount;
            for (int tmpI = 0; tmpI < tmpBigCount; tmpI++)
                ActivatePair(tmpRandom, true, ref tmpPlanet);
            // Активируем простые чт
            int tmpSmallCount = Math.Max(2, Engine.Planets.PlanetList.Count / ciSmallHoleRange) * 2 - fSmallHolesCount;
            for (int tmpI = 0; tmpI < tmpSmallCount; tmpI++)
                ActivatePair(tmpRandom, false, ref tmpPlanet);
        }

        /// <summary>
        /// Регистрация планеты как ЧТ
        /// </summary>
        /// <param name="aPlanet">Планетоид</param>
        public void Register(Planet aPlanet)
        {
            if (aPlanet.State == PlanetState.Inactive)
                fWormholesList.Add(aPlanet);
            else if (aPlanet.IsBigHole)
                fBigHolesCount++;
        }

        /// <summary>
        /// Активация таймера ЧТ
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aTimer">Тип таймера</param>
        /// <param name="aValue">Значение таймера</param>
        public void Activate(Planet aPlanet, PlanetTimer aTimer, int aValue)
        {
            Engine.Planets.Action.Utils.TimerAdd(aPlanet, aTimer, OnTimer, aValue);
        }
    }
}