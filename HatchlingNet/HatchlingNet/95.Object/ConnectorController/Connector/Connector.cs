using System;
using System.Net;
using System.Net.Sockets;

namespace HatchlingNet
{
    public class Connector
    {
        Socket client;

        public Action<Socket, UserToken> ConnectProcess { private get => ConnectProcess; set => ConnectProcess = value; }
        public Action<UserToken> CallbackConnect;

        public IPEndPoint RemoteEndPoint
        {
            private get => RemoteEndPoint;
            set
            {
                if (RemoteEndPoint == null)
                    RemoteEndPoint = value;
            }
        }

        public Connector(IPEndPoint remoteEndPoint)
        {
            RemoteEndPoint = remoteEndPoint;
        }

        public void Connect()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
            eventArgs.Completed += CallConnectComplete;
            eventArgs.RemoteEndPoint = RemoteEndPoint;

            bool pending = client.ConnectAsync(eventArgs);
            if (!pending)
                CallConnectComplete(null, eventArgs);
        }

        void CallConnectComplete(object sender, SocketAsyncEventArgs args)
        {
            UserToken token = new UserToken();
            ConnectProcess(client, token);

            CallbackConnect?.Invoke(token);
        }
    }
}
