using System;
using System.Net;
using System.Net.Sockets;

namespace HatchlingNet
{
    public class Connector
    {
        Socket client;

        public Action<Socket, UserToken> ConnectProcess { private get; set; }
        public Action<UserToken> CallbackConnect;

        public void Connect(IPEndPoint remoteEndPoint)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
            eventArgs.Completed += CallConnectComplete;
            eventArgs.RemoteEndPoint = remoteEndPoint;

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
