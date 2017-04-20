using HatchlingNet;
using System;
using System.Net.Sockets;

namespace Network
{
    public class ListenerController
    {
        Listener clientListener;

        public SocketAsyncEventArgsPool receiveEventArgsPool;//메시지 수신객체, 풀링해서 사용예정
        public SocketAsyncEventArgsPool sendEventArgsPool;//메시지 전송객체, 풀링해서 사용예정
        BufferManager buffer_manager;//나중에가면 패킷매니저 씀...일단은 에코서버 될때까지 놔두자

        int maxConnection;//모든 리스너들의 연결 맥스
        int connectionCount;//모든 리스너들의 연결 총합
        int bufferSize;
        readonly int preAllocCount = 2;

        public delegate void SessionHandler(Socket socket, UserToken token);
        public SessionHandler CallbackSessionCreate { get; set; }

        public void Initialize()
        {
            maxConnection = 10000;
            bufferSize = 1024;

            receiveEventArgsPool = new SocketAsyncEventArgsPool(maxConnection);
            sendEventArgsPool = new SocketAsyncEventArgsPool(maxConnection);

            buffer_manager = new BufferManager(maxConnection * bufferSize * preAllocCount, bufferSize);
            buffer_manager.InitBuffer();

            for (int i = 0; i < this.maxConnection; ++i)
            {
                UserToken token = new UserToken();

                PushReceiveEventArgsPool(token);
                PushSendEventArgsPool(token);
            }
        }

        //receive pool
        //Pre-allocate a set of reusable SocketAsyncEventArgs
        private void PushReceiveEventArgsPool(UserToken token)
        {
            SocketAsyncEventArgs args = PreAllocateSocketAsyncEventArgs(token, new EventHandler<SocketAsyncEventArgs>(CallReceiveComplete));
            receiveEventArgsPool.Push(args);
        }

        //send pool
        //Pre-allocate a set of reusable SocketAsyncEventArgs
        private void PushSendEventArgsPool(UserToken token)
        {
            SocketAsyncEventArgs args = PreAllocateSocketAsyncEventArgs(token, new EventHandler<SocketAsyncEventArgs>(CallSendComplete));
            receiveEventArgsPool.Push(args);
        }

        private SocketAsyncEventArgs PreAllocateSocketAsyncEventArgs(UserToken token, EventHandler<SocketAsyncEventArgs> handler)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(CallReceiveComplete);
            args.UserToken = token;

            buffer_manager.SetBuffer(args);

            return args;
        }

        public void Listen(string host, int port, int backlog)
        {
            clientListener = new Listener(this);

            clientListener.Start(host, port, backlog);
        }

        public void BeginReceive(Socket clientSocket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            UserToken token = receiveArgs.UserToken as UserToken;
            token.receiveEventArgs = receiveArgs;
            token.sendEventArgs = sendArgs;

            token.socket = clientSocket;

            bool pending = clientSocket.ReceiveAsync(receiveArgs);
            if (!pending)
            {
                Console.WriteLine("비긴 리시브");
                ProcessReceive(receiveArgs);
            }
        }

        public void CallReceiveComplete(object sender, SocketAsyncEventArgs receiveArgs)
        {
            if (receiveArgs.LastOperation == SocketAsyncOperation.Receive)
            {
                ProcessReceive(receiveArgs);
                Console.WriteLine("콜백 리시브컴플리트!");
                return;
            }
            else
            {
                Console.WriteLine("콜백 리시브컴플리트 실패!");
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs receiveArgs)
        {
            UserToken token = receiveArgs.UserToken as UserToken;

            if (receiveArgs.BytesTransferred > 0)
            {
                //e.Buffer : 클라로부터 수신된 데이터, e.offset : 버퍼의 포지션, e.ByesTransferred : 이번에 수신된 바이트의 수
                token.OpenMessage(receiveArgs.Buffer, receiveArgs.Offset, receiveArgs.BytesTransferred);

                Console.WriteLine("대기");
                bool pending = token.socket.ReceiveAsync(receiveArgs);
                if (!pending)
                {
                    ProcessReceive(receiveArgs);
                }//비동기로 한번이라도 처리되는 순간 함수 나가게 되니 스택에 ProcessReceive가 계속 쌓이는건 아닌지에 대한 걱정은 안해도 된다.

                Console.WriteLine("지나감");

            }
            else
            {
                Console.WriteLine(string.Format("error {0}, transferred {1}", receiveArgs.SocketError, receiveArgs.BytesTransferred));
                CloseClientSocket(token);
            }
        }

        public void CloseClientSocket(UserToken token)
        {
            token.OnRemove();

            if (this.receiveEventArgsPool != null)
            {
                this.receiveEventArgsPool.Push(token.receiveEventArgs);
            }

            if (this.sendEventArgsPool != null)
            {
                this.sendEventArgsPool.Push(token.sendEventArgs);
            }
        }

        public void CallSendComplete(object sender, SocketAsyncEventArgs sendArgs)
        {
            UserToken token = sendArgs.UserToken as UserToken;

            token.ProcessSend(sendArgs);
        }

    }
}
