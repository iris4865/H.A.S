using System;
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
            bool pending = clientSocket.ReceiveAsync(receiveArgs);
            if (!pending)
            {
//                Console.WriteLine("비긴 리시브");
                ProcessReceive(receiveArgs);
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs receiveArgs)
        {
            UserToken token = receiveArgs.UserToken as UserToken;

            if (!IsArgsSocketClosed(receiveArgs.BytesTransferred))
            {
                //e.Buffer : 클라로부터 수신된 데이터, e.offset : 버퍼의 포지션, e.ByesTransferred : 이번에 수신된 바이트의 수
                token.OpenMessage(receiveArgs.Buffer, receiveArgs.Offset, receiveArgs.BytesTransferred);

//                Console.WriteLine("대기");
                bool pending = token.socket.ReceiveAsync(receiveArgs);
                if (!pending)
                {
                    ProcessReceive(receiveArgs);
                }//비동기로 한번이라도 처리되는 순간 함수 나가게 되니 스택에 ProcessReceive가 계속 쌓이는건 아닌지에 대한 걱정은 안해도 된다.

//                Console.WriteLine("지나감");

            }
            else
                CloseClientSocket(token);
        }

        bool IsArgsSocketClosed(int value) => value <= 0;

        public void CallSendComplete(object sender, SocketAsyncEventArgs sendArgs)
        {
            UserToken token = sendArgs.UserToken as UserToken;

            token.ProcessSend(sendArgs);
        }

        public void CallReceiveComplete(object sender, SocketAsyncEventArgs receiveArgs)
        {
            if (receiveArgs.LastOperation == SocketAsyncOperation.Receive)
            {
                ProcessReceive(receiveArgs);
                //                Console.WriteLine("콜백 리시브컴플리트!");
            }
            else
            {
                //              Console.WriteLine("콜백 리시브컴플리트 실패!");
            }
        }
    }
}
