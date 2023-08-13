using System;
using Empire.EngineSpace;

namespace Empire
{
    internal class Program
    {
        private static void Main()
        {
            Core.Start();
            Console.ReadLine();
            Core.Stop();
            Console.ReadLine();
        }
    }
}