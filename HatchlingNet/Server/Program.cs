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


        static void Main(string[] args)
        {
            //Initialize();

            MySQLConnecter mysql = new MySQLConnecter("localhost", "anstjd");
            mysql.Open();
            mysql.SelectDatabase("test");
            mysql.ConnectDatabase();

            while (true)
            {
                try
                {
                    Console.Write("input: ");
                    mysql.ExcuteQuery(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("ERROR");
                }
            }
            mysql.SelectTable("userinfo");
            mysql.ShowColumns();

            Console.WriteLine("Server Start");

            //Update();
        }

        static public void Initialize()
        {
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
