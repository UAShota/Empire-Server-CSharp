using System.Collections.Generic;
using config = Empire.PlanetarGenerator.Configuration;

namespace Empire.PlanetarGenerator
{
    public class SectorsCollection
    {
        private List<Sector> sectors;
        private Sector[,] sectorsTable;
        public List<Sector> averages;
        public List<Sector> externals;
        public List<Sector> iternals;
        public List<Sector> emtys;
        public List<Sector> freeBorderItAv;
        public List<Sector> freeBorderAvEx;
        public List<Claster> clasters = new List<Claster>();
        public List<Claster> nullClasters = new List<Claster>();
        public List<CutConstelletion> cuts = new List<CutConstelletion>(8);
        public List<Pulsar> pulsarsAvEx = new List<Pulsar>(config.maxPulsarAvEx);
        public List<Pulsar> pulsarsItAv = new List<Pulsar>(config.maxPulsarItAv);

        public void SetNumsSectors(int numsColumAndLine)
        {
            sectors = new List<Sector>(numsColumAndLine * numsColumAndLine);
            sectorsTable = new Sector[numsColumAndLine, numsColumAndLine];
            emtys = new List<Sector>(numsColumAndLine * numsColumAndLine);
            averages = new List<Sector>(numsColumAndLine * numsColumAndLine);
            iternals = new List<Sector>(numsColumAndLine * numsColumAndLine);
            externals = new List<Sector>(numsColumAndLine * numsColumAndLine);
            freeBorderAvEx = new List<Sector>(numsColumAndLine * 5);
            freeBorderItAv = new List<Sector>(numsColumAndLine * 5);
        }

        public void AddSector(Sector sector, int colum, int line)
        {
            emtys.Add(sector);
            sectors.Add(sector);
            sectorsTable[colum, line] = sector;
        }

        public Sector GetIndex(int index)
        {
            return sectors[index];
        }

        public Sector GetColumLine(int numColum, int numLine)
        {
            return sectorsTable[numColum, numLine];
        }

        public List<Sector> ListAll()
        {
            return sectors;
        }

        public int Length()
        {
            return sectors.Count;
        }



    }
}
