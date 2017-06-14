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
        int bufferSize = 1024;
        readonly int preAllocCount = 2;

        public Thread ListenerThread { get; private set; }
        Listener listener;

        public void Initialize(int maxConnection)
        {
            this.maxConnection = maxConnection;

            AllocateListener();
            AllocateBuffer();
            AllocateEventArgsPool();
        }

        //멀티 리스너를 사용할 때 수정
        void AllocateListener()
        {
            listener = new Listener(maxConnection)
            {
                BeginReceive = network.BeginReceive
            };
            network.CloseClientSocket = listener.Disconnect;
        }

        void AllocateBuffer()
        {
            PacketBufferManager.Instance.Initialize(2000);

            buffer_manager = new BufferManager(maxConnection * bufferSize * preAllocCount, bufferSize);
            buffer_manager.InitBuffer();
        }

        void AllocateEventArgsPool()
        {
            SocketAsyncEventArgsPool.receiveInstance.Count = maxConnection;
            SocketAsyncEventArgsPool.sendInstance.Count = maxConnection;

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
            listener.Initialize();

            listener.Ready(host, port, backlog);

            ListenerThread = new Thread(listener.DoListen);
            ListenerThread.Start();
        }
    }
}