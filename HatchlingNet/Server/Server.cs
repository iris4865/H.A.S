using DataBase;
using HatchlingNet;
using System;

namespace Server
{
    class Server
    {
        static NetworkService networkService;
        static MySQLConnecter mysql = new MySQLConnecter("localhost", "apmsetup");

        static void Main(string[] args)
        {
            new Server().Initialize();
            Console.WriteLine("Server Start");

            new Server().Update();
        }

        public void Initialize()
        {
            mysql.Open();

            PacketBufferManager.Initialize(2000);
            InitNeworkService();
        }

        private void InitNeworkService()
        {
            networkService = new NetworkService();
            networkService.Initialize();

            networkService.CallbackSessionCreate = UserList.CallSessionCreate;
        }

        public void Update()
        {
            networkService.Listen("0.0.0.0", 7979, 1000);

            while (true)
                System.Threading.Thread.Sleep(10000);
        }
    }
}
