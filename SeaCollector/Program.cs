using System;
using SeaCollector.HxObj;

namespace SeaCollector
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {

            ObjLoader.Load("G:/Modelle/Sameplayer_Flasche/OBJ/Flasche.obj");
            
            using var game = new Application();
            game.Run();
        }
    }
}