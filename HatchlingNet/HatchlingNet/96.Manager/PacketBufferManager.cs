using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HatchlingNet
{
    public class PacketBufferManager
    {
        static object csBuffer = new object();
        static Stack<Packet> pool;
        static int poolCapacity;

        public static void Initialize(int capacity)
        {
            pool = new Stack<Packet>();
            poolCapacity = capacity;
            Allocate();
        }

        static void Allocate()
        {
            for (int i = 0; i < poolCapacity; ++i)
            {
                pool.Push(new Packet());
            }
        }

        public static Packet Pop(Int16 protocol_id)
        {
            lock (csBuffer)
            {
                if (pool.Count <= 0)
                {
                    Console.WriteLine("reallocate");
                    Allocate();
                }

                Packet packet = pool.Pop();
                packet.SetProtocol(protocol_id);

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
