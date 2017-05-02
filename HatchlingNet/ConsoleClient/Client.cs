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
        static ConnectorController service;
         
        int index;

        public void Initialize(int i = 12)
        {
            this.index = i;
            PacketBufferManager.Initialize(2000);
            service = new ConnectorController();

            Connector connector = new Connector(service);
            connector.CallbackConnect += CallConnectGameserver;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7979);
            Console.WriteLine("connect start");
            connector.Connect(endPoint);//리시브도 이안에서 해결
            Console.WriteLine("connect end");

            this.whileActive();

            ((RemoteServerPeer)gameServer[0]).serverToken.Disconnect();
            //            Console.ReadKey();
        }

        private void whileActive()
        {
            int count = 0;
            while (true)
            {
                count++;

                //string line = index + ". ?";
                //Console.WriteLine("send: "+line);
                //Console.Write("> ");
                //string line = Console.ReadLine();

                /*

                if (line == "q")
                {
                    break;
                }
                */

                //Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ChatReq, (short)SEND_TYPE.BroadcastWithMe);
                //msg.Push(line);



                Console.Write("ID > ");
                string id = Console.ReadLine();

                Console.Write("password > ");
                string password = Console.ReadLine();

                Packet msg = PacketBufferManager.Pop((short)PROTOCOL.SignupReq, (short)SEND_TYPE.Single);
                msg.Push(id);
                msg.Push(password);



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
