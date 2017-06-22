using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public sealed class UserList
    {
        static readonly Lazy<UserList> instance = new Lazy<UserList>(() => new UserList());
        public static UserList Instance => instance.Value;

        readonly object syncObj = new object();
        readonly Dictionary<string, GameUser> userList;

        UserList()
        {
            userList = new Dictionary<string, GameUser>();
        }

        public void AddUser(GameUser user)
        {
            lock (syncObj)
            {
                userList[user.UserID] = user;
            }
        }

        public void RemoveUser(GameUser user)
        {
            lock (syncObj)
            {
                try
                {
                    userList.Remove(user.UserID);
                }
                catch { }
            }
        }

        public bool IsLoginUser(string userId)
        {
            return userList.ContainsKey(userId);
        }

        public GameUser GetUser(string userId)
        {
            return userList[userId];
        }

        public GameUser[] GetAllUser()
        {
            return userList.Values.ToArray();
        }
    }
}
