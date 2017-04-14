using System;
using System.Collections.Generic;


namespace HatchlingNet
{
    public class PacketBufferManager
    {
        static object csBuffer = new object();

        static Stack<Packet> pool;
        static int poolCapacity;


        public static void Initialize(int capacity)
        {
            poolCapacity = capacity;
            Allocate();
        }

        static void Allocate()
        {
            pool = new Stack<Packet>();
            for (int i = 0; i < poolCapacity; ++i)
            {
                pool.Push(new Packet());
            }
        }

        public static Packet Pop(Int16 protocolType, Int16 sendType)
        {
            lock (csBuffer)
            {
                if (pool.Count <= 0)
                {
                    Console.WriteLine("reallocate");
                    Allocate();
                }

                Packet packet = pool.Pop();
                packet.SetProtocol(protocolType);
                packet.SetSendType(sendType);

                return packet;
            }
        }

        public static void Push(Packet packet)
        {
            lock (csBuffer)
            {
                pool.Push(packet);
            }
        }
    }
}