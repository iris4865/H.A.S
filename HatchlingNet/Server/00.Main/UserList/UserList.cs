using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server
{
    public sealed class UserList
    {
        private static readonly Lazy<UserList> instance = new Lazy<UserList>(() => new UserList());
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
                return instance.Value;
            }
        }

        public void SessionCreate(Socket socket, UserToken token)
        {
            GameUser user = new GameUser(token);

            lock (syncObj)
            {
                userList.Add(user);
            }
        }

        public void RemoveUser(GameUser user)
        {
            lock (syncObj)
            {
                userList.Remove(user);
            }
        }
    }
}
