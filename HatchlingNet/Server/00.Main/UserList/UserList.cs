using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server
{
    public sealed class UserList
    {
        private static readonly Lazy<UserList> instance = new Lazy<UserList>(() => new UserList());
        public static UserList Instance => instance.Value;

        private static object syncObj = new object();
        List<GameUser> userList;

        private UserList()
        {
            userList = new List<GameUser>();
        }

        public void SessionCreate(UserToken token)
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
