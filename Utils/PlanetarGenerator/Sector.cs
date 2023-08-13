using System;
using System.Collections.Generic;
using config = Empire.PlanetarGenerator.Configuration;

/// <summary>
/// Идет робота над написаниием 
/// </summary>

namespace Empire.PlanetarGenerator
{
    /// <summary>
    /// Типы секторов
    /// </summary>
    public enum TypeSector
    {
        emty,
        average,
        iternal,
        external,
        border,
        nul,
        pulsar
    }
    /// <summary>

    public enum Quarter
    {
        upLeft,
        upRight,
        downLeft,
        downRight,
        missing
    }

    public enum Half
    {
        up,
        down,
        left,
        right,
        missing
    }


    /// Клас сектор описывает область пространства в созвездии
    /// </summary>
    public class Sector
    {
        //Сылка на кластера (если -1 то кластера нет)
        public Claster myClaster = null;
        //координаты сектора
        public int posX, posY;
        //границы сектора в общей системе координат
        private int minX, maxX, minY, maxY;
        //список соседних секторов
        public List<Sector> sectorsNeighborin = new List<Sector>(8);
        //список тел что входят в состав сектора
        public List<CosmicBody> bodies = new List<CosmicBody>();
        //список секторов с которыми сектор имеет перелет
        public List<Sector> connectedSectors = new List<Sector>();
        //тип сектора
        public TypeSector type;
        //маркер сектора с ошыбкой
        public bool bad = false;
        //Номер кластера нужен только для тестирования
        public int numClaster = -1;
        //Сылка на пульсар етого сектора, если сектор является сектором пульсара
        public Pulsar myPulsar = null;
        /// <summary>
        /// Четверть к которой относиться суктор
        /// </summary>
        public Quarter quarter;

        public Half half = Half.missing;


        //конструктор класа сектор
        public Sector(int posX, int posY, int centerX, int centerY)
        {
            this.posX = posX;
            this.posY = posY;
            minX = posX - config.widhtSector / 2;
            maxX = posX + config.widhtSector / 2;
            minY = posY - config.heightSector / 2;
            maxY = posY + config.heightSector / 2;
            type = TypeSector.emty;
            InitilizeQuartion(centerX, centerY);
        }

        private void InitilizeQuartion(int centerX, int centerY)
        {
            if (posX == centerX || posY == centerY)
            {
                InitializeHalf(centerX, centerY);
                quarter = Quarter.missing;
                return;
            }
            if (posX > centerX)
            {
                if (posY > centerY)
                    quarter = Quarter.upRight;
                else
                    quarter = Quarter.downRight;
            }
            else
            {
                if (posY > centerY)
                    quarter = Quarter.upLeft;
                else
                    quarter = Quarter.downLeft;
            }

        }

        private void InitializeHalf(int centerX, int centerY)
        {
            if (posX != centerX)
                if (posX > centerX)
                    half = Half.right;
                else
                    half = Half.left;
            if (posY != centerY)
                if (posY > centerY)
                    half = Half.up;
                else
                    half = Half.down;
        }

        //метод создания нового соедененния с однотипным соседом с которым ище нет соедененния
        public bool GreateNewConnect()
        {
            foreach (Sector sector in sectorsNeighborin)
            {
                if (sector.type == type && !connectedSectors.Contains(sector))
                    if (GreateNewConnectToSector(sector, config.attemptGreateConnection))
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Метод создания перелета с выбраным сектором
        /// </summary>
        /// <param name="sector"> обект выбраного для перелета сектора</param>
        /// <param name="attempt"> счетчик попыток</param>
        /// <returns></returns>
        public bool GreateNewConnectToSector(Sector sector, int attempt)
        {
            int myPosX, myPosY;
            int neighborinX, neighborinY;
            if (attempt < 1)// проверка есть ли ище попытки 
                return false;
            //проверка первая ли ето попытка
            if (attempt > config.attemptGreateConnection / 10 * 7
                && bodies.Count > 0)
            {
                if (sector.bodies.Count > 0)
                {
                    if (attempt > config.attemptGreateConnection - 1)
                    {
                        foreach (CosmicBody body in sector.bodies)
                        {
                            foreach (CosmicBody bodyMy in bodies)
                            {
                                if (BodyIsJoining(body, bodyMy))
                                {
                                    return true;//если находим соедененние между существующими
                                    //телами возвращаем истинну
                                }
                            }
                        }
                        //если не нашли соеденнений между существующими телами перезапускаем метод
                        return GreateNewConnectToSector(sector, attempt - 1);
                    }
                    else
                    {
                        if (bodies.Count > sector.bodies.Count)
                        {
                            neighborinX = config.random.Next(0, 10);
                            neighborinY = config.random.Next(0, 10);
                            if (sector.FreePoint(neighborinX, neighborinY))
                            {
                                foreach (CosmicBody body in bodies)
                                {
                                    if (PointesIsJoining(neighborinX, neighborinY, body.posX, body.posY))
                                    {
                                        //если случайно выбранные координаты точки под новое тело свободные 
                                        // и они удовлетворяют по  дистанции создаем  
                                        // тело в етой точке и возвращаем истинну
                                        sector.GreateBody(neighborinX - sector.minX, neighborinY - sector.minY);
                                        return true;
                                    }
                                }
                                // если точка не подходит для создания соеденения
                                // перезапускаем метод создания соеденнения
                                return GreateNewConnectToSector(sector, attempt - 1);
                            }
                            else
                            {
                                // если точка не свободна перезапускаем метод генерации
                                return GreateNewConnectToSector(sector, attempt - 1);
                            }

                        }
                        else
                        {
                            myPosX = config.random.Next(0, 10);
                            myPosY = config.random.Next(0, 10);
                            if (FreePoint(myPosX, myPosY))
                            {
                                foreach (CosmicBody body in sector.bodies)
                                {
                                    if (PointesIsJoining(myPosX, myPosY, body.posX, body.posY))
                                    {
                                        GreateBody(myPosX - minX, myPosY - minY);
                                        return true;
                                    }
                                }
                                //если кординаты точки не подходят для сооденения 
                                //перезапускаем метод
                                return GreateNewConnectToSector(sector, attempt - 1);
                            }
                            else
                            {
                                // если координаты для точки занятые перезапускаем метод
                                GreateNewConnectToSector(sector, attempt - 1);
                            }
                        }
                    }
                }
                else
                {//если у соседа нет космических тел
                    neighborinX = config.random.Next(0, 10);
                    neighborinY = config.random.Next(0, 10);
                    if (sector.FreePoint(neighborinX, neighborinY))
                    {
                        foreach (CosmicBody body in bodies)
                        {
                            if (PointesIsJoining(neighborinX, neighborinY, body.posX, body.posY))
                            {
                                //если случайно выбранные координаты точки под новое тело свободные 
                                // и они удовлетворяют по  дистанции создаем  
                                // тело в етой точке и возвращаем истинну
                                sector.GreateBody(neighborinX - sector.minX, neighborinY - sector.minY);
                                return true;
                            }
                        }
                        // если точка не подходит для создания соеденения
                        // перезапускаем метод создания соеденнения
                        return GreateNewConnectToSector(sector, attempt - 1);
                    }
                    else
                    {
                        // если точка не свободна перезапускаем метод генерации
                        return GreateNewConnectToSector(sector, attempt - 1);
                    }
                }

            }
            if (attempt > 0)
            {
                if (sector.bodies.Count > 0)
                {
                    myPosX = config.random.Next(0, 10);
                    myPosY = config.random.Next(0, 10);
                    if (FreePoint(myPosX, myPosY))
                    {
                        foreach (CosmicBody body in sector.bodies)
                        {
                            if (sector.PointesIsJoining(myPosX, myPosY, body.posX, body.posY))
                            {
                                GreateBody(myPosX, myPosY);
                                return true;
                            }
                        }
                        return GreateNewConnectToSector(sector, attempt - 1);
                    }
                    else
                    {
                        return GreateNewConnectToSector(sector, attempt - 1);
                    }
                }
                else
                {
                    myPosX = config.random.Next(0, 10);
                    myPosY = config.random.Next(0, 10);
                    neighborinX = config.random.Next(0, 10);
                    neighborinY = config.random.Next(0, 10);
                    if (FreePoint(myPosX, myPosY))
                    {
                        if (sector.FreePoint(neighborinX, neighborinY))
                        {
                            if (PointesIsJoining(myPosX, myPosY, neighborinX, neighborinY))
                            {
                                GreateBody(myPosX, myPosY);
                                sector.GreateBody(neighborinX, neighborinY);
                                return true;
                            }
                            else
                                return GreateNewConnectToSector(sector, attempt - 1);
                        }
                        else
                            return GreateNewConnectToSector(sector, attempt - 1);
                    }
                    else
                        return GreateNewConnectToSector(sector, attempt - 1);
                }
            }

            return false;
        }

        /// <summary>
        /// Метод определения есть ли между телами допустимое соедененние
        /// </summary>
        /// <returns>Возвращает true если допустимое соеденнение есть</returns>
        private bool BodyIsJoining(CosmicBody body, CosmicBody body1)
        {
            if (PointesIsJoining(body.posX, body.posY, body1.posX, body1.posY))
                return true;
            return false;
        }

        /// <summary>
        /// Метод определения возможно ли соеденить между собой точки (Учитывается 
        /// минимальное растояние между планетами
        /// и максимальная дальность)
        /// </summary>
        /// <param name="x1">Х первой точки</param>
        /// <param name="y1">У первой точки</param>
        /// <param name="x2">Х второй точки</param>
        /// <param name="y2">У второй точки</param>
        /// <returns></returns>
        private bool PointesIsJoining(int x1, int y1, int x2, int y2)
        {
            double distance = GetDistancePointes(x1, y1, x2, y2);
            if (distance < config.maxLongDistance && distance > config.minDistanceBetweenPlanets)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Метод измерения растояния между двумя точками
        /// </summary>
        /// <returns>Возвращается дистанция между точками</returns>
        public double GetDistancePointes(int x1, int y1, int x2, int y2)
        {
            double result = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            return result;
        }

        //есть ли в соседях пустой сектор
        public bool EmtyNeighborin()
        {
            bool result = false;
            foreach (Sector sector in sectorsNeighborin)
            {
                if (sector.type == TypeSector.emty)
                    return true;
            }
            return result;
        }

        //добавление сектора в соседи
        public void AddNeighborin(Sector sector)
        {
            sectorsNeighborin.Add(sector);
        }

        //создание космического тела внутри сектора
        public void GreateBody(int posX, int posY)
        {
            bodies.Add(new CosmicBody(posX + minX, posY + minY));
        }

        //создание космического тела внутри сектора с зарание извесным типом 
        public void GreateBody(int posX, int posY, BodyType type)
        {
            bodies.Add(new CosmicBody(posX + minX, posY + minY, type));
        }

        //входит ли точка в состав етого сектора
        public bool Contains(int posX, int posY)
        {
            if (posX > minX && posX < maxX)
                if (posY > minY && posY < maxY)
                    return true;
                else

                    return false;
            else
                return false;
        }

        //проверка доступности точки под планету
        public bool FreePoint(int posX, int posY)
        {
            bool result = true;
            posX = minX + posX;
            posY = minY + posY;
            if (!PointThisSector(posX, posY))
                return false;
            if (!PointNeighborin(posX, posY))
                return false;
            return result;

        }

        //проверка доступности точки под планету внутри етого сектора
        private bool PointThisSector(int posX, int posY)
        {
            bool avaible = true;
            foreach (CosmicBody body in bodies)
            {
                double distance = Math.Sqrt((posX - body.posX) * (posX - body.posX) + (posY - body.posY) * (posY - body.posY));
                if (distance < config.minDistanceBetweenPlanets)
                    return false;
            }
            return avaible;
        }

        //проверка доступности точки под планету среди каждого из соседей
        private bool PointNeighborin(int posX, int posY)
        {
            bool result = true;
            foreach (Sector sector in sectorsNeighborin)
            {
                if (!sector.PointThisSector(posX, posY))
                {
                    return false;
                }
            }
            return result;
        }

        // <summary>
        ///Возвращает количество соседних секторов тип которых Average и которые не относятся к кластерам 
        /// </summary>
        /// <returns></returns>
        public int GetNumAverageNeghborin()
        {
            int result = 0;
            foreach (Sector sector in sectorsNeighborin)
            {
                if (sector.type.Equals(TypeSector.average) && myClaster == null)
                    result++;
            }
            return result;
        }

        public int GetNumNeighborinForNullClaster()
        {
            int res = 0;
            foreach (Sector sector in sectorsNeighborin)
            {
                if (sector.type.Equals(type) &&
                    !sector.HaveNeighborinType(TypeSector.nul))
                    res++;

            }
            return res;
        }
        /// <summary>
        /// Получить случайного однотипного соседа для обєдинения в кластер
        /// </summary>
        /// <returns></returns>
		public Sector GetRandomNeigborinForClaster()
        {
            Sector result = null;
            List<Sector> sectorsAverages = new List<Sector>();
            sectorsAverages.AddRange(sectorsNeighborin.FindAll((Sector s) => { return s.type == type && s.myClaster == null; }));
            if (sectorsAverages.Count > 0)
                if (sectorsAverages.Count > 1)
                    result = sectorsAverages[config.random.Next(0, sectorsAverages.Count - 1)];
                else
                    result = sectorsAverages[0];

            return result;
        }

        public Sector GetRandomInList(List<Sector> sectorsNeighborin)
        {
            Sector result = null;
            List<Sector> sectorsMyType = new List<Sector>();
            sectorsMyType.AddRange(sectorsNeighborin.FindAll((Sector s) => { return s.type == type && s.myClaster == null; }));
            if (sectorsMyType.Count > 0)
                if (sectorsMyType.Count > 1)
                    result = sectorsMyType[config.random.Next(0, sectorsMyType.Count - 1)];
                else
                    result = sectorsMyType[0];

            return result;
        }

        public Sector GetRandomCommonNeigborin(Sector sectorNeighborin)
        {
            List<Sector> commons = new List<Sector>();
            foreach (Sector sector in sectorsNeighborin)
            {
                if (sectorNeighborin.sectorsNeighborin.Contains(sector))
                    commons.Add(sector);
            }

            return GetRandomInList(commons);
        }

        public Sector GetNeigborinType(TypeSector type)
        {
            foreach (Sector sector in sectorsNeighborin)
            {
                if (sector.type == type)
                    return sector;
            }
            return null;
        }

        //есть ли в соседях сектор из внешнего кольца
        public bool HaveNeighborinType(TypeSector typeSector)
        {
            bool result = false;
            foreach (Sector sector in sectorsNeighborin)
            {
                if (sector.type == typeSector)
                    return true;
            }
            return result;
        }
    }
}