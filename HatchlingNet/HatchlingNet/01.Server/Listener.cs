using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HatchlingNet
{
    public class Listener
    {
        NetworkService networkService;
        List<UserToken> userList;

        SocketAsyncEventArgs acceptArgs;//비동기 Accept를 위한 객체;
        Socket listenSocket;           //클라이언트의 접속을 처리할 소켓

        AutoResetEvent flowController;

        //public delegate void NewclientHandler(Socket clientSocket, Object token);
        //public NewclientHandler CallbackNewclient;

        public delegate void ReceiveBeginHandler(Socket clientSocket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs);
        public ReceiveBeginHandler receiveBeginTrigger;

        int maxConnection;
        int connectionCount;
        int bufferSize;

        public Listener(NetworkService networkService)
        {
            this.networkService = networkService;
            userList = new List<UserToken>();
            this.acceptArgs = new SocketAsyncEventArgs();//SocketAsyncEventArgs 라고하는 비동기 객체 생성 
        }


        public void Start(string host, int port, int backlog)//backlog : 대기큐의 크기
        {
            this.listenSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            IPAddress address;
            if (host == "0.0.0.0")
            {
                address = IPAddress.Any;
            }
            else
            {
                address = IPAddress.Parse(host);
            }

            IPEndPoint endpoint = new IPEndPoint(address, port);//단순히 ip와 연결할 포트를 합쳐놓은걸 EndPoint라 한다


            this.listenSocket.Bind(endpoint); //호스트의 정보 등록
            this.listenSocket.Listen(backlog); //받아들일 클라이언트 수 결정



           
            this.acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptComplete);
            //연결이 되었을경우 호출할 콜백함수의 핸들러를 Completed에 저장함...다만 연결이 되었는지에 대한 검사는 별도로 해야됨. 그게 바로 아래 코드


            Thread listen_thread = new Thread(do_listen);
            listen_thread.Start();

            
        }

        public void do_listen()
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
            Console.WriteLine("액셉컴플리트");

            if (e.SocketError == SocketError.Success)
            {
                Socket clientSocket = e.AcceptSocket;


                //요 아래 이벤트풀 호출하는 부분에서 이벤트풀은 네트워크서비스 클래스에 있는데
                //여기서 이런식으로 거슬러 올라가서 쓰는게 좋은걸까?
                //네트워크 클래스에서 콜백함수를 정의해서 여기서 콜백함수를 호출하는게 맞는걸까?
                //콜백함수 쓰면 구조가 너무 복잡해지는 느낌...
                Interlocked.Increment(ref this.connectionCount);
                SocketAsyncEventArgs receiveArgs = networkService.receiveEventArgsPool.Pop();
                SocketAsyncEventArgs sendArgs = networkService.sendEventArgsPool.Pop();

                UserToken userToken = receiveArgs.UserToken as UserToken;
                //if (this.callbackSessionCreate != null)
                //{
                //    callbackSessionCreate(userToken);
                //}


                userToken.socket = clientSocket;
                userToken.sendEventArgs = sendArgs;
                userToken.receiveEventArgs = receiveArgs;


                lock (userToken)
                {
                    userList.Add(userToken);
                }

                //if (this.CallbackNewclient != null)//각 리스너는 userList를 들고있고 dll을 포함한 프로젝트의 main에서
                //                            //모든 리스너에 접속한 유저들의 리스트를 가지는데
                //                            //해당 유저들을 등록할때 CallbackNewClient에서 수행
                //                            //프로그램의 main에서 등록해서 사용
                //{
                //    this.CallbackNewclient(clientSocket, e.UserToken);

                //}

                if (this.networkService.callbackSessionCreate != null)
                {
                    this.networkService.callbackSessionCreate(clientSocket, userToken);
                }


                receiveBeginTrigger(clientSocket, receiveArgs, sendArgs);


                //bool pending = clientSocket.ReceiveAsync(receiveArgs);//receiveArgs에는 complted콜백 등록 안해줬음...해줘야됨
                //if (!pending)
                //{
                //    ProcessReceive(userToken.receiveEventArg);
                //}

                //BeginReceive(clientSocket, receiveArgs, sendArgs);

                return;
            }
            else
            {
                Console.WriteLine("Failed to Accept client");
            }


            flowController.Set();
        }


        //public void ProcessReceive(SocketAsyncEventArgs receiveArgs)
        //{
        //    UserToken token = receiveArgs.UserToken as UserToken;

        //    if (receiveArgs.BytesTransferred > 0 && receiveArgs.SocketError == SocketError.Success)
        //    {
        //        //e.Buffer : 클라로부터 수신된 데이터, e.offset : 버퍼의 포지션, e.ByesTransferred : 이번에 수신된 바이트의 수
        //        token.OnReceive(receiveArgs.Buffer, receiveArgs.Offset, receiveArgs.BytesTransferred);

        //        bool pending = token.socket.ReceiveAsync(receiveArgs);
        //        if (!pending)                               
        //        {
        //            ProcessReceive(receiveArgs);
        //        }//비동기로 한번이라도 처리되는 순간 함수 나가게 되니 스택에 ProcessReceive가 계속 쌓이는건 아닌지에 대한 걱정은 안해도 된다.
        //    }
        //    else
        //    {
        //        Console.WriteLine(string.Format("error {0}, transferred {1}", receiveArgs.SocketError, receiveArgs.BytesTransferred));
        //        CloseClientSocket(token);
        //    }
        //}

        public void AddUser(UserToken user)
        {
            lock (userList)
            {
                userList.Add(user);
            }
        }

        public void RemoveUser(UserToken user)
        {
            lock (userList)
            {
                userList.Remove(user);
            }
        }

        //public void CloseClientSocket(UserToken user)
        //{

        //}
    }
}
