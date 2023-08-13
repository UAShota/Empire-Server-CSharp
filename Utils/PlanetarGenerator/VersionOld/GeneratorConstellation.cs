using System.Collections.Generic;
using  config = EmpireServer.PlanetarGenerator.Configuration;
using op = ConsoleApp1.Uttils.MathOperation;
using System;

/// <summary>
///Оформлен шаблон работа приостановленна
/// </summary>
namespace EmpireServer.PlanetarGenerator
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
        public bool itsAvaible() {
            return avaible;
        }

        //метод начала генерации созвездия
        public void StartGenerateConstellation() {
            Initializtion();
            GreateEmtySectors();
            JudjeSectorsRole();
            JudjeSectorsNeighborins();
            JudjeBorderSectors();
            GreatePulsarSectors();
            GreateNullSectors();
            FillingSectors();
            JudjeCosmicBodysType();
            JudjePlanetResurse();
            avaible = true;
            Console.WriteLine("The End");
        }

        //метод инициализации переменных количества строк и линий а также определения центрального сектора
        private void Initializtion() {
            numsColumAndLineMax = (int)Math.Sqrt(config.maxPlanetGalaxy / config.maxSeectorCosmicbody)
               + config.random.Next(config.minAdditionalSectorColumAndLine, config.maxAdditionalSectorColumAndLine);
            sectors.SetNumsSectors(numsColumAndLineMax);
            Console.WriteLine(numsColumAndLineMax);
            if (numsColumAndLineMax % 2 == 0) { 
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
        private void GreateEmtySectors() {
            for (int colum  = 0;colum<numsColumAndLineMax;colum++) {
                for (int line = 0;line<numsColumAndLineMax;line++) {
                    AddNewSector(colum, line);
                }
            }
        }

        /// <summary>
        /// Создание сектора и запись его в колекции
        /// </summary>
        private void AddNewSector(int colum, int line) {
            int posX = (colum - columAndLineCenter) * config.widhtSector;
            int posY = (line -columAndLineCenter) * config.heightSector;
            Sector sector = new Sector(posX,posY);
            sectors.AddSector(sector,colum,line);
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
            for (int i = sectors.Length()-1; i > -1; i--)
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
        private void JudjeSectorsNeighborins() {
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
        private void AddNeighborinToSector(int colum,int line)
        {
            for (int columN= colum-1;columN<colum+2;columN++ ) {
                for (int lineN = line-1;lineN<line+2;lineN++) {
                    if (!(columN < 0) &&
                        !(columN > numsColumAndLineMax-2) &&
                        !(lineN < 0) &&
                        !(lineN > numsColumAndLineMax-2))
                        sectors.GetColumLine(colum,line).AddNeighborin(sectors.GetColumLine(columN,lineN));    
                }
            }


        }

        /// <summary>
        /// Метод отбора граничных секторов
        /// </summary>
        private void JudjeBorderSectors() {
            List<Sector> removeEmty = new List<Sector>();
            foreach (Sector sector in sectors.emtys) {
                bool itsIternalNeighborin = false;
                bool itsAverageNeighborin = false;
                bool itsExternalNeighborin = false;
                foreach (Sector sectorN in sector.sectorsNeighborin) {
                   switch (sectorN.type) {
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
                    sector.type = TypeSector.borderIternalAverag;
                    removeEmty.Add(sector);
                    sectors.freeBorderItAv.Add(sector);
                }
                if (itsAverageNeighborin && itsExternalNeighborin)
                {
                    sector.type = TypeSector.borderAveragExternal;
                    removeEmty.Add(sector);
                    sectors.freeBorderAvEx.Add(sector);
                }
            }
            foreach (Sector sector in removeEmty) {
                sectors.emtys.Remove(sector);
            }
            sectors.emtys.Capacity = sectors.emtys.Count;
        }

        //метод определения секторов под пульсары 
        private void GreatePulsarSectors() {
            GreateIternalAveragPulsarSectors();
            GreateAveragExternalPulsarSectors();
        }

        //метод создания секторов с пульсарами между внутреним и средним кольцом 
        private void GreateIternalAveragPulsarSectors()
        {
            List<Sector> permission = new List<Sector>();
            permission.AddRange(sectors.freeBorderItAv);
            int numsPulsarItAv = config.random.Next(config.minPulsarItAv, config.maxPulsarItAv);
            for (int i = 0; i < numsPulsarItAv; i++)
            {
                AddNewPulsarSector(sectors.freeBorderItAv, sectors.pulsarItAv, config.maxAttemptAddNewPulsar, config.minDistanseBetweenSectorsPulsarItAv);
            }
            if (!(sectors.pulsarItAv.Count < config.minPulsarItAv) &&
                !(sectors.pulsarItAv.Count > config.maxPulsarItAv))
                permission.Clear();
            else
            {
                foreach (Sector sector in sectors.pulsarItAv)
                {
                    sector.type = TypeSector.borderIternalAverag;
                    sectors.freeBorderItAv = permission;
                }
                sectors.pulsarItAv.Clear();
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
        private void AddNewPulsarSector(List<Sector> borderSectors,List<Sector> pulsarSectors,
            int attempt, int minDistanseBetweenPulsar) {
            List<Sector> removeSectors = new List<Sector>();
            if (attempt > 0)
            {
                if (borderSectors.Count > 0)
                {
                    int numNewPulsar = config.random.Next(0, borderSectors.Count - 1);

                    bool avaibleDistanse = true;
                    foreach (Sector sector in pulsarSectors)
                    {
                        if (!AvaibleDistanseBetweenSectors(borderSectors[numNewPulsar], sector,
                            minDistanseBetweenPulsar))
                            avaibleDistanse = false;
                    }
                    if (avaibleDistanse)
                    {
                        borderSectors[numNewPulsar].type = TypeSector.pulsar;
                        pulsarSectors.Add(borderSectors[numNewPulsar]);
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
                        AddNewPulsarSector(borderSectors, pulsarSectors, attempt - 1,minDistanseBetweenPulsar);
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
        private bool AvaibleDistanseBetweenSectors(Sector sector1, Sector sector2,int minDistanse) {
            double distanse = op.Distanse(sector1.posX, sector1.posY, sector2.posX, sector2.posY);
            if (distanse > minDistanse)
                return true;
            else
                return false;            
        }

        //метод создания секторов с пульсарами между средним и внешним кольцом
        private void GreateAveragExternalPulsarSectors()
        {
            List<Sector> permission = new List<Sector>();
            permission.AddRange(sectors.freeBorderAvEx);
            int numsPulsarAvEx = config.random.Next(config.minPulsarAvEx, config.maxPulsarAvEx);
            for (int i = 0; i < numsPulsarAvEx; i++)
            {
                AddNewPulsarSector(sectors.freeBorderAvEx, sectors.pulsarAvEx, config.maxAttemptAddNewPulsar, config.minDistanseBetweenSectorsPulsarAvEx);
            }
            if (!(sectors.pulsarAvEx.Count < config.minPulsarAvEx) &&
                !(sectors.pulsarAvEx.Count > config.maxPulsarAvEx))
                permission.Clear();
            else
            {
                foreach (Sector sector in sectors.pulsarAvEx)
                {
                    sector.type = TypeSector.borderAveragExternal;
                    sectors.freeBorderAvEx = permission;
                }
                sectors.freeBorderAvEx.Clear();
                config.minDistanseBetweenSectorsPulsarAvEx -= 3;
                GreateAveragExternalPulsarSectors();

            }
        }

        //метод разброса нулевых секторов (в которых не будет космических тел)
        private void GreateNullSectors() {

        }

        private void GreateConectionSectors() {

        }

        private void GreateConectionOnType(TypeSector type) {
        }

        //метод насыщения секторов космическими телами
        private void FillingSectors() {

        }

        //метод распределения космических тел по типах
        private void JudjeCosmicBodysType() {

        }

        //метод распределения ресурсов по планетах
        private void JudjePlanetResurse()
        {

        }



    }

}