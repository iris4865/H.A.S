using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatchlingNet
{
    class Program
    {
        NetworkService networkService;

        static void Main(string[] args)
        {
            Program server = new Program();
            server.Initialize();

            Console.WriteLine("Server Start");
            server.Update();


        }

        public void Initialize()
        {
            networkService = new NetworkService();
            networkService.Initialize();   
        }

        public void Update()
        {
            networkService.Listen("0.0.0.0", 7979, 1000);

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

   
    }
}
