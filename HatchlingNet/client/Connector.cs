using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HatchlingNet;
using System.Net.Sockets;

namespace client
{
    class Connector
    {
        private NetworkService networkService;
        private Socket client;

        public delegate void ConnectHandler(UserToken token);
        public ConnectHandler callbackConnect { get; set; }


        public Connector(NetworkService networkService)
        {
            this.networkService = networkService;
                

        }

        public void connect(IPEndPoint remoteEndPoint)
        {
            this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
            eventArgs.Completed += CallConnectComplete;
            eventArgs.RemoteEndPoint = remoteEndPoint;


            bool pending = this.client.ConnectAsync(eventArgs);
            if (!pending)
            {
                CallConnectComplete(null, eventArgs);
            }

            
        }

        public void CallConnectComplete(object sender, SocketAsyncEventArgs args)
        {
            UserToken token = new UserToken();
//            token.sendEventArgs = 
            this.networkService.ConnectProcess(this.client, token);

            if (this.callbackConnect != null)
            {
                this.callbackConnect(token);
            }
            
        }


    }
}
