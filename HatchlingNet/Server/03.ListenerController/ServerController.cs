using HatchlingNet;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class ServerController
    {
        NetworkService networkService = new NetworkService();

        int maxConnection;//모든 리스너들의 연결 맥스
        //int connectionCount;//모든 리스너들의 연결 총합
        int bufferSize = 1024;

        public Thread ListenerThread { get; private set; }
        Listener listener;

        public void Initialize(int maxConnection)
        {
            this.maxConnection = maxConnection;

            AllocateListener();
            AllocateBuffer();
            AllocateToken();
        }

        public void Listen(string host, int port, int backlog)
        {
            listener.Ready(host, port, backlog);

            //ListenerThread = new Thread(listener.DoListen);
            //ListenerThread.Start();
            listener.AcceptStart();
        }

        //멀티 리스너를 사용하게 되면 추가수정
        void AllocateListener()
        {
            listener = new Listener()
            {
                BeginReceive = networkService.BeginReceive
            };
            networkService.CloseClientSocket = listener.Disconnect;
        }

        void AllocateBuffer()
        {
            PacketBufferManager.Instance.Initialize(2000);
        }

        void AllocateToken()
        {
            UserTokenPool.Instance.Count = maxConnection;

            for (int i = 0; i < maxConnection; ++i)
                AddToken(i);
        }

        void AddToken(int i)
        {
            UserToken token = new UserToken();
            token.receiveEventArgs = GetArgs(token, networkService.ReceiveComplete);
            token.sendEventArgs = GetArgs(token, networkService.SendComplete);
            token.TokenID = i;
            token.Peer = new GameUser(token);
            UserTokenPool.Instance.Push(token);
        }

        SocketAsyncEventArgs GetArgs(UserToken token, EventHandler<SocketAsyncEventArgs> handler)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.UserToken = token;
            args.Completed += handler;
            args.SetBuffer(new byte[bufferSize], 0, bufferSize);

            return args;
        }
    }
}