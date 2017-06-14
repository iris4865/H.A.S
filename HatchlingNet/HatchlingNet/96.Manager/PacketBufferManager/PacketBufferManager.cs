using System;
using System.Collections.Generic;


namespace HatchlingNet
{
    public class PacketBufferManager
    {
        public static PacketBufferManager Instance { get { return CreateInstance.Instance; } }
        class CreateInstance
        {
            static CreateInstance() { }

            internal static readonly PacketBufferManager Instance = new PacketBufferManager();
        }
        private PacketBufferManager() { }

        Stack<Packet> pool;
        int poolCapacity;
        readonly object csBuffer = new object();

        public void Initialize(int capacity)
        {
            poolCapacity = capacity;
            Allocate();
        }

        void Allocate()
        {
            pool = new Stack<Packet>();
            for (int i = 0; i < poolCapacity; ++i)
            {
                pool.Push(new Packet());
            }
        }

        public Packet Pop(Int16 protocolType, Int16 sendType)
        {
            lock (csBuffer)
            {
                if (pool.Count <= 0)
                {
//                    Console.WriteLine("reallocate");
                    Allocate();
                }

                Packet packet = pool.Pop();
                packet.SetProtocol(protocolType);
                packet.SetSendType(sendType);

                return packet;
            }
        }

        public void Push(Packet packet)
        {
            lock (csBuffer)
            {
                pool.Push(packet);
            }
        }
    }
}