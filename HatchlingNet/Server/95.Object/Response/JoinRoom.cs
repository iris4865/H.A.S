using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;

namespace Server
{
    class JoinRoom : IResponse
    {
        public IGameUser Self { get; set; }
        int roomNumber;

        UserList list = UserList.Instance;
        List<int> playerPositionList;
        List<int> itemList;

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            roomNumber = msg.PopInt32();
        }

        public void Process()
        {
            list.roomUserList.Add(Self.UserID);
            int userCount = list.roomUserList.Count;

            if (userCount == 4)
            {
                int playercount = 4;
                int itemCount = 3;

                playerPositionList = new List<int>(playercount);
                itemList = new List<int>(itemCount);

                FillwithRandom(playerPositionList);
                FillwithRandom(itemList);
            }
        }

        void FillwithRandom(List<int> objectList)
        {
            for (int i = 0; i < objectList.Capacity; i++)
            {
                int value = new Random().Next(0, objectList.Capacity);

                if (objectList.Contains(value))
                    i--;
                else
                    objectList.Add(value);
            }
        }

        public void Send()
        {
            int userCount = list.roomUserList.Count;

            SendUserCount();
            if (userCount == 4)
            {
                Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.GameStart);

                for (int i = 0; i < playerPositionList.Count; i++)
                {
                    response.Push(playerPositionList[i]);
                    response.Push(list.roomUserList[i]);
                }

                for (int i = 0; i < itemList.Count; i++)
                    response.Push(itemList[i]);

                Self.SendAll(response);
            }
        }

        void SendUserCount()
        {
            Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.JoinRoomRes);
            int count = list.roomUserList.Count;

            if (count > 4)
            {
                response.Push(-1);
                Self.SendTo(Self.UserID, response);
            }
            else
            {
                response.Push(count);
                Self.SendAll(response);
            }
        }
    }
}