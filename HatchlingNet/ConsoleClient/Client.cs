using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;

using HatchlingNet;

namespace client
{
    public class Client
    {
        static List<IPeer> gameServer = new List<IPeer>();
        static NetworkService service;

        int index;

        public void Initialize(int i = 12)
        {
            this.index = i;
            PacketBufferManager.Initialize(2000);
            service = new NetworkService();

            Connector connector = new Connector(service);
            connector.callbackConnect += CallConnectGameserver;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7979);
            Console.WriteLine("connect start");
            connector.connect(endPoint);//리시브도 이안에서 해결
            Console.WriteLine("connect end");

            this.whileActive();

            ((RemoteServerPeer)gameServer[0]).serverToken.disconnect();
            //            Console.ReadKey();
        }

        private void whileActive()
        {
            int count = 0;
            while (true)
            {
                count++;
                //Console.Write("> ");
                //string line = Console.ReadLine();
                string line = index + ". ?";
                Console.WriteLine("send: "+line);


                /*

                if (line == "q")
                {
                    break;
                }
                */

                Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ChatReq);
                msg.Push(line);
                Thread.Sleep(1000);
                gameServer[0].Send(msg);
            }
        }

        public void CallConnectGameserver(UserToken serverToken)
        {
            lock (gameServer)
            {
                // Peer server = new CRemote

                IPeer server = new RemoteServerPeer(serverToken);
                gameServer.Add(server);

            }
        }

        public void Update()
        {

        }
    }
}
