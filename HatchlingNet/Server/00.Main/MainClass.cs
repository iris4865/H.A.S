﻿using Management;
using System;

namespace Server
{
    class MainClass
    {
        [STAThread]
        static void Main()
        {
            bool firstWpf = false;
            
            if (firstWpf)
                WpfInit();
            else
                ServerInit();
        }

        public static void ServerInit()
        {
            ServerApp server = new ServerApp();
            server.Initialize();
            server.Update();
        }

        static void WpfInit()
        {
            ConsoleScreen.OnScreen(false);
            new WpfApp().Start();
        }
    }
}
