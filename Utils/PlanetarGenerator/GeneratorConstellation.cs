using System;
using System.Collections.Generic;
using config = Empire.PlanetarGenerator.Configuration;
using op = ConsoleApp1.Uttils.MathOperation;
//using UnityEngine;

/// <summary>
///Оформлен шаблон работа приостановленна
/// </summary>
namespace Empire.PlanetarGenerator
{
    /// <summary>
    /// Клас генерации созвездий
    /// </summary>
    public class GeneratorConstellation
    {
        // Переменная максимального размера сетки созвездия 
        // И переменая центра 
        int numsColumAndLineMax, columAndLineCenter;

        //Центр созвездия
        int centerX = 0, centerY = 0;

        //Колекция всех секторов
        public SectorsCollection sectors = new SectorsCollection();

        //готовность созвездия  
        public bool avaible = false;

        //гетер готовности
        public bool itsAvaible()
        {
            return avaible;
        }

        //метод начала генерации созвездия
        public void StartGenerateConstellation()
        {
            Initializtion();
            GreateEmtySectors();
            JudjeSectorsRole();
            JudjeSectorsNeighborins();
            JudjeBorderSectors();
            Console.WriteLine("Start averages num - " + sectors.averages.Count);
            Console.WriteLine("Start iternals num - " + sectors.iternals.Count);
            Console.WriteLine("Start external num - " + sectors.externals.Count);
            GreatePulsarSectors();
            GreateClasterSectors();
            GreateTestCutConstelation();
            GreateNullSectors();
            FillingSectors();
            JudjeCosmicBodysType();
            JudjePlanetResurse();
            avaible = true;
            Console.WriteLine("Finish averages num - " + sectors.averages.Count);
            Console.WriteLine("Finish iternals num - " + sectors.iternals.Count);
            Console.WriteLine("Finish external num - " + sectors.externals.Count);
            Console.WriteLine("The End");
        }

        //метод инициализации переменных количества строк и линий а также определения центрального сектора
        private void Initializtion()
        {
            numsColumAndLineMax = (int)Math.Sqrt(config.maxPlanetGalaxy / config.maxSeectorCosmicbody)
               + config.random.Next(config.minAdditionalSectorColumAndLine, config.maxAdditionalSectorColumAndLine);
            sectors.SetNumsSectors(numsColumAndLineMax);
            Console.WriteLine(numsColumAndLineMax);
            if (numsColumAndLineMax % 2 == 0)
            {
                columAndLineCenter = numsColumAndLineMax / 2 + 1;
                centerX = -10;
                centerY = -10;
            }
            else
                columAndLineCenter = (numsColumAndLineMax - 1) / 2;
        }

        /// <summary>
        /// Создание пустых секторов по сетке
        /// </summary>
        private void GreateEmtySectors()
        {
            for (int colum = 0; colum < numsColumAndLineMax; colum++)
            {
                for (int line = 0; line < numsColumAndLineMax; line++)
                {
                    AddNewSector(colum, line);
                }
            }
        }

        /// <summary>
        /// Создание сектора и запись его в колекции
        /// </summary>
        private void AddNewSector(int colum, int line)
        {
            int posX = (colum - columAndLineCenter) * config.widhtSector;
            int posY = (line - columAndLineCenter) * config.heightSector;
            Sector sector = new Sector(posX, posY, centerX, centerY);
            sectors.AddSector(sector, colum, line);
        }

        //метод роспределения секторов по кольцам
        private void JudjeSectorsRole()
        {
            int minDistanseFromCenterIternalSectors = 0,
                maxDistanseFromCenterIternalSectors = config.widhtSector *
                config.persentInternalSectors * numsColumAndLineMax / 100 / 2;
            int minDistanseFromCenterAverageSectors = maxDistanseFromCenterIternalSectors + config.widhtSector,
                maxDistanseFromCenterAverageSectors = minDistanseFromCenterAverageSectors +
                config.widhtSector * config.persentAveragSectors * numsColumAndLineMax / 100 / 2 - config.widhtSector;
            int minDistanseFromCenterExternalSectors = maxDistanseFromCenterAverageSectors + config.widhtSector,
                maxDistanseFromCenterExternalSectors = minDistanseFromCenterExternalSectors +
                config.widhtSector * config.persentExternalSectors * numsColumAndLineMax / 100 / 2 - config.widhtSector;
            for (int i = sectors.Length() - 1; i > -1; i--)
            {
                double distanceSectorFromCenter = op.Distanse(sectors.GetIndex(i).posX, sectors.GetIndex(i).posY, centerX, centerY);
                if (distanceSectorFromCenter > minDistanseFromCenterIternalSectors &&
                    distanceSectorFromCenter < maxDistanseFromCenterIternalSectors)
                {
                    sectors.GetIndex(i).type = TypeSector.iternal;
                    sectors.iternals.Add(sectors.GetIndex(i));
                    sectors.emtys.Remove(sectors.GetIndex(i));
                    sectors.emtys.Capacity = sectors.emtys.Count;
                }
                if (distanceSectorFromCenter > minDistanseFromCenterAverageSectors &&
                    distanceSectorFromCenter < maxDistanseFromCenterAverageSectors)
                {
                    sectors.GetIndex(i).type = TypeSector.average;
                    sectors.averages.Add(sectors.GetIndex(i));
                    sectors.emtys.Remove(sectors.GetIndex(i));
                    sectors.emtys.Capacity = sectors.emtys.Count;
                }
                if (distanceSectorFromCenter > minDistanseFromCenterExternalSectors &&
                    distanceSectorFromCenter < maxDistanseFromCenterExternalSectors)
                {
                    sectors.GetIndex(i).type = TypeSector.external;
                    sectors.externals.Add(sectors.GetIndex(i));
                    sectors.emtys.Remove(sectors.GetIndex(i));
                    sectors.emtys.Capacity = sectors.emtys.Count;
                }
            }
        }

        /// <summary>
        /// Метод присуждения соседства всем секторам
        /// </summary>
        private void JudjeSectorsNeighborins()
        {
            for (int colum = 0; colum < numsColumAndLineMax; colum++)
            {
                for (int line = 0; line < numsColumAndLineMax; line++)
                {
                    AddNeighborinToSector(colum, line);
                }
            }
        }

        /// <summary>
        /// Метод записи в сектор его соседей
        /// </summary>
        /// <param name="colum">столбец сектора</param>
        /// <param name="line">строка сектора</param>
        private void AddNeighborinToSector(int colum, int line)
        {
            for (int columN = colum - 1; columN < colum + 2; columN++)
            {
                for (int lineN = line - 1; lineN < line + 2; lineN++)
                {
                    if (!(columN < 0) &&
                        !(columN > numsColumAndLineMax - 1) &&
                        !(lineN < 0) &&
                        !(lineN > numsColumAndLineMax - 1))
                        sectors.GetColumLine(colum, line).AddNeighborin(sectors.GetColumLine(columN, lineN));
                }
            }


        }

        /// <summary>
        /// Метод отбора граничных секторов
        /// </summary>
        private void JudjeBorderSectors()
        {
            List<Sector> removeEmty = new List<Sector>();
            foreach (Sector sector in sectors.emtys)
            {
                bool itsIternalNeighborin = false;
                bool itsAverageNeighborin = false;
                bool itsExternalNeighborin = false;
                foreach (Sector sectorN in sector.sectorsNeighborin)
                {
                    switch (sectorN.type)
                    {
                        case TypeSector.iternal:
                            itsIternalNeighborin = true;
                            break;
                        case TypeSector.average:
                            itsAverageNeighborin = true;
                            break;
                        case TypeSector.external:
                            itsExternalNeighborin = true;
                            break;
                    }
                }
                if (itsIternalNeighborin && itsAverageNeighborin)
                {
                    sector.type = TypeSector.border;
                    removeEmty.Add(sector);
                    sectors.freeBorderItAv.Add(sector);
                }
                if (itsAverageNeighborin && itsExternalNeighborin)
                {
                    sector.type = TypeSector.border;
                    removeEmty.Add(sector);
                    sectors.freeBorderAvEx.Add(sector);
                }
            }
            foreach (Sector sector in removeEmty)
            {
                sectors.emtys.Remove(sector);
            }
            sectors.emtys.Capacity = sectors.emtys.Count;
        }

        //метод определения секторов под пульсары 
        private void GreatePulsarSectors()
        {
            GreateIternalAveragPulsarSectors();
            GreateAveragExternalPulsarSectors(0);
        }

        //метод создания секторов с пульсарами между внутреним и средним кольцом 
        private void GreateIternalAveragPulsarSectors()
        {
            List<Sector> permission = new List<Sector>();
            permission.AddRange(sectors.freeBorderItAv);
            int numsPulsarItAv = config.random.Next(config.minPulsarItAv, config.maxPulsarItAv);
            for (int i = 0; i < numsPulsarItAv; i++)
            {
                AddNewPulsarSector(sectors.freeBorderItAv, sectors.pulsarsItAv, config.maxAttemptAddNewPulsar, config.minDistanseBetweenSectorsPulsarItAv);
            }
            if (!(sectors.pulsarsItAv.Count < config.minPulsarItAv) &&
                !(sectors.pulsarsItAv.Count > config.maxPulsarItAv))
                permission.Clear();
            else
            {
                foreach (Pulsar p in sectors.pulsarsItAv)
                {
                    p.sectorPulsar.type = TypeSector.border;

                }
                sectors.freeBorderItAv = permission;
                sectors.pulsarsItAv.Clear();
                config.minDistanseBetweenSectorsPulsarItAv -= 3;
                GreateIternalAveragPulsarSectors();

            }
        }

        /// <summary>
        /// Метод определения сектора под пульсар
        /// </summary>
        /// <param name="borderSectors">Список доступных секторов для создания пульсара</param>
        /// <param name="pulsarSectors">Список секторов с пульсарами</param>
        /// <param name="attempt">Количество допустимых попыток</param>
        /// <param name="minDistanseBetweenPulsar">Минимальное растояние между пульсарами</param>
        private void AddNewPulsarSector(List<Sector> borderSectors, List<Pulsar> pulsarSectors,
            int attempt, int minDistanseBetweenPulsar)
        {
            List<Sector> removeSectors = new List<Sector>();
            if (attempt > 0)
            {
                if (borderSectors.Count > 0)
                {
                    int numNewPulsar = config.random.Next(0, borderSectors.Count - 1);

                    bool avaibleDistanse = true;
                    foreach (Pulsar pulsar in pulsarSectors)
                    {
                        if (!AvaibleDistanseBetweenSectors(borderSectors[numNewPulsar], pulsar.sectorPulsar,
                            minDistanseBetweenPulsar))
                            avaibleDistanse = false;
                    }
                    if (avaibleDistanse)
                    {
                        borderSectors[numNewPulsar].type = TypeSector.pulsar;
                        Pulsar newPulsar = new Pulsar(borderSectors[numNewPulsar]);
                        newPulsar.Initialize();
                        pulsarSectors.Add(newPulsar);

                        for (int i = borderSectors.Count - 1; i > -1; i--)
                        {
                            if (!AvaibleDistanseBetweenSectors(borderSectors[numNewPulsar], borderSectors[i], minDistanseBetweenPulsar))
                            {
                                removeSectors.Add(borderSectors[i]);
                            }
                        }
                        foreach (Sector sector in removeSectors)
                        {
                            borderSectors.Remove(sector);
                        }
                        removeSectors.Clear();
                    }
                    else
                    {
                        AddNewPulsarSector(borderSectors, pulsarSectors, attempt - 1, minDistanseBetweenPulsar);
                    }
                }
            }
        }

        /// <summary>
        /// Метод проверки является ли растояние между секторами больше минимального
        /// </summary>
        /// <param name="sector1">Сектор1 для проверки</param>
        /// <param name="sector2">Сектор2 для проверки</param>
        /// <param name="minDistanse">минимально допустимое растояние</param>
        /// <returns></returns>
        private bool AvaibleDistanseBetweenSectors(Sector sector1, Sector sector2, int minDistanse)
        {
            double distanse = op.Distanse(sector1.posX, sector1.posY, sector2.posX, sector2.posY);
            if (distanse > minDistanse)
                return true;
            else
                return false;
        }

        //метод создания секторов с пульсарами между средним и внешним кольцом
        private void GreateAveragExternalPulsarSectors(int corectionDistans)
        {
            if (corectionDistans < 50)
            {
                List<Sector> permission = new List<Sector>();
                permission.AddRange(sectors.freeBorderAvEx);
                int numsPulsarAvEx = config.random.Next(config.minPulsarAvEx, config.maxPulsarAvEx);
                for (int i = 0; i < numsPulsarAvEx; i++)
                {
                    AddNewPulsarSector(sectors.freeBorderAvEx, sectors.pulsarsAvEx, config.maxAttemptAddNewPulsar, config.minDistanseBetweenSectorsPulsarAvEx - corectionDistans);
                }
                if (!(sectors.pulsarsAvEx.Count < config.minPulsarAvEx) &&
                    !(sectors.pulsarsAvEx.Count > config.maxPulsarAvEx))
                    permission.Clear();
                else
                {
                    foreach (Pulsar p in sectors.pulsarsAvEx)
                    {
                        p.sectorPulsar.type = TypeSector.border;

                    }
                    sectors.freeBorderAvEx = permission;
                    sectors.pulsarsAvEx.Clear();
                    GreateAveragExternalPulsarSectors(corectionDistans + 10);
                }
            }
        }

        /// <summary>
        /// Начало кластеризации созвездия
        /// </summary>
        private void GreateClasterSectors()
        {
            GreateClaster(config.minPersentClasterAverage, config.maxPersentClasterAverage, sectors.averages);
            GreateClaster(config.minPersentClasterExternal, config.maxPersentClasterExternal, sectors.externals);
            ReturnInClasterSectors();
        }

        /// <summary>
        /// Создание кластеров внутри кольца
        /// </summary>
        /// <param name="minPersentClaster">минимальное количество кластеров для кольца</param>
        /// <param name="maxPersentClaster">максимальное количество кластеров для кольца</param>
        /// <param name="sectors">сектора текущего кольца</param>
        private void GreateClaster(int minPersentClaster, int maxPersentClaster, List<Sector> sectors)
        {
            int persentSectorsInList = config.random.Next(minPersentClaster, maxPersentClaster);
            int numsSectorsTypeInClasters = sectors.Count * persentSectorsInList / 100;
            int numClaster = 0;
            for (int i = 0; i < numsSectorsTypeInClasters;)
            {
                int numsSectorsInNewClaster = config.random.Next(config.minSectorsInClaster, config.maxSectorsInClaster);
                i = i + numsSectorsInNewClaster;
                AddClaster(sectors, numClaster, numsSectorsInNewClaster, 5);
                numClaster++;

            }
        }

        /// <summary>
        /// Метод добавления нового кластера в кольце
        /// </summary>
        /// <param name="sectors">доступные сектора текущего кольца</param>
        /// <param name="numClaster">номер текущего кластера</param>
        /// <param name="numsSectorsInNewClaster">число секторов в новом секторе</param>
        /// <param name="attempt">количество попыток для создания</param>
        private void AddClaster(List<Sector> sectors, int numClaster, int numsSectorsInNewClaster, int attempt)
        {
            int numRandomSector = config.random.Next(0, sectors.Count - 1);
            if (sectors[numRandomSector].myClaster == null)
            {
                Claster newClaster = new Claster(numClaster, sectors[numRandomSector].type, sectors[numRandomSector]);
                sectors[numRandomSector].myClaster = newClaster;
                newClaster.SaturationClaster(sectors, numsSectorsInNewClaster);
                this.sectors.clasters.Add(newClaster);
            }
            else
                if (attempt > 0)
                AddClaster(sectors, numClaster, numsSectorsInNewClaster, attempt - 1);
        }

        /// <summary>
        /// Возвращения всех секторов которые были кластеризированые в списки их колец
        /// </summary>
        private void ReturnInClasterSectors()
        {
            foreach (Claster claster in sectors.clasters)
            {
                switch (claster.type)
                {
                    case TypeSector.average:
                        sectors.averages.AddRange(claster.sectors);
                        break;
                    case TypeSector.external:
                        sectors.externals.AddRange(claster.sectors);
                        break;
                    case TypeSector.iternal:
                        sectors.iternals.AddRange(claster.sectors);
                        break;
                }
            }
        }

        //метод разброса нулевых секторов (в которых не будет космических тел)
        private void GreateNullSectors()
        {
            int maxSectorsInNullClasterForIternal = 2;
            GreateNullInList(sectors.iternals, maxSectorsInNullClasterForIternal, config.persentNullIternalSectors);

            int maxSectorsInNullClasterForAverage = sectors.cuts.Find((CutConstelletion c) =>
            { return c.type == TypeSector.average && c.side == Side.down; }).sectors.Count - 1;
            GreateNullInList(sectors.averages, maxSectorsInNullClasterForAverage, config.persentNullAverageSectors);

            int maxSectorsInNullClasterForExternal = sectors.cuts.Find((CutConstelletion c) =>
            { return c.type == TypeSector.external && c.side == Side.down; }).sectors.Count - 1;
            GreateNullInList(sectors.externals, maxSectorsInNullClasterForExternal, config.persentNullExternalSectors);

        }


        /// <summary>
        /// Создание нулевых секторов для одного из колец 
        /// </summary>
        /// <param name="sectorsOnType">сектора из выбраного кольца</param>
        /// <param name="maxNullSectorsInClasterThisList">максимальное количество нулевых секторов в кластере</param>
        /// <param name="persentNullSectorsInList">процент нулевых секторов в етом кольце</param>
        private void GreateNullInList(List<Sector> sectorsOnType, int maxNullSectorsInClasterThisList, int persentNullSectorsInList)
        {
            List<Sector> permission = new List<Sector>();
            permission.AddRange(sectorsOnType);

            int numNullSectorsInTheList = sectorsOnType.Count * persentNullSectorsInList / 100;
            for (int i = 0; i < numNullSectorsInTheList;)
            {
                int nullSectorsInNewClaster = config.random.Next(1, maxNullSectorsInClasterThisList);
                i = nullSectorsInNewClaster + i;
                if (!GreateNewNullClaster(100, nullSectorsInNewClaster, sectorsOnType, permission))
                {
                    i = numNullSectorsInTheList;
                }
            }
            sectorsOnType = permission;
        }

        /// <summary>
        /// Создание одного нулевого кластера внутри кольца
        /// </summary>
        /// <param name="attempt">количество попыток</param>
        /// <param name="numNullSectorsInClaster">количество нулевых секторов в новом кластере</param>
        /// <param name="sectorsOnType">сетора из текощего кольца</param>
        private bool GreateNewNullClaster(int attempt, int numNullSectorsInClaster
            , List<Sector> sectorsOnType, List<Sector> permission)
        {
            if (attempt > 0)
            {
                if (attempt < 5)
                {
                    if (numNullSectorsInClaster > 1)
                    {
                        numNullSectorsInClaster = 1;
                    }
                }
                int randomNumSectorInList = config.random.Next(0, sectorsOnType.Count - 1);
                Sector centralSector = sectorsOnType[randomNumSectorInList];
                if (!centralSector.HaveNeighborinType(TypeSector.nul) &&
                    centralSector.GetNumNeighborinForNullClaster() > numNullSectorsInClaster &&
                    centralSector.type != TypeSector.nul &&
                    !centralSector.HaveNeighborinType(TypeSector.pulsar))
                {
                    TypeSector oldType = centralSector.type;
                    Claster newNullClaster = new Claster(numNullSectorsInClaster,
                       centralSector.type, centralSector);
                    newNullClaster.oldType = oldType;
                    newNullClaster.SaturrationNullClaster(sectorsOnType, numNullSectorsInClaster);
                    if (TheNullClasterItsValid(newNullClaster, permission))
                    {
                        sectors.nullClasters.Add(newNullClaster);
                        Console.WriteLine(true);
                    }
                    else
                    {
                        Console.WriteLine(false);
                        GreateNewNullClaster(attempt - 1, numNullSectorsInClaster
                            , sectorsOnType, permission);
                        newNullClaster.NewType(oldType);
                        sectorsOnType.AddRange(newNullClaster.sectors);
                    }
                }
                else
                {
                    GreateNewNullClaster(attempt - 1, numNullSectorsInClaster
                        , sectorsOnType, permission);
                }
            }
            return true;
        }

        private bool TheNullClasterItsValid(Claster claster
            , List<Sector> sectorsOneType)
        {
            bool result = false;
            int numsQurtionTest = 0;
            List<Sector> sectorsForTest = null;
            List<Sector> sectorsForAudit = new List<Sector>(sectorsOneType.Count / 3);
            List<Sector> sectorsListConnect = new List<Sector>(sectorsOneType.Count);
            TypeSector typeSector = claster.oldType;
            List<Sector> sectorsNull;
            List<CutConstelletion> cuts = SelectedCutsForTestClaster(claster,
              sectorsOneType, ref sectorsForTest, ref numsQurtionTest);
            sectorsNull = sectorsForTest.FindAll((Sector s) => { return s.type == TypeSector.nul; });
            foreach (CutConstelletion c in cuts)
            {
                c.CutOn();
            }
            sectorsForAudit.Add(claster.FindNeighborinSectorType(claster.oldType));
            while (sectorsForAudit.Count > 0)
            {
                foreach (Sector s in sectorsForAudit[0].sectorsNeighborin.FindAll((Sector ss) => { return ss.type == typeSector; }))
                {
                    if (!sectorsForAudit.Contains(s)
                        && !sectorsListConnect.Contains(s))
                        sectorsForAudit.Add(s);
                }
                sectorsListConnect.Add(sectorsForAudit[0]);
                sectorsForAudit.RemoveAt(0);
            }
            foreach (CutConstelletion c in cuts)
            {
                c.CutOf();
            }

            Console.WriteLine(sectorsForTest.Count);
            Console.WriteLine(sectorsNull.Count);
            Console.WriteLine(sectorsForAudit.Count);
            Console.WriteLine(sectorsListConnect.Count);
            if (sectorsForTest.Count == sectorsNull.Count + sectorsListConnect.Count)
            {
                return true;
            }
            else
            {
                foreach (Sector s in sectorsForTest)
                {
                    s.type = TypeSector.pulsar;
                }
                foreach (Sector s in sectorsListConnect)
                {
                    s.type = TypeSector.iternal;
                }
                sectorsListConnect[sectorsListConnect.Count - 1].type = TypeSector.border;
                foreach (Sector s in claster.sectors)
                {
                    s.type = TypeSector.nul;
                }
            }
            return result;
        }

        private List<CutConstelletion> SelectedCutsForTestClaster(Claster claster
            , List<Sector> SectorsOneType, ref List<Sector> sectorsForTest
            , ref int numQurtion)
        {
            List<CutConstelletion> selectCuts = sectors.cuts.FindAll
                ((CutConstelletion c) => { return c.type == claster.oldType; });
            Sector centralSector = claster.sectors[0];
            if (centralSector.quarter == Quarter.missing)
            {
                numQurtion = 2;
                switch (centralSector.half)
                {
                    case Half.up:
                        selectCuts.RemoveAll((CutConstelletion c) =>
                        {
                            return c.side == Side.up
                              || c.side == Side.down;
                        });
                        sectorsForTest = SectorsOneType.FindAll((Sector s) =>
                            {
                                return s.quarter == Quarter.upLeft
                                      || s.quarter == Quarter.upRight
                                      || s.half == Half.up;
                            });
                        break;
                    case Half.down:
                        selectCuts.RemoveAll((CutConstelletion c) =>
                        {
                            return c.side == Side.up
                              || c.side == Side.down;
                        });
                        sectorsForTest = SectorsOneType.FindAll((Sector s) =>
                            {
                                return s.quarter == Quarter.downLeft
                                  || s.quarter == Quarter.downRight
                                  || s.half == Half.down;
                            });
                        break;
                    case Half.right:
                        selectCuts.RemoveAll((CutConstelletion c) =>
                        {
                            return c.side == Side.left
                              || c.side == Side.right;
                        });
                        sectorsForTest = SectorsOneType.FindAll((Sector s) =>
                        {
                            return s.quarter == Quarter.upRight
                              || s.quarter == Quarter.downRight
                              || s.half == Half.right;
                        });
                        break;
                    case Half.left:
                        selectCuts.RemoveAll((CutConstelletion c) =>
                        {
                            return c.side == Side.left
                              || c.side == Side.right;
                        });
                        sectorsForTest = SectorsOneType.FindAll((Sector s) =>
                        {
                            return s.quarter == Quarter.downLeft
                              || s.quarter == Quarter.upLeft
                              || s.half == Half.left;
                        });
                        break;
                }
            }
            else
            {
                numQurtion = 3;
                switch (centralSector.quarter)
                {
                    case Quarter.downLeft:
                        selectCuts.RemoveAll((CutConstelletion c) =>
                        { return c.side == Side.down || c.side == Side.left; });
                        sectorsForTest = SectorsOneType.FindAll((Sector s) =>
                        {
                            return s.quarter == Quarter.downLeft
                                 || s.quarter == Quarter.downRight
                                 || s.half == Half.down || s.half == Half.left
                                 || s.quarter == Quarter.upLeft;
                        });
                        break;
                    case Quarter.downRight:
                        selectCuts.RemoveAll((CutConstelletion c) =>
                        { return c.side == Side.down || c.side == Side.right; });
                        sectorsForTest = SectorsOneType.FindAll((Sector s) =>
                        {
                            return s.quarter == Quarter.downLeft
                             || s.quarter == Quarter.downRight
                             || s.half == Half.down || s.half == Half.right
                             || s.quarter == Quarter.upRight;
                        });
                        break;
                    case Quarter.upLeft:
                        selectCuts.RemoveAll((CutConstelletion c) =>
                        { return c.side == Side.up || c.side == Side.left; });
                        sectorsForTest = SectorsOneType.FindAll((Sector s) =>
                        {
                            return s.quarter == Quarter.downLeft
                             || s.quarter == Quarter.upRight
                             || s.half == Half.up || s.half == Half.left
                             || s.quarter == Quarter.upLeft;
                        });
                        break;
                    case Quarter.upRight:
                        selectCuts.RemoveAll((CutConstelletion c) =>
                        { return c.side == Side.up || c.side == Side.right; });
                        sectorsForTest = SectorsOneType.FindAll((Sector s) =>
                        {
                            return s.quarter == Quarter.upLeft
                             || s.quarter == Quarter.upRight
                             || s.half == Half.up || s.half == Half.right
                             || s.quarter == Quarter.downRight;
                        });
                        break;
                }
            }
            return selectCuts;
        }

        private void GreateConectionOnType(TypeSector type)
        {
        }
        /// <summary>
        /// Инициализация масива сеторов для тестовых "розрезов" колец 
        /// </summary>
        private void GreateTestCutConstelation()
        {
            GreateCuts();
            AddSectorsInCut(sectors.iternals, TypeSector.iternal);
            AddSectorsInCut(sectors.averages, TypeSector.average);
            AddSectorsInCut(sectors.externals, TypeSector.external);
        }

        /// <summary>
        /// Иницициализация самих "разрезов"
        /// </summary>
        private void GreateCuts()
        {
            sectors.cuts.Add(new CutConstelletion(TypeSector.iternal, Side.left));
            sectors.cuts.Add(new CutConstelletion(TypeSector.iternal, Side.right));
            sectors.cuts.Add(new CutConstelletion(TypeSector.iternal, Side.up));
            sectors.cuts.Add(new CutConstelletion(TypeSector.iternal, Side.down));
            sectors.cuts.Add(new CutConstelletion(TypeSector.average, Side.left));
            sectors.cuts.Add(new CutConstelletion(TypeSector.average, Side.right));
            sectors.cuts.Add(new CutConstelletion(TypeSector.average, Side.up));
            sectors.cuts.Add(new CutConstelletion(TypeSector.average, Side.down));
            sectors.cuts.Add(new CutConstelletion(TypeSector.external, Side.left));
            sectors.cuts.Add(new CutConstelletion(TypeSector.external, Side.right));
            sectors.cuts.Add(new CutConstelletion(TypeSector.external, Side.up));
            sectors.cuts.Add(new CutConstelletion(TypeSector.external, Side.down));
        }



        /// <summary>
        /// Добавление к разрезам их Секторов
        /// </summary>
        /// <param name="sectors">Список текущих секторов</param>
        /// <param name="type">Тип текущих секторов</param>
        private void AddSectorsInCut(List<Sector> sectors, TypeSector type)
        {
            foreach (Sector sector in sectors)
            {
                if (sector.posX == centerX)
                    if (sector.posY > centerY)
                        this.sectors.cuts.Find((CutConstelletion cut) => { return cut.type == type && cut.side == Side.up; }).sectors.Add(sector);
                    else
                        this.sectors.cuts.Find((CutConstelletion cut) => { return cut.type == type && cut.side == Side.down; }).sectors.Add(sector);
                if (sector.posY == centerY)
                    if (sector.posX > centerX)
                        this.sectors.cuts.Find((CutConstelletion cut) => { return cut.type == type && cut.side == Side.right; }).sectors.Add(sector);
                    else
                        this.sectors.cuts.Find((CutConstelletion cut) => { return cut.type == type && cut.side == Side.left; }).sectors.Add(sector);
            }
        }


        //метод насыщения секторов космическими телами
        private void FillingSectors()
        {

        }

        //метод распределения космических тел по типах
        private void JudjeCosmicBodysType()
        {

        }

        //метод распределения ресурсов по планетах
        private void JudjePlanetResurse()
        {

        }



    }

}