﻿using DataBase;
using HatchlingNet;
using System;

namespace Server
{
    class MainApp
    {
        static ListenerController networkService;
        static MySQLConnecter mysql = new MySQLConnecter("localhost", "apmsetup");

        static void Main(string[] args)
        {
            new MainApp().Initialize();
            Console.WriteLine("Server Start");

            new MainApp().Update();
        }

        public void Initialize()
        {
            mysql.Open();

            PacketBufferManager.Initialize(2000);
            InitNeworkService();
        }

        private void InitNeworkService()
        {
            networkService = new ListenerController();
            networkService.Initialize();
        }

        public void Update()
        {
            networkService.Listen("0.0.0.0", 7979, 1000);

            while (true)
                System.Threading.Thread.Sleep(10000);
        }
    }
}