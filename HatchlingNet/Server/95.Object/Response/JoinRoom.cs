using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;

namespace Server
{
    class JoinRoom : IResponse
    {
        public IGameUser Self { get; set; }

        RoomList roomList = RoomList.Instance;
        RoomInfo room;
        List<int> playerPositionList;
        List<int> itemList;

        bool isEnter;

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            room = roomList[msg.PopInt32()];
        }

        public void Process()
        {
            isEnter = room.EnterRoom(Self as GameUser);

            if (room.IsFully)
            {
                int itemCount = 3;

                playerPositionList = new List<int>(room.Count);
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
            SendUserCount();
            if (room.IsFully)
            {
                Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.GameStart);

                GameUser[] user = room.Array;
                for (int i = 0; i < playerPositionList.Count; i++)
                {
                    response.Push(playerPositionList[i]);
                    response.Push(user[i].UserID);
                }

                for (int i = 0; i < itemList.Count; i++)
                    response.Push(itemList[i]);

                room.SendToAll(response);
                room.GameStart(20);
            }
        }

        void SendUserCount()
        {
            Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.JoinRoomRes);
            if (isEnter)
            {
                response.Push(room.Count);
                room.SendToAll(response);
            }
            else
            {
                response.Push(-1);
                Self.SendTo(Self, response);
            }
        }
    }
}