﻿using System.Diagnostics;
using System.Threading;

namespace Server
{
    public class ServerApp
    {
        ServerController network;
        public Thread GetThread
        {
            get
            {
                return network.ListenerThread;
            }
        }
        int maxConnection = 10000;

        static void Main()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            ServerApp mainServer = new ServerApp();
            mainServer.Initialize();
            mainServer.Update();
        }

        public void Initialize()
        {
            Trace.WriteLine("Server Start");
            network = new ServerController();

            network.Initialize(maxConnection);
        }

        public void Update()//콘솔용
        {
            network.Listen("0.0.0.0", 7979, maxConnection);

            //Thread.Sleep(Timeout.Infinite);
            while (true)
                Thread.Sleep(10000);
        }

        public void Start()//gui용
        {
            network.Listen("0.0.0.0", 7979, maxConnection);
        }
    }
}