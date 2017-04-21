using DataBase;
using HatchlingNet;
using System;

namespace Server
{
    class MainApp
    {
        Listener mainListener;
        //static ListenerController listenerController;
        static MySQLConnecter mysql = new MySQLConnecter("localhost", "apmsetup");

        static void Main(string[] args)
        {
            Console.WriteLine("Server Start");

            MainApp mainApp = new MainApp();
            mainApp.Initialize();
            mainApp.Update();
        }

        public void Initialize()
        {
            mysql.Open();

            PacketBufferManager.Initialize(2000);

            mainListener = new Listener(10000);
            mainListener.Initialize();
        }

        public void Update()
        {
            mainListener.Start("0.0.0.0", 7979, 1000);

            while (true)
                System.Threading.Thread.Sleep(10000);
        }
    }
}
