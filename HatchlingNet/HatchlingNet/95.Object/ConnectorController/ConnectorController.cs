using System;
using System.Net.Sockets;

namespace HatchlingNet
{
    public class ConnectorController : NetworkService
    {
        public void ConnectProcess(Socket clientSocket, UserToken token)//클라에서 Initialize대신 사용
        {
            SocketAsyncEventArgs receiveEventArg = GetEventArgs(token, new EventHandler<SocketAsyncEventArgs>(CallReceiveComplete));
            SocketAsyncEventArgs sendEventArg = GetEventArgs(token, new EventHandler<SocketAsyncEventArgs>(CallSendComplete));

            BeginReceive(clientSocket, receiveEventArg, sendEventArg);
        }

        private SocketAsyncEventArgs GetEventArgs(UserToken token, EventHandler<SocketAsyncEventArgs> handler)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += handler;
            args.UserToken = token;
            args.SetBuffer(new Byte[1204], 0, 1024);

            return args;
        }

        public override void CloseClientSocket(UserToken token)
        {
            token.OnRemove();
        }
    }
}
