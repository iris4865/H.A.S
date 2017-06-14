using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace HatchlingNet
{
    public sealed class NetworkService
    {
        public Action<UserToken> CloseClientSocket;

        public void BeginReceive(UserToken userToken)
        {
            Socket clientSocket = userToken.socket;
            SocketAsyncEventArgs receiveArgs = userToken.receiveEventArgs;

            bool isAsync = clientSocket.ReceiveAsync(receiveArgs);
            if (!isAsync)
                ReceiveMessage(receiveArgs);
        }

        void ReceiveMessage(SocketAsyncEventArgs receiveArgs)
        {
            UserToken token = receiveArgs.UserToken as UserToken;
            if (IsSocketAlive(receiveArgs.BytesTransferred))
            {
                token.OpenMessage();
                BeginReceive(token);
            }
            else
                CloseClientSocket(receiveArgs.UserToken as UserToken);
        }

        public void SendComplete(object sender, SocketAsyncEventArgs sendArgs)
        {
            UserToken token = sendArgs.UserToken as UserToken;

            token.ProcessSend(sendArgs);
        }

        public void ReceiveComplete(object sender, SocketAsyncEventArgs receiveArgs)
        {
            if (receiveArgs.LastOperation == SocketAsyncOperation.Receive)
                ReceiveMessage(receiveArgs);
            else
                Trace.WriteLine("Message Receive Fail");
        }

        /// <summary>
        /// 메세지가 0 이상인지 확인한다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsSocketAlive(int value)
        {
            return value > 0;
        }
    }
}
