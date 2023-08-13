using System;
namespace Empire.PlanetarGenerator
{

    public static class Configuration
    {

        //this class configuration generator galaxy
        //ето клас настроек генерации созвездия

        //Настройка размеров сектора
        public static int widhtSector = 10, heightSector = 10;

        //Настройка количества космических тел в галактике
        public static int maxPlanetGalaxy = 1200, minPlanetGalaxy = 800;

        //Настройка максимального и минимального количества тел в секторе
        public static int minSectorCosmicBody = 0, maxSeectorCosmicbody = 4;

        //Максимальная дистанция перелёта кораблей
        public static int maxLongDistance = 12;

        //Минимальная дистанция между планетами
        public static int minDistanceBetweenPlanets = 5;

        /// <summary>
        /// Количество попыток создания коннекта между секторами
        /// </summary>
        public static int attemptGreateConnection = 100;

        //Настройка количества планет в одном секторе для среднего кольца планет секторов.
        public static int minAveragePlanet = 1, maxAveragePlanet = 3;

        //Настройка количества планет в одном секторе для внутренего кольца секторов
        public static int minIternalPlanet = 1, maxIternalPlanet = 4;

        //Настройка количества планет в одном секторе для внешнего кольца секторов
        public static int minExternalPlanet = 1, maxExternalPlanet = 4;

        /// <summary>
        /// Максимальное количество попыток добавление нового сектора под пульсар
        /// </summary>
        public static int maxAttemptAddNewPulsar = 10;

        /// <summary>
        /// Минимально допустимое растояние между двумя пульсарами
        /// </summary>
        public static int minDistanseBetweenSectorsPulsarItAv = 30;
        public static int minDistanseBetweenSectorsPulsarAvEx = 60;

        /// <summary>
        /// Максимальное и минимально количество пульсаров между внутреним и средним кольцом
        /// </summary>
        public static int minPulsarItAv = 3, maxPulsarItAv = 5;

        /// <summary>
        /// Максиимальное и минимально количество пульсаров между средним и внешним кольцами
        /// </summary>
        public static int minPulsarAvEx = 4, maxPulsarAvEx = 8;

        //Настройка количества планет
        public static int minTerainPlanet;
        public static int maxTerainPlanet;

        //настройка количества звезд
        public static int minStar;
        public static int maxStar;

        //настройка процентого соотношения между кольцами секторов
        public static int persentInternalSectors = 20, persentAveragSectors = 50, persentExternalSectors = 30;

        /// <summary>
        /// Минимальное и максимальное количество кластеров для среднего кольца
        /// </summary>
        public static int minPersentClasterAverage = 65, maxPersentClasterAverage = 75;

        /// <summary>
        /// Минимальное и максимальное количество кластеров для внешнего кольца
        /// </summary>
        public static int minPersentClasterExternal = 60, maxPersentClasterExternal = 70;


        /// <summary>
        /// Минимальное и максимальное количество секторов в одном кластере.
        /// </summary>
        public static int maxSectorsInClaster = 5, minSectorsInClaster = 3;

        //Настройка процентного соотношения пустых секторов к заполненым для среднего и внешнего кольца секторов
        public static int persentNullIternalSectors = 5;
        public static int persentNullAverageSectors = 10;
        public static int persentNullExternalSectors = 5;

        //Среднее количество космических тел в одном секторе
        public static int advangBodyInOneSector = 2;
        /// <summary>
        /// Статичный екземпляр рандома для роздачи нуждающимся
        /// </summary>
        public static Random random = new Random();

        /// <summary>
        /// Минимальное и максимальное значение дополнительной ширины и высоты лички 
        /// </summary>
        public static int minAdditionalSectorColumAndLine = 4, maxAdditionalSectorColumAndLine = 8;


    }

}