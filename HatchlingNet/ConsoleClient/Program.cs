using System;

namespace client
{
    class Program
    {

        static void Main(string[] args)
        {
            /**/
            Client testClient = new Client();
            testClient.Initialize();
            /**/
            /*
            int dummyClientCount = 100;
            Console.WriteLine("dummy client's number is " + dummyClientCount);
            DummyClient dummy = new DummyClient(dummyClientCount);
            /**/
        }

    }
}