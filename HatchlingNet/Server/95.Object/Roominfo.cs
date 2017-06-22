using HatchlingNet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    public class RoomInfo
    {
        List<GameUser> userList = new List<GameUser>();

        public bool IsFully => userList.Count == 4;
        public int Count => userList.Count;
        public GameUser[] Array => userList.ToArray();

        public bool EnterRoom(GameUser user)
        {
            if (CheckInRoom(user))
            {
                userList.Add(user);
                return true;
            }
            return false;
        }

        bool CheckInRoom(GameUser user)
        {
            if (user == null)
                return false;
            if (userList.Count == 4)
                return false;
            if (userList.Contains(user))
                return false;
            return true;
        }

        public void ExitRoom(GameUser user)
        {
            userList.Remove(user);
        }

        public void SendToSameGroup(int job, Packet msg)
        {
            Parallel.ForEach(
                userList, (user) =>
                {
                    if (user.Job == job)
                        user.Send(msg);
                }
            );
        }

        public void SendToAll(Packet msg)
        {
            Parallel.ForEach(
                userList, (user) =>
                {
                    user.Send(msg);
                }
            );
        }
    }
}
