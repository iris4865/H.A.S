using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CustomServer
{
    class Program
    {
        static GameServer gameMain;
        static NetworkService networkService;
        static List<GameUser> userList;


        static void Main(string[] args)
        {
            Initialize();

            Console.WriteLine("Server Start");

            Update();
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
                gameMain.userDiconnec
            }
        }


    }
}
