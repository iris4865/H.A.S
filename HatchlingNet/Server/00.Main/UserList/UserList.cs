using HatchlingNet;
using System.Collections.Generic;
using System.Net.Sockets;
using System;

namespace Server
{
    public sealed class UserList
    {
        private static volatile UserList instance;
        private static object syncObj = new object();

        static List<GameUser> userList;

        private UserList()
        {
            userList = new List<GameUser>();
        }

        public static UserList GetInstance//...함수같지만 변수인...
        {                               //이것이 c#의 겟터인가...
            get
            {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                        {
                            instance = new UserList();
                        }
                    }
                }

                return instance;
            }

       
        }

        public void CallSessionCreate(Socket socket, UserToken token)
        {
            GameUser user = new GameUser(token);

            lock (syncObj)
            {
                userList.Add(user);
            }
        }

        public static void RemoveUser(GameUser user)
        {
            lock (syncObj)
            {
                userList.Remove(user);
            }
        }
    }
}
