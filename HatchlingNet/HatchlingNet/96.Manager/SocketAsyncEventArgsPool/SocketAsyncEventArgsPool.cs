using System;
using System.Collections.Generic;
using System.Net.Sockets;


//https://msdn.microsoft.com/ko-kr/library/system.net.sockets.socketasynceventargs.socketasynceventargs(v=vs.110).aspx

namespace HatchlingNet
{
    public class SocketAsyncEventArgsPool
    {
        public static SocketAsyncEventArgsPool receiveInstance { get { return CreateInstance.receiveInstance; } }
        public static SocketAsyncEventArgsPool sendInstance { get { return CreateInstance.sendInstance; } }

        Stack<SocketAsyncEventArgs> pool;

        private class CreateInstance
        {
            static CreateInstance() { }

            internal static readonly SocketAsyncEventArgsPool sendInstance = new SocketAsyncEventArgsPool();
            internal static readonly SocketAsyncEventArgsPool receiveInstance = new SocketAsyncEventArgsPool();
        }
        private SocketAsyncEventArgsPool() { }

        public int Count
        {
            get
            {
                return pool.Count;
            }
            set
            {
                if (pool == null)
                    pool = new Stack<SocketAsyncEventArgs>(value);
            }
        }

        // Initializes the object pool to the specified size
        //
        // The "capacity" parameter is the maximum number of 
        // SocketAsyncEventArgs objects the pool can hold

        //public SocketAsyncEventArgsPool(int capacity)
        //{
        //    pool = new Stack<SocketAsyncEventArgs>(capacity);
        //}

        // Add a SocketAsyncEventArg instance to the pool
        //
        //The "item" parameter is the SocketAsyncEventArgs instance 
        // to add to the pool
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");

            lock (pool)
            {
                pool.Push(item);
            }
        }

        // Removes a SocketAsyncEventArgs instance from the pool
        // and returns the object removed from the pool
        public SocketAsyncEventArgs Pop()
        {
            lock (pool)
            {
                return pool.Pop();
            }
        }
    }
}
