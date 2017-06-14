using HatchlingNet;
using System;
using System.Collections.Generic;

namespace Server
{
    public sealed class UserList
    {
        static readonly Lazy<UserList> instance = new Lazy<UserList>(() => new UserList());
        public static UserList Instance => instance.Value;

        readonly object syncObj = new object();
        readonly List<GameUser> userList;

        UserList()
        {
            userList = new List<GameUser>();
        }

        public void AddUser(UserToken token)
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
