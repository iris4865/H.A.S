using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HatchlingNet
{
    public class NumberingPool
    {
        Stack<int> pool;

        public NumberingPool(int capacity)
        {
            pool = new Stack<int>(capacity);

            for (int i = 0; i < capacity; ++i)
            {
                pool.Push(i);
            }
        }

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
