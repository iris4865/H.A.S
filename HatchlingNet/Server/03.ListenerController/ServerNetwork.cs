using HatchlingNet;
using System.Threading;

namespace Server
{
    //Date: 4. 21
    //NetworkController식이 더 어울려 보인다.
    public partial class ServerNetwork
    {
        NetworkService network = new NetworkService();
        BufferManager buffer_manager;

        //메시지 송수신객체, 풀링해서 사용예정
        SocketAsyncEventArgsPool receiveEventArgsPool = SocketAsyncEventArgsPool.receiveInstance;
        SocketAsyncEventArgsPool sendEventArgsPool = SocketAsyncEventArgsPool.sendInstance;

        int maxConnection;//모든 리스너들의 연결 맥스
        //int connectionCount;//모든 리스너들의 연결 총합
        int bufferSize;
        readonly int preAllocCount = 2;

        public Thread ListenerThread { get; private set; }
        Listener listener;

        public ServerNetwork(int maxConnection)
        {
            this.maxConnection = maxConnection;
            network.CloseClientSocket = CloseClientSocket;
        }

        public void Initialize()
        {
            bufferSize = 1024;

            receiveEventArgsPool.Count = maxConnection;
            sendEventArgsPool.Count = maxConnection;

            buffer_manager = new BufferManager(maxConnection * bufferSize * preAllocCount, bufferSize);
            buffer_manager.InitBuffer();

            //Pre-allocate a set of reusable SocketAsyncEventArgs
            for (int i = 0; i < maxConnection; ++i)
            {
                UserToken token = new UserToken();

                PushReceiveEventArgsPool(token);
                PushSendEventArgsPool(token);
            }
        }

        public void Listen(string host, int port, int backlog)
        {
            listener = new Listener(maxConnection)
            {
                BeginReceive = network.BeginReceive
            };
            listener.Initialize();

            listener.Ready(host, port, backlog);

            ListenerThread = new Thread(listener.DoListen);
            ListenerThread.Start();
        }

        public void CloseClientSocket(UserToken token)
        {
            listener.Disconnect(token);
        }
    }
}