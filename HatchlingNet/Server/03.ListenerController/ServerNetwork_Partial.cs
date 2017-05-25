using HatchlingNet;
using System;
using System.Net.Sockets;

namespace Server
{
    public partial class ServerNetwork
    {
        //receive pool
        void PushReceiveEventArgsPool(UserToken token)
        {
            SocketAsyncEventArgs args = PreAllocateSocketAsyncEventArgs(token, network.CallReceiveComplete);
            receiveEventArgsPool.Push(args);
        }

        //send pool
        void PushSendEventArgsPool(UserToken token)
        {
            SocketAsyncEventArgs args = PreAllocateSocketAsyncEventArgs(token, network.CallSendComplete);
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
