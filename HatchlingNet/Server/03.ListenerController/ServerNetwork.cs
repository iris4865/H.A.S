using HatchlingNet;
using System;
using System.Net.Sockets;

namespace Server
{
    //Date: 4. 21
    //NetworkController식이 더 어울려 보인다.
    public partial class ServerNetwork : NetworkService
    {
        BufferManager buffer_manager;

        //메시지 송수신객체, 풀링해서 사용예정
        SocketAsyncEventArgsPool receiveEventArgsPool = SocketAsyncEventArgsPool.receiveInstance;
        SocketAsyncEventArgsPool sendEventArgsPool = SocketAsyncEventArgsPool.sendInstance;

        int maxConnection;//모든 리스너들의 연결 맥스
        //int connectionCount;//모든 리스너들의 연결 총합
        int bufferSize;
        readonly int preAllocCount = 2;

        public ServerNetwork(int maxConnection)
        {
            this.maxConnection = maxConnection;
        }

        public void Initialize()
        {
            bufferSize = 1024;

            receiveEventArgsPool.Count = maxConnection;
            sendEventArgsPool.Count = maxConnection;

            //receiveEventArgsPool = new SocketAsyncEventArgsPool(maxConnection);
            //sendEventArgsPool = new SocketAsyncEventArgsPool(maxConnection);

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

        public void Listen(string host, int port, int backlog)
        {
            Listener listener = new Listener(maxConnection)
            {
                BeginReceive = BeginReceive
            };
            listener.Initialize();
            listener.Start(host, port, backlog);
        }


        public override void CloseClientSocket(UserToken token)
        {
            token.OnRemove();

            receiveEventArgsPool.Push(token.receiveEventArgs);
            sendEventArgsPool.Push(token.sendEventArgs);
        }
    }
}