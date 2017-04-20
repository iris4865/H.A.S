using HatchlingNet;
using System;
using System.Net.Sockets;

namespace Server
{
    public class ListenerController : NetworkService
    {
//        NetworkService service;
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

        public ListenerController()
        {
//            service = new NetworkService();
        }

        public void Initialize()//서버에서만 호출...클라에선 안호출...
        {
            maxConnection = 10000;
            bufferSize = 1024;

            receiveEventArgsPool = new SocketAsyncEventArgsPool(maxConnection);
            sendEventArgsPool = new SocketAsyncEventArgsPool(maxConnection);

            buffer_manager = new BufferManager(maxConnection * bufferSize * preAllocCount, bufferSize);
            buffer_manager.InitBuffer();

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

        public void Listen(string host, int port, int backlog)
        {
            clientListener = new Listener(this);

            clientListener.receiveBeginTrigger = service.BeginReceive;

            clientListener.Start(host, port, backlog);
        }

        public override void CloseClientSocket(UserToken token)
        {
            receiveEventArgsPool.Push(token.receiveEventArgs);
            sendEventArgsPool.Push(token.sendEventArgs);
        }
    }
}
