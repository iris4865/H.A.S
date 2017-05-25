using System;
using System.Net.Sockets;

namespace HatchlingNet
{
    public class ConnectorController
    {
        NetworkService network = new NetworkService();

        public ConnectorController()
        {
            network.CloseClientSocket = CloseClientSocket;
        }

        public void ConnectProcess(Socket clientSocket, UserToken token)//클라에서 Initialize대신 사용
        {
            //token에 집어넣고 보내는 방법도 있다.
            token.socket = clientSocket;
            token.sendEventArgs = GetEventArgs(token, network.CallSendComplete);
            token.receiveEventArgs = GetEventArgs(token, network.CallReceiveComplete);

            network.BeginReceive(token);
        }

        SocketAsyncEventArgs GetEventArgs(UserToken token, EventHandler<SocketAsyncEventArgs> handler)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += handler;
            args.UserToken = token;
            args.SetBuffer(new Byte[1204], 0, 1024);

            return args;
        }

        void CloseClientSocket(UserToken token)
        {
            token.OnRemove();
        }
    }
}
