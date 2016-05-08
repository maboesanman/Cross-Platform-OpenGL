using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cross_Platform_OpenGL.Windows
{
    class Program
    {  
        static void Main(string[] args)
        {
            GameView myGameView = new GameView();
            myGameView.Run(30.0);
        }
    }
}
