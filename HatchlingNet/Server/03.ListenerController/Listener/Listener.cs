using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Listener
    {
        public Action<UserToken> BeginReceive;//Action을 이용하면 델리게이트 사용안하고 짦게 사용가능 대신 인자 하나만 넘길수 있음.
        SocketAsyncEventArgs acceptArgs; //비동기 Accept를 위한 객체;
        Socket listenSocket;

        Dictionary<int, UserToken> tokenList;
        NumberingPool tokenNumberingPool;

        AutoResetEvent flowController;

        int maxConnection;
        int connectionCount;
        //        int assignIDToUser;
        //int bufferSize;


        public Listener(int maxConnection)
        {
            this.maxConnection = maxConnection;
        }

        public void Initialize()
        {
            //            assignIDToUser = 0;

            tokenList = new Dictionary<int, UserToken>();
            this.acceptArgs = new SocketAsyncEventArgs();//SocketAsyncEventArgs 라고하는 비동기 객체 생성 

            tokenNumberingPool = new NumberingPool(10000);
            for (int i = 0; i < tokenNumberingPool.capacity; ++i)
            {
                int number = new int();
                number = i;

                tokenNumberingPool.Push(number);
            }


        }

        public void Start(string host, int port, int backlog)//backlog : 대기큐의 크기
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address;
            if (host == "0.0.0.0")
            {
                address = IPAddress.Any;
            }
            else
            {
                address = IPAddress.Parse(host);
            }

            IPEndPoint endpoint = new IPEndPoint(address, port);

            this.listenSocket.Bind(endpoint); //호스트의 정보 등록
            this.listenSocket.Listen(backlog); //받아들일 클라이언트 수 결정

            this.acceptArgs.Completed += AcceptComplete;
            //연결이 되었을경우 호출할 콜백함수의 핸들러를 Completed에 저장함...다만 연결이 되었는지에 대한 검사는 별도로 해야됨. 그게 바로 아래 코드

            Thread listenThread = new Thread(DoListen);
            listenThread.Start();
        }

        public void DoListen()
        {
            this.flowController = new AutoResetEvent(false);

            while (true)
            {
                this.acceptArgs.AcceptSocket = null;
                bool pending = true;

                pending = listenSocket.AcceptAsync(this.acceptArgs);
                //반환값이 false이면 동기적 처리. 동기적 처리이면 콜백함수 호출 안됨
                //listenSocket에만 콜백 등록해놨지 다른 소켓에는 콜백 등록 안했으니
                //연결됨으로써 새롭게 만들어지는 소켓에는 콜백이 없는거임 착각 ㄴㄴ

                if (!pending)
                {
                    AcceptComplete(null, this.acceptArgs);
                }

                this.flowController.WaitOne();
            }
        }

        public void AcceptComplete(Object sender, SocketAsyncEventArgs e)
        {
            //            Console.WriteLine("액셉컴플리트");

            if (e.SocketError == SocketError.Success)
            {
                Socket clientSocket = e.AcceptSocket;

                Interlocked.Increment(ref this.connectionCount);
                SocketAsyncEventArgs receiveArgs = SocketAsyncEventArgsPool.receiveInstance.Pop();
                SocketAsyncEventArgs sendArgs = SocketAsyncEventArgsPool.sendInstance.Pop();

                UserToken userToken = receiveArgs.UserToken as UserToken;

                //여기서 send, receive, socket 다 연결하는데 BeginReceive에 userToken을 주면 안되나?
                userToken.socket = clientSocket;
                userToken.sendEventArgs = sendArgs;
                userToken.receiveEventArgs = receiveArgs;
                userToken.TokenID = tokenNumberingPool.Pop();//연결해지시 push해주는거 아직 안함
                userToken.CallbackBroadcast = CallBroadCast;
                userToken.CallbackSendTo = CallSendTo;

                lock (userToken)
                {
                    tokenList.Add(userToken.TokenID, userToken);
                }

                UserList.Instance.SessionCreate(clientSocket, userToken);

                BeginReceive(userToken);

                flowController.Set();

                return;
            }
            else
            {
//                Console.WriteLine("Failed to Accept client");
            }
        }


        public void CallBroadCast(Packet msg, int withOut = -1)
        {
            //foreach (KeyValuePair<int, UserToken> user in tokenList)
            if (withOut == -1)
            {
                foreach (var user in tokenList)
                    user.Value.Send(msg);
            }
            else
            {
                foreach (var user in tokenList)
                {
                    if (user.Key != withOut)
                        user.Value.Send(msg);
                }
            }
        }

        public void CallSendTo(int tokenID, Packet msg)
        {
            tokenList[tokenID].Send(msg);
        }

    }
}