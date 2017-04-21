using HatchlingNet;
using System;
using System.Net.Sockets;

namespace Server
{
    public class ListenerController : NetworkService
    {
        Listener clientListener;

        public SocketAsyncEventArgsPool receiveEventArgsPool;//메시지 수신객체, 풀링해서 사용예정
        public SocketAsyncEventArgsPool sendEventArgsPool;//메시지 전송객체, 풀링해서 사용예정
        BufferManager buffer_manager;

        int maxConnection;//모든 리스너들의 연결 맥스
        int connectionCount;//모든 리스너들의 연결 총합
        int bufferSize;
        readonly int preAllocCount = 2;



        public void Initialize()
        {
            maxConnection = 10000;
            bufferSize = 1024;

            receiveEventArgsPool = new SocketAsyncEventArgsPool(maxConnection);
            sendEventArgsPool = new SocketAsyncEventArgsPool(maxConnection);

            buffer_manager = new BufferManager(maxConnection * bufferSize * preAllocCount, bufferSize);
            buffer_manager.InitBuffer();

            //Pre-allocate a set of reusable SocketAsyncEventArgs
            for (int i = 0; i < this.maxConnection; ++i)
            {
                UserToken token = new UserToken();

                PushReceiveEventArgsPool(token);
                PushSendEventArgsPool(token);
            }
        }

        //receive pool
        private void PushReceiveEventArgsPool(UserToken token)
        {
            SocketAsyncEventArgs args = PreAllocateSocketAsyncEventArgs(token, new EventHandler<SocketAsyncEventArgs>(CallReceiveComplete));
            receiveEventArgsPool.Push(args);
        }

        //send pool
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
            clientListener = new Listener(this, receiveEventArgsPool, sendEventArgsPool);

            clientListener.receiveBeginTrigger = BeginReceive;

            clientListener.Start(host, port, backlog);
        }

        public override void CloseClientSocket(UserToken token)
        {
            token.OnRemove();

            receiveEventArgsPool.Push(token.receiveEventArgs);
            sendEventArgsPool.Push(token.sendEventArgs);
        }
    }
}
