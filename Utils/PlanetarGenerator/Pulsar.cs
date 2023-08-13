using System.Collections.Generic;

namespace Empire.PlanetarGenerator
{
    public class Pulsar
    {
        public Sector sectorPulsar;
        public List<Sector> sectorsNeighborinType1 = new List<Sector>(3);
        public List<Sector> sectorsNeighborinType2 = new List<Sector>(3);
        private TypeSector type1 = TypeSector.border;
        private TypeSector type2 = TypeSector.border;

        public Pulsar(Sector sector)
        {
            this.sectorPulsar = sector;
        }

        public void Initialize()
        {
            sectorPulsar.myPulsar = this;
            foreach (Sector s in sectorPulsar.sectorsNeighborin)
            {
                if (type1 == TypeSector.border)
                {
                    if (s.type != TypeSector.border)
                    {
                        type1 = s.type;
                        sectorsNeighborinType1.Add(s);
                    }
                }
                else
                {
                    if (type2 == TypeSector.border)
                    {
                        if (s.type != TypeSector.border)
                        {
                            type2 = s.type;
                            sectorsNeighborinType2.Add(s);
                        }
                    }
                    else
                    {
                        if (s.type == type1)
                            sectorsNeighborinType1.Add(s);
                        if (s.type == type2)
                            sectorsNeighborinType2.Add(s);
                    }
                }
            }
        }

        public bool FreeSector(Sector sector)
        {
            if (sector.type.Equals(type1))
            {
                if (sectorsNeighborinType1.Contains(sector) &&
                    !(sectorsNeighborinType1.Count > 1))
                    return false;
                else
                {
                    sectorsNeighborinType1.Remove(sector);
                    return true;
                }
            }
            if (sector.type.Equals(type2))
            {
                if (sectorsNeighborinType2.Contains(sector) &&
                    !(sectorsNeighborinType2.Count > 1))
                    return false;
                else
                {
                    sectorsNeighborinType2.Remove(sector);
                    return true;
                }
            }
            else
            {
                return true;
            }


        }
    }
}
