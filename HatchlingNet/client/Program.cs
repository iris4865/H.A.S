using HatchlingNet;
using INTERFACE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        static List<Peer> gameServer = new List<Peer>();
        static NetworkService service;

        static void Main(string[] args)
        {
            Initialize();


        }

        static public void Initialize()
        {
            PacketBufferManager.Initialize(2000);
            service = new NetworkService();

            Connector connector = new Connector(service);
            connector.callbackConnect += CallConnectGameserver;



            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7979);
            connector.connect(endPoint);//리시브도 이안에서 해결

            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();

                if (line == "q")
                {
                    break;
                }

                Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ChatReq);
                msg.Push(line);
                gameServer[0].Send(msg);
            }

            ((RemoteServerPeer)gameServer[0]).serverToken.disconnect();
//            Console.ReadKey();
        }

        static void CallConnectGameserver(UserToken serverToken)
        {
            lock (gameServer)
            {
                // Peer server = new CRemote

                Peer server = new RemoteServerPeer(serverToken);
                gameServer.Add(server);

            }
        }

        public void Update()
        {

        }
    }
}
