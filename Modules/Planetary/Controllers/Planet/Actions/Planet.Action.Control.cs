/////////////////////////////////////////////////
//
// Изменение видимости и покрытия территории
//
// Copyright(c) 2016 UAShota
//
// Rev H  2020.02.29
//
/////////////////////////////////////////////////

using System.Collections.Generic;
using Empire.Modules.Classes;
using Empire.Planetary.Classes;

namespace Empire.Planetary.PlanetSpace
{
    /// <summary>
    /// Класс изменения видимости и покрытия территории
    /// </summary>
    internal class Control : PlanetaryAccess
    {
        /// <summary>
        /// Обновление видимости планетоида
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок, получающий или теряющий видимость</param>
        /// <param name="aHardLight">Текущая планета или окраина</param>
        /// <param name="aIncrement">Получение или потеря видимости</param>
        private void ChangeVisibility(Planet aPlanet, Player aPlayer, bool aHardLight, bool aIncrement)
        {
            // Локальные не имеют контроля
            if (aPlayer.ID == 1)
                return;
            // Словарь контроля
            Dictionary<Player, int> tmpDict;
            // Выберем тип списка
            if (aHardLight)
                tmpDict = aPlanet.PlayerLightHard;
            else
                tmpDict = aPlanet.PlayerLightSoft;
            // Проверим его наличие
            if (!tmpDict.TryGetValue(aPlayer, out int tmpCount))
                tmpCount = 0;
            // Увеличим счетчик или добавим новый
            if (aIncrement)
            {
                tmpCount++;
                if (tmpCount == 1)
                {
                    tmpDict.Add(aPlayer, tmpCount);
                    Engine.SocketWriter.PlanetVisibilityUpdate(aPlanet, aPlayer, aHardLight, true);
                    return;
                }
            }
            else
            // Уменьшим счетчик или удалим пустой
            {
                tmpCount--;
                if (tmpCount == 0)
                {
                    tmpDict.Remove(aPlayer);
                    Engine.SocketWriter.PlanetVisibilityUpdate(aPlanet, aPlayer, aHardLight, false);
                    return;
                }
            }
            // Если данные уже есть, то просто обновим
            tmpDict[aPlayer] = tmpCount;
        }

        /// <summary>
        /// Обновление контроля покрытия планеты
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок, получающий или теряющий контроль покрытия</param>
        /// <param name="aIncrement">Получение или потеря контроля</param>
        private void ChangeCoverage(Planet aPlanet, Player aPlayer, bool aIncrement)
        {
            // Локальные или ЧТ не имеют контроля
            if ((aPlayer.ID == 1) || (aPlanet.Type == PlanetType.Hole))
                return;
            // Проверим его наличие
            if (!aPlanet.PlayerCoverage.TryGetValue(aPlayer, out int tmpCount))
                tmpCount = 0;
            // Увеличим счетчик или добавим новый
            if (aIncrement)
            {
                tmpCount++;
                if (tmpCount == 1)
                {
                    aPlanet.PlayerCoverage.Add(aPlayer, tmpCount);
                    Engine.SocketWriter.PlanetCoverageUpdate(aPlanet, aPlayer, true);
                    return;
                }
            }
            else
            // Уменьшим счетчик или удалим пустой
            {
                tmpCount--;
                if (tmpCount == 0)
                {
                    aPlanet.PlayerCoverage.Remove(aPlayer);
                    Engine.SocketWriter.PlanetCoverageUpdate(aPlanet, aPlayer, false);
                    return;
                }
            }
            // Если данные уже есть, то просто обновим
            aPlanet.PlayerCoverage[aPlayer] = tmpCount;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="aEngine">Базовый движок</param>
        public Control(PlanetaryEngine aEngine) : base(aEngine)
        {
        }

        /// <summary>
        /// Обновления контроля над планетой
        /// </summary>
        /// <param name="aPlanet">Планета</param>
        /// <param name="aPlayer">Игрок</param>
        /// <param name="aIncrement">Получение или потеря контроля</param>
        /// <param name="aOnlyCurrent">Только текущая планета или с окраиной (для чт)</param>
        public void Call(Planet aPlanet, Player aPlayer, bool aIncrement, bool aOnlyCurrent)
        {
            // Отправим текущую планету
            ChangeVisibility(aPlanet, aPlayer, false, aIncrement);
            // Большую видимость и видимость окраин, только если освещаемый
            if (!aOnlyCurrent)
                ChangeVisibility(aPlanet, aPlayer, true, aIncrement);
            else
                return;
            // Отправим кольца
            foreach (Planet tmpRing1 in aPlanet.Links)
            {
                // Отправим видимость первого кольца
                ChangeVisibility(tmpRing1, aPlayer, false, aIncrement);
                // Отправим контроль первого кольца
                ChangeCoverage(tmpRing1, aPlayer, aIncrement);
                // Отправим контроль второго кольца
                foreach (Planet tmpRing2 in tmpRing1.Links)
                    ChangeCoverage(tmpRing2, aPlayer, aIncrement);
            }
        }
    }
}