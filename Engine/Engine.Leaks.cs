/////////////////////////////////////////////////
//
// Модуль обработки утечек
//
// Copyright(c) 2016 UAShota
//
// Rev 0  2020.02.29
//
/////////////////////////////////////////////////
using System;
using System.Collections.Generic;

namespace Empire.EngineSpace
{
    internal static class Leaks
    {
        public static List<object> fLeaks = new List<object>();

        public static void Enter(object aLeak)
        {
            fLeaks.Add(aLeak);
        }

        public static void Leave(object aLeak)
        {
            if (!fLeaks.Remove(aLeak))
                Console.WriteLine("no leak instance");
        }
    }

    internal class Leak : IDisposable
    {
        public Leak()
        {
            Leaks.Enter(this);
        }

        public virtual void Dispose()
        {
            Leaks.Leave(this);
        }
    }
}