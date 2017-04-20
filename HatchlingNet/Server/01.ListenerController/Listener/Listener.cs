﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using HatchlingNet;

namespace Server
{
    public class Listener
    {
        Dictionary<int, UserToken> tokenList;
        SocketAsyncEventArgsPool receiveEventArgsPool;//메시지 수신객체, 풀링해서 사용예정
        SocketAsyncEventArgsPool sendEventArgsPool;//메시지 전송객체, 풀링해서 사용예정

        SocketAsyncEventArgs acceptArgs;//비동기 Accept를 위한 객체;
        Socket listenSocket;           //클라이언트의 접속을 처리할 소켓

        AutoResetEvent flowController;

        int maxConnection;
        int connectionCount;
        int assignIDToUser;
        int bufferSize;

        public delegate void ReceiveBeginHandler(Socket clientSocket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs);
        public ReceiveBeginHandler receiveBeginTrigger;


        public Listener(ListenerController networkService, SocketAsyncEventArgsPool receivePool, SocketAsyncEventArgsPool sendPool)
        {
            assignIDToUser = 0;
            this.receiveEventArgsPool = receivePool;
            this.sendEventArgsPool = sendPool;

            tokenList = new Dictionary<int, UserToken>();
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

                Interlocked.Increment(ref this.connectionCount);
                SocketAsyncEventArgs receiveArgs = receiveEventArgsPool.Pop();
                SocketAsyncEventArgs sendArgs = sendEventArgsPool.Pop();

                UserToken userToken = receiveArgs.UserToken as UserToken;

                userToken.socket = clientSocket;
                userToken.sendEventArgs = sendArgs;
                userToken.receiveEventArgs = receiveArgs;
                userToken.tokenID = assignIDToUser++;
                userToken.callbackBroadcast = CallBroadCast;
                userToken.callbackSendTo = CallSendTo;

                lock (userToken)
                {
                    tokenList.Add(userToken.tokenID, userToken);
                }

                UserList.GetInstance.CallSessionCreate(clientSocket, userToken);

                receiveBeginTrigger(clientSocket, receiveArgs, sendArgs);

                flowController.Set();

                return;
            }
            else
            {
                Console.WriteLine("Failed to Accept client");
            }
        }
        

        public void CallBroadCast(Packet msg, int withOut = -1)
        {
            if (withOut != -1)
            {
                foreach (KeyValuePair<int, UserToken> user in tokenList)
                {
                    user.Value.Send(msg);
                }
            }
            else
            {
                foreach (KeyValuePair<int, UserToken> user in tokenList )
                {
                    if(withOut != user.Key)
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