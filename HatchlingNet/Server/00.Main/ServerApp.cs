using HatchlingNet;
using System;

namespace Server
{
    public class ServerApp
    {
        ServerNetwork network;

        //NumberingPool objNumberingPool;

        static void Main()
        {
            ServerApp mainServer = new ServerApp();
            mainServer.Initialize();
            mainServer.Update();
        }

        public void Initialize()
        {
            Console.WriteLine("Server Start");
            PacketBufferManager.Initialize(2000);
            //objNumberingPool = new NumberingPool(20000);

            network = new ServerNetwork(10000);
            network.Initialize();
        }

        public void Update()//콘솔용
        {
            Start();

            while (true)
                System.Threading.Thread.Sleep(10000);
        }

        public void Start()//gui용
        {
            network.Listen("0.0.0.0", 7979, 1000);
        }
    }
}