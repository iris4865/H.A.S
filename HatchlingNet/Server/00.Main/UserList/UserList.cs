using HatchlingNet;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server
{
    public class UserList
    {
        private static UserList instance = new UserList();
        static List<GameUser> userList;

        private UserList()
        {
            userList = new List<GameUser>();
        }

        public static UserList GetInstance()
        {
            return instance;
        }

        public static void CallSessionCreate(Socket socket, UserToken token)
        {
            GameUser user = new GameUser(token);
            //user.mysql = mysql;

            lock (userList)
            {
                userList.Add(user);
            }
        }

        public static void RemoveUser(GameUser user)
        {
            lock (userList)
            {
                userList.Remove(user);
            }
        }
    }
}
