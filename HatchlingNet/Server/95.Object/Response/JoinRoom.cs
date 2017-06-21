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

        PROTOCOL responseState;
        UserList list = UserList.Instance;
        List<int> playerPosition;
        List<int> itemList;

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            roomNumber = msg.PopInt32();
        }

        public void Process()
        {
            int userCount = list.roomUserList.Count + 1;
            if (userCount < 4)
            {
                list.roomUserList.Add(Self.UserID);
                responseState = PROTOCOL.JoinRoomRes;
            }
            else if (userCount == 4)
            {
                //int allPlayerCount = 20 + userCount;
                int playercount = 4;
                int nonPlayerCount = 20;
                int itemCount = 3;

                Random rand = new Random();
                //rand.Next(0, );

                playerPosition = new List<int>(playercount);
                itemList = new List<int>(itemCount);

                for (int i = 0; i < playercount; i++)
                {
                    playerPosition[i] = rand.Next(0, playercount);
                    if (playerPosition.Contains(i))
                        i--;
                }

                for (int i = 0; i < itemCount; i++)
                {
                    itemList[i] = rand.Next(0, itemCount);
                    if (itemList.Contains(i))
                        i--;
                }
            }
        }

        public void Send()
        {
            Packet response;
            int userCount = list.roomUserList.Count;

            if (userCount < 4)
            {
                response = PacketBufferManager.Instance.Pop(PROTOCOL.JoinRoomRes);
                response.Push(userCount);
            }
            else if (userCount == 4)
            {
                response = PacketBufferManager.Instance.Pop(PROTOCOL.GameStart);
                for( int i=0; i< playerPosition.Count; i++)
                {
                    response.Push(playerPosition[i]);
                    response.Push(list.roomUserList[i]);
                }

                for (int i = 0; i < itemList.Count; i++)
                {
                    response.Push(itemList[i]);
                }
            }
            else
            {
                response = PacketBufferManager.Instance.Pop(PROTOCOL.JoinRoomRes);
                response.Push(-1);
            }
            Self.SendAll(response);
        }
    }
}