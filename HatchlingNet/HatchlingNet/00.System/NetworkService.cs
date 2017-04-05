using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;


namespace HatchlingNet
{
    public class NetworkService
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
        public SessionHandler callbackSessionCreate { get; set; }

        public NetworkService()
        {
            this.connectionCount = 0;
        }

        public void Initialize()//서버에서만 호출...클라에선 안호출...
        {
            this.maxConnection = 10000;
            this.bufferSize = 1024;

            this.receiveEventArgsPool = new SocketAsyncEventArgsPool(this.maxConnection);
            this.sendEventArgsPool = new SocketAsyncEventArgsPool(this.maxConnection);

            this.buffer_manager = new BufferManager(this.maxConnection * this.bufferSize * this.preAllocCount, this.bufferSize);
            this.buffer_manager.InitBuffer();

            SocketAsyncEventArgs arg;

            for (int i = 0; i < this.maxConnection; ++i)
            {
                UserToken token = new UserToken();
//                token.callbackBroadcast = Call
                //receive pool
                {
                    //Pre-allocate a set of reusable SocketAsyncEventArgs
                    arg = new SocketAsyncEventArgs();
                    arg.Completed += new EventHandler<SocketAsyncEventArgs>(CallReceiveComplete);
                    arg.UserToken = token;

                    this.buffer_manager.SetBuffer(arg);

                    this.receiveEventArgsPool.Push(arg);
                }

                //send pool
                {
                    //Pre-allocate a set of reusable SocketAsyncEventArgs
                    arg = new SocketAsyncEventArgs();
                    arg.Completed += new EventHandler<SocketAsyncEventArgs>(CallSendComplete);
                    arg.UserToken = token;

                    this.buffer_manager.SetBuffer(arg);

                    this.sendEventArgsPool.Push(arg);
                }
            }
        }

        public void ConnectProcess(Socket clientSocket, UserToken token)//클라에서 Initialize대신 사용
        {
            SocketAsyncEventArgs receiveEventArg = new SocketAsyncEventArgs();
            receiveEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(CallReceiveComplete);
            receiveEventArg.UserToken = token;
            receiveEventArg.SetBuffer(new Byte[1204], 0, 1024);

            SocketAsyncEventArgs sendEventArg = new SocketAsyncEventArgs();
            sendEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(CallSendComplete);
            sendEventArg.UserToken = token;
            sendEventArg.SetBuffer(new Byte[1024], 0, 1024);


            BeginReceive(clientSocket, receiveEventArg, sendEventArg);
        }




        public void Listen(string host, int port, int backlog)
        {
            clientListener = new Listener(this);

            clientListener.receiveBeginTrigger = BeginReceive;

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

            if (receiveArgs.BytesTransferred > 0 )
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
