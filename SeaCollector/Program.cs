using System;
using SeaCollector.HxObj;

namespace SeaCollector
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using var game = new Application();
            game.Run();
        }
    }
}