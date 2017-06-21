using HatchlingNet;
using Header;
using System;

namespace Server
{
    class JoinRoom : IResponse
    {
        public IGameUser Self { get; set; }
        int roomNumber;

        PROTOCOL responseState;
        UserList list = UserList.Instance;
        int[] playerList;
        int[] itemList;
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
                int itemPositionCount;

                Random rand = new Random();
                //rand.Next(0, );

                playerList = new int[playercount];

                /*
                for(int i=0; i<playercount; i++)
                {
                    playerList[i] = rand.Next(0, playercount);
                    for(int j=0;)
                }
                */
                playerList[0] = 1;
                playerList[1] = 1;
                playerList[2] = 2;
                playerList[3] = 2;

                itemList[0] = 0;
                itemList[1] = 1;
                itemList[2] = 2;

                //경찰 = 1, 도둑 = 2
                //유저 id, num
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
                for( int i=0; i< playerList.Length; i++)
                {
                    response.Push(playerList[i]);
                    response.Push(i);
                    response.Push(list.roomUserList[i]);
                }

                for (int i = 0; i < itemList.Length; i++)
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
