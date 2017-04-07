using DataBase;
using Server;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace HatchlingNet
{
    class MainServer
    {
        static NetworkService networkService;
        static List<GameUser> userList;
        static MySQLConnecter mysql = new MySQLConnecter("localhost", "apmsetup");

        static void Main(string[] args)
        {
            new MainServer().Initialize();
            Console.WriteLine("Server Start");

            new MainServer().Update();
        }

        public void Initialize()
        {
            mysql.Open();

            PacketBufferManager.Initialize(2000);

            userList = new List<GameUser>();
            InitNeworkService();
        }

        private void InitNeworkService()
        {
            networkService = new NetworkService();
            networkService.Initialize();

            networkService.CallbackSessionCreate = CallSessionCreate;
        }

        public void Update()
        {
            networkService.Listen("0.0.0.0", 7979, 1000);

            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }

        static void CallSessionCreate(Socket socket, UserToken token)
        {
            GameUser user = new GameUser(token);
            user.mysql = mysql;

            lock (userList)
            {
                userList.Add(user);
            }
        }

        public static void RemoveUser(GameUser user)
        {
            lock (userList)
            {
                userList.Remove(user);
            }
        }
    }
}
