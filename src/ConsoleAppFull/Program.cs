using System;

namespace ConsoleAppFull
{
    class MainClass
    {
        static holdem_engine.HandEngine engine = new holdem_engine.HandEngine();
        
        public static void Main(string[] args)
        {
            
            engine.DealHoleCards();
        }
    }
}
