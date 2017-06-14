using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HatchlingNet
{
    public class NumberingPool
    {
        readonly Stack<int> pool;
        public int Capacity { get; private set; }

        public NumberingPool(int capacity)
        {
            pool = new Stack<int>(capacity);
            Capacity = capacity;

            //for (int i = 0; i < capacity; ++i)
            //{
            //    pool.Push(i);
            //}
        }

        //public int GetCapacity()
        //{

        //}

        public int Pop()
        {
            return pool.Pop();
        }

        public void Push(int value)
        {
            pool.Push(value);
        }
    }
}
