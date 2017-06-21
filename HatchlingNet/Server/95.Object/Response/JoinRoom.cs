using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class JoinRoom : IResponse
    {
        public IGameUser Self { get; set; }
        int roomNumber;

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            roomNumber = msg.PopInt32();

        }

        public void Process()
        {
        }

        public void Send()
        {
            Packet response;
            int userCount = UserList.Instance.roomUserCount;

            if (userCount < 4)
            {
                userCount = ++UserList.Instance.roomUserCount;
                response = PacketBufferManager.Instance.Pop(PROTOCOL.JoinRoomRes);
                response.Push(userCount);
            }
            else
            {
                response = PacketBufferManager.Instance.Pop(PROTOCOL.GameStart);
            }

            Self.SendAll(response);
        }

    }
}
