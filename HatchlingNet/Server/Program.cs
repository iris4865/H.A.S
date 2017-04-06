using Server;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using DataBase;

namespace HatchlingNet
{
    class Program
    {
        static NetworkService networkService;
        static List<GameUser> userList;
        static MySQLConnecter mysql = new MySQLConnecter("localhost", "apmsetup");

        static void Main(string[] args)
        {

            Initialize();
            Console.WriteLine("Server Start");

            Update();
        }

        static public void Initialize()
        {
            mysql.Open();

            PacketBufferManager.Initialize(2000);
            userList = new List<GameUser>();

            networkService = new NetworkService();
            networkService.Initialize();
            networkService.callbackSessionCreate += CallSessionCreate;
        }

        static public void Update()
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
