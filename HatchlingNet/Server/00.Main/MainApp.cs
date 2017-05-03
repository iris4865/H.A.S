using HatchlingNet;
using System;
using Management;

namespace Server
{
    class MainApp
    {
        ListenerController listenerController;
        Monitor monitor = Monitor.Instance;
//        NumberingPool objNumberingPool;

        static void Main(string[] args)
        {
            Console.WriteLine("Server Start");

            MainApp mainApp = new MainApp();
            mainApp.Initialize();
            mainApp.Update();
        }

        public void Initialize()
        {
            PacketBufferManager.Initialize(2000);
  //          objNumberingPool = new NumberingPool(20000);

            listenerController = new ListenerController(10000);
            listenerController.Initialize();
        }

        public void Update()
        {
            listenerController.Start("0.0.0.0", 7979, 1000);
            monitor.Start();

            while (true)
                System.Threading.Thread.Sleep(10000);
        }
    }
}
