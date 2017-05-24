using HatchlingNet;
using System;
using System.Net.Sockets;

namespace Server
{
    public partial class ServerNetwork : NetworkService
    {
        //receive pool
        void PushReceiveEventArgsPool(UserToken token)
        {
            SocketAsyncEventArgs args = PreAllocateSocketAsyncEventArgs(token, CallReceiveComplete);
            receiveEventArgsPool.Push(args);
        }

        //send pool
        void PushSendEventArgsPool(UserToken token)
        {
            SocketAsyncEventArgs args = PreAllocateSocketAsyncEventArgs(token, CallSendComplete);
            sendEventArgsPool.Push(args);
        }

        SocketAsyncEventArgs PreAllocateSocketAsyncEventArgs(UserToken token, EventHandler<SocketAsyncEventArgs> handler)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += handler;
            args.UserToken = token;

            buffer_manager.SetBuffer(args);

            return args;
        }
    }
}
