using HatchlingNet;
using MySQL.Adapter;
using System;

namespace Server
{
    class MainApp
    {
        ListenerController listenerController;
        static MysqlCommand command;

        static void Main(string[] args)
        {
            Console.WriteLine("Server Start");
            
            MainApp mainApp = new MainApp();
            mainApp.Initialize();
            mainApp.Update();
        }

        public void Initialize()
        {
            command = new MysqlCommand();
            //db id, password 입력
            command.ConnectMysql("localhost", "id", "password");

            PacketBufferManager.Initialize(2000);

            listenerController = new ListenerController(10000);
            listenerController.Initialize();
        }

        public void Update()
        {
            listenerController.Start("0.0.0.0", 7979, 1000);

            while (true)
                System.Threading.Thread.Sleep(10000);
        }
    }
}
