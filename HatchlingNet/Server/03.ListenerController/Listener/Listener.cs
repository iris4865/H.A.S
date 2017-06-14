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
        Socket listenSocket;
        public Action<UserToken> BeginReceive;
        readonly SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs(); //비동기 Accept를 위한 객체;

        Dictionary<int, UserToken> tokenList = new Dictionary<int, UserToken>();

        int connectionCount;

        public void Ready(string host, int port, int backlog)//backlog : 대기큐의 크기
        {
            IPEndPoint endpoint = new IPEndPoint(GetServerIP(host), port);

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endpoint); //호스트의 정보 등록
            listenSocket.Listen(backlog); //받아들일 클라이언트 수 결정

            acceptArgs.Completed += AcceptComplete;
        }

        public void AcceptStart()
        {
            acceptArgs.AcceptSocket = null;

            bool isAsync = listenSocket.AcceptAsync(acceptArgs);
            if (!isAsync)
                AcceptComplete(null, acceptArgs);
        }


        public void AcceptComplete(Object sender, SocketAsyncEventArgs args)
        {
            Trace.WriteLine("Client Accept");

            if (CanAcceptSuccess(args.SocketError))
            {
                Interlocked.Increment(ref connectionCount);

                UserToken userToken = UserTokenPool.Instance.Pop();

                userToken.socket = args.AcceptSocket;
                userToken.Broadcast = CallBroadCast;
                userToken.SendTo = CallSendTo;

                lock (userToken)
                {
                    tokenList.Add(userToken.TokenID, userToken);
                }

                UserList.Instance.AddUser(userToken);

                BeginReceive(userToken);
            }
            else
                Trace.WriteLine("Failed to Accept client");

            AcceptStart();
        }


        public void CallBroadCast(Packet msg, int withOut = -1)
        {
            ForEach(
                tokenList, (user) =>
                {
                    if (user.Key != withOut)
                        user.Value.Send(msg);
                }
            );

            /*
            foreach(var user in tokenList)
            {
                if (user.Key != withOut)
                    user.Value.Send(msg);
            }
            */
        }

        public void CallSendTo(Packet msg, int tokenID)
        {
            tokenList[tokenID].Send(msg);
        }

        public void Disconnect(UserToken token)
        {
            Interlocked.Decrement(ref connectionCount);
            token.Clear();
            //tokenID 제거추가할것
            lock (token)
            {
                tokenList.Remove(token.TokenID);
            }
            UserList.Instance.RemoveUser(token.Peer as GameUser);
            UserTokenPool.Instance.Push(token);
        }

        /// <summary>
        /// IPAddress객체로 변환
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        IPAddress GetServerIP(string host)
        {
            return (host == "0.0.0.0" ? IPAddress.Any : IPAddress.Parse(host));
        }

        /// <summary>
        /// 접속성공 확인
        /// </summary>
        /// <param name="socketError"></param>
        /// <returns></returns>
        bool CanAcceptSuccess(SocketError socketError)
        {
            return socketError == SocketError.Success;
        }
    }
}