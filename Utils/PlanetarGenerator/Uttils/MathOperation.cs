using System;

namespace ConsoleApp1.Uttils
{
    public static class MathOperation
    {
        public static double Distanse(int x1, int y1, int x2, int y2)
        {
            double distanse = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            return distanse;
        }

    }
}
