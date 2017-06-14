using System;
using System.Collections.Generic;

namespace HatchlingNet
{
    public class UserTokenPool
    {
        public static UserTokenPool Instance { get { return CreateInstance.Instance; } }
        Stack<UserToken> pool;

        class CreateInstance
        {
            static CreateInstance() { }

            internal static readonly UserTokenPool Instance = new UserTokenPool();
        }
        private UserTokenPool() { }

        public int Count
        {
            get
            {
                return pool.Count;
            }
            set
            {
                if (pool == null)
                    pool = new Stack<UserToken>(value);
            }
        }

        public void Push(UserToken item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "is Null");

            lock (pool)
            {
                pool.Push(item);
            }
        }

        public UserToken Pop()
        {
            lock (pool)
            {
                return pool.Pop();
            }
        }
    }
}
