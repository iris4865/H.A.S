using HatchlingNet;
using System;

namespace Server
{
    class ServerApp
    {
        ListenerController listenerController;
        //NumberingPool objNumberingPool;

        public void Initialize()
        {
            Console.WriteLine("Server Start");
            PacketBufferManager.Initialize(2000);
            //objNumberingPool = new NumberingPool(20000);

            listenerController = new ListenerController(10000);
            listenerController.Initialize();
        }

        public void Update()
        {
            Start();

            while (true)
                System.Threading.Thread.Sleep(10000);
        }

        public void Start()
        {
            listenerController.Start("0.0.0.0", 7979, 1000);
        }
    }
}
