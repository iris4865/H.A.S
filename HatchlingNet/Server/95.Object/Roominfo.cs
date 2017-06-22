using HatchlingNet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    //후에 대기방, 게임방 분리가능
    public class RoomInfo
    {
        List<GameUser> userList = new List<GameUser>();

        public bool IsFully => userList.Count == 2;
        public int Count => userList.Count;
        public GameUser[] Array => userList.ToArray();
        NPCManager npcManager;

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
            if (IsFully || userList.Contains(user))
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

        public void GameStart(int npcNumber)
        {
            npcManager = new NPCManager(npcNumber)
            {
                Send = SendToAll
            };
            npcManager.Start();
        }

        public void GameEnd() => npcManager.Stop();
    }
}
