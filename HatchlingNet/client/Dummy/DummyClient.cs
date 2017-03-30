
using System;
using System.Threading;

namespace client
{
    class DummyClient
    {
        Thread[] clients;
        int ClientTotalCount;

        public DummyClient(int count)
        {
            ClientTotalCount = count;
            this.Initialize();
        }

        private void Initialize()
        {
            clients = new Thread[this.ClientTotalCount];

            for (int i = 0; i < this.ClientTotalCount; i++)
            {
                Console.WriteLine("start: "+i);
                clients[i] = new Thread(() => ActiveClient(i));
                clients[i].Start();
            }

            while (true)
            {
                int clientThreadFinishCount = 0;
                for (int i = 0; i < this.ClientTotalCount; i++)
                {
                    if (!clients[i].IsAlive)
                        clientThreadFinishCount++;
                }
                if (clientThreadFinishCount.Equals(this.ClientTotalCount))
                    return;

                Thread.Sleep(1000);
            }
        }

        private void ActiveClient(int i)
        {
            Client client = new Client();

            client.Initialize(i);
        }
    }
}