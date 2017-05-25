using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static System.Threading.Tasks.Parallel;

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
            acceptArgs = new SocketAsyncEventArgs();//SocketAsyncEventArgs 라고하는 비동기 객체 생성 

            tokenNumberingPool = new NumberingPool(10000);

            for (int number = 0; number < tokenNumberingPool.capacity; number++)
                tokenNumberingPool.Push(number);
        }

        public void Start(string host, int port, int backlog)//backlog : 대기큐의 크기
        {
            //0.0.0.0 ==> any
            IPAddress address;
            address = (host == "0.0.0.0" ? IPAddress.Any : IPAddress.Parse(host));

            IPEndPoint endpoint = new IPEndPoint(address, port);

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endpoint); //호스트의 정보 등록
            listenSocket.Listen(backlog); //받아들일 클라이언트 수 결정

            acceptArgs.Completed += AcceptComplete;

            Thread listenThread = new Thread(DoListen);
            listenThread.Start();
        }

        public void DoListen()
        {
            flowController = new AutoResetEvent(false);

            while (true)
            {
                acceptArgs.AcceptSocket = null;

                //반환값이 false이면 동기적 처리. 동기적 처리이면 콜백함수 호출 안됨
                //listenSocket에만 콜백 등록해놨지 다른 소켓에는 콜백 등록 안했으니
                //연결됨으로써 새롭게 만들어지는 소켓에는 콜백이 없는거임 착각 ㄴㄴ

                //Date. 5.25 true로 준 이유가?
                bool pending = true;
                pending = listenSocket.AcceptAsync(acceptArgs);
                if (!pending)
                    AcceptComplete(null, acceptArgs);

                flowController.WaitOne();
            }
        }

        public void AcceptComplete(Object sender, SocketAsyncEventArgs e)
        {
            //            Console.WriteLine("액셉컴플리트");

            if (CanAcceptSuccess(e.SocketError))
            {
                Interlocked.Increment(ref connectionCount);

                SocketAsyncEventArgs receiveArgs = SocketAsyncEventArgsPool.receiveInstance.Pop();
                SocketAsyncEventArgs sendArgs = SocketAsyncEventArgsPool.sendInstance.Pop();

                Socket clientSocket = e.AcceptSocket;
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
            Trace.WriteLine("Failed to Accept client");
        }


        public void CallBroadCast(Packet msg, int withOut = -1)
        {
            ForEach(tokenList, (user) =>
                {
                    if (user.Key != withOut)
                        user.Value.Send(msg);
                }
                );
            //foreach (KeyValuePair<int, UserToken> user in tokenList)
            /*
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
            */
        }

        public void CallSendTo(int tokenID, Packet msg)
        {
            tokenList[tokenID].Send(msg);
        }

        bool CanAcceptSuccess(SocketError socketError) => socketError == SocketError.Success;
    }
}