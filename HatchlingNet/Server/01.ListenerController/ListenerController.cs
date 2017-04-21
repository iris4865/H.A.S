using HatchlingNet;
using System;
using System.Net.Sockets;

namespace Server
{
    //Date: 4. 21
    //NetworkController식이 더 어울려 보인다.
    public class ListenerController : NetworkService
    {
        BufferManager buffer_manager;

        //메시지 송수신객체, 풀링해서 사용예정
        SocketAsyncEventArgsPool receiveEventArgsPool;
        SocketAsyncEventArgsPool sendEventArgsPool;

        int maxConnection;//모든 리스너들의 연결 맥스
        int connectionCount;//모든 리스너들의 연결 총합
        int bufferSize;
        readonly int preAllocCount = 2;

        public ListenerController(int maxConnection)
        {
            this.maxConnection = maxConnection;
        }

        public void Initialize()
        {
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
            args.Completed += handler;
            args.UserToken = token;

            buffer_manager.SetBuffer(args);

            return args;
        }

        public override void CloseClientSocket(UserToken token)
        {
            token.OnRemove();

            receiveEventArgsPool.Push(token.receiveEventArgs);
            sendEventArgsPool.Push(token.sendEventArgs);
        }

        public SocketAsyncEventArgs PopReceiveEventArgs()
        {
            return receiveEventArgsPool.Pop();
        }

        public SocketAsyncEventArgs PopSendEventArgs()
        {
            return sendEventArgsPool.Pop();
        }
    }
}
