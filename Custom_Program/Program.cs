using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

using SplashKitSDK;

namespace Custom_Program
{
    public class Program
    {
        public static void Main()
        {
            Monopoly game = new Monopoly(new string[] { "Player 1", "Player 2" });
            Window w = new Window("Custom Program", 1200, 750);
            do
            {
                SplashKit.ProcessEvents();
                game.HandleInput();
                SplashKit.ClearScreen(Color.White);
                game.Draw();
                game.Update();
                SplashKit.RefreshScreen();
            } while (!SplashKit.WindowCloseRequested("Custom Program"));
        }
    }
}


