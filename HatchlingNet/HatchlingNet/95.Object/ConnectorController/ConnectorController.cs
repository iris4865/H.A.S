using System;
using System.Net.Sockets;

namespace HatchlingNet
{
    public class ConnectorController : NetworkService
    {
        public void ConnectProcess(Socket clientSocket, UserToken token)//클라에서 Initialize대신 사용
        {
            SocketAsyncEventArgs receiveEventArg = GetEventArgs(token, CallReceiveComplete);
            SocketAsyncEventArgs sendEventArg = GetEventArgs(token, CallSendComplete);

            //token에 집어넣고 보내는 방법도 있다.
            token.socket = clientSocket;
            token.sendEventArgs = sendEventArg;
            token.receiveEventArgs = receiveEventArg;

            BeginReceive(token);
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
