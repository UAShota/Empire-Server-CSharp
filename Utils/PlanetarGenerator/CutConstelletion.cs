
using System.Collections.Generic;

namespace Empire.PlanetarGenerator
{
    public enum Side
    {
        left,
        right,
        up,
        down
    }

    public class CutConstelletion
    {
        public List<Sector> sectors = new List<Sector>(10);
        public TypeSector type;
        public Side side;
        private List<TypeSector> typeSectors = new List<TypeSector>(1);

        public CutConstelletion(TypeSector type, Side side)
        {
            this.type = type;
            this.side = side;
        }

        public void CutOn()
        {
            typeSectors.Clear();
            for (int i = sectors.Count; i > 0; i--)
            {
                typeSectors.Add(sectors[i - 1].type);
                sectors[i - 1].type = TypeSector.nul;
            }
        }

        public void CutOf()
        {
            for (int i = sectors.Count; i > 0; i--)
            {
                sectors[i - 1].type = typeSectors[i - 1];
            }
        }
    }
}
