using System.Collections.Generic;
using cnf = Empire.PlanetarGenerator.Configuration;
//using UnityEngine;

namespace Empire.PlanetarGenerator
{

    public enum ResurseClaster
    {
        Hydrogen,
        Titan,
        Ksenon,
        Kremniy,
        Antikristal,
        emty
    }


    public class Claster
    {
        /// <summary>
        /// Список секторов что относятся к даному кластеру
        /// </summary>
        public List<Sector> sectors;
        /// <summary>
        /// Тип секторов етого кластера
        /// </summary>
        public TypeSector type = TypeSector.emty;

        private List<Sector> neighborinsSectors = new List<Sector>();
        /// <summary>
        /// Старый тип секторов 
        /// </summary>
        public TypeSector oldType = TypeSector.emty;
        /// <summary>
        /// Ресурсы в етом кластере
        /// </summary>
        public ResurseClaster resurse = ResurseClaster.emty;
        /// <summary>
        /// номер етого кластера (нужно только для тестирования)
        /// </summary>
        private int num { get; }


        /// <summary>
        /// Конструктор кластера
        /// </summary>
        /// <param name="num">Номер кластера среди однотипных</param>
        /// <param name="type">Тип секторов внутри кластера</param>
        public Claster(int num, TypeSector type, Sector sector)
        {
            this.num = num;
            this.type = type;
            sectors = new List<Sector>();
            sectors.Add(sector);
        }



        /// <summary>
        /// Досыщение кластера секторами 
        /// </summary>
        /// <param name="sectorsOneType">Список секторов той же области что и кластер(нужен 
        /// для удаления секторра из него после добавления его в кластер)</param>
        /// <param name="numsSectorsInNewClaster">Количество секторов в етом кластере</param>
        public void SaturationClaster(List<Sector> sectorsOneType, int numsSectorsInNewClaster)
        {
            //Debug.Log ("yes");
            for (int i = 0; i < numsSectorsInNewClaster; i++)
            {
                AddNewSector(sectorsOneType);

            }
            sectors[0].numClaster = num;
            sectorsOneType.RemoveAll((Sector s) => { return sectors.Contains(s); });
            InitializeNeghborins();
        }

        /// <summary>
        /// Метод досыщения "пустого" кластера секторами
        /// </summary>
        /// <param name="sectorsOnType">список секторов из одного из колец(нужен для удаление из етого списка етих секторов)</param>
        /// <param name="numSectorsInTheClaster">количество секторов для насыщения</param>
        public void SaturrationNullClaster(List<Sector> sectorsOnType, int numSectorsInTheClaster)
        {
            List<Sector> avaibleNeighborinsCentralSector = sectors[0].sectorsNeighborin.FindAll((Sector s) => { return s.type == sectors[0].type && !s.HaveNeighborinType(TypeSector.nul); });

            for (int i = 0; i < numSectorsInTheClaster; i++)
            {
                //   Console.WriteLine(avaibleNeighborinsCentralSector.Count);
                if (avaibleNeighborinsCentralSector.Count > 0)
                {
                    int randomNumNeighborin = cnf.random.Next(0, avaibleNeighborinsCentralSector.Count - 1);
                    if (avaibleNeighborinsCentralSector[randomNumNeighborin].HaveNeighborinType(TypeSector.pulsar))
                    {
                        if (avaibleNeighborinsCentralSector[randomNumNeighborin].GetNeigborinType(TypeSector.pulsar).
                            myPulsar.FreeSector(avaibleNeighborinsCentralSector[randomNumNeighborin]))
                        {
                            sectors.Add(avaibleNeighborinsCentralSector[randomNumNeighborin]);
                            avaibleNeighborinsCentralSector.RemoveAt(randomNumNeighborin);
                        }
                        else
                        {
                            avaibleNeighborinsCentralSector.RemoveAt(randomNumNeighborin);
                            i--;
                        }
                    }
                    else
                    {
                        if (!sectors.Contains(avaibleNeighborinsCentralSector[randomNumNeighborin]))
                            sectors.Add(avaibleNeighborinsCentralSector[randomNumNeighborin]);
                        avaibleNeighborinsCentralSector.RemoveAt(randomNumNeighborin);
                    }
                }
                else
                    i = numSectorsInTheClaster;
            }
            InitializeNeghborins();
            NewType(TypeSector.nul);
            sectorsOnType.RemoveAll((Sector s) => { return sectors.Contains(s); });
        }


        /// <summary>
        /// Метод добавления в кластер нового сектора(Выбирается рандомно свободный сектор сосед изначального сектора)
        /// </summary>
        /// <param name="sectorsOneType">Список секторов той же области (Полное описание в методе выше)</param>
        private void AddNewSector(List<Sector> sectorsOneType)
        {
            Sector newSector = null;
            if (sectors.Count > 1)
            {
                newSector = sectors[0].GetRandomCommonNeigborin(sectors[1]);
            }
            else
            {
                newSector = sectors[0].GetRandomNeigborinForClaster();
            }
            if (newSector != null)
            {
                newSector.myClaster = this;
                newSector.numClaster = num;
                sectors.Add(newSector);
                // sectorsOneType.Remove(newSector);
            }
            //Debug.Log (newSector.type.ToString());
        }

        private void InitializeNeghborins()
        {
            foreach (Sector s in sectors)
            {
                foreach (Sector ss in s.sectorsNeighborin)
                {
                    if (!neighborinsSectors.Contains(ss))
                    {
                        neighborinsSectors.Add(ss);
                    }
                }
            }
        }

        public Sector FindNeighborinSectorType(TypeSector typeSector)
        {
            return neighborinsSectors.Find((Sector s) =>
            { return s.type == typeSector; });
        }

        public Sector FindType(TypeSector typeSector)
        {
            return sectors.Find((Sector s) => { return s.type == typeSector; });
        }

        public List<Sector> FindAllType(TypeSector typeSector)
        {
            return sectors.FindAll((Sector s) => { return s.type == typeSector; });
        }
        /// <summary>
        /// Определяется имеются ли "пустые" сектора в соседях у какогото из секторов кластера
        /// </summary>
        /// <returns></returns>
        public bool HaveSectorNeighborinNullSector()
        {
            bool res = false;
            foreach (Sector sector in sectors)
            {
                if (sector.HaveNeighborinType(TypeSector.nul))
                    return true;
            }
            return res;
        }

        /// <summary>
        ///Присвоение нового типа всем секторам кластера
        /// </summary>
        /// <param name="newTypeSectors">Новый тип для секторов кластера</param>
        public void NewType(TypeSector newTypeSectors)
        {
            foreach (Sector sector in sectors)
            {
                sector.type = newTypeSectors;
            }
        }
    }

}
