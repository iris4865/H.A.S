using HatchlingNet;
using Header;
using System;

namespace Server
{
    public class Position : IResponse
    {
        SEND_TYPE sendType;
        int remoteId;
        MyVector3 vec;
        float rotateY, speed;
        int animationType;

        public IGameUser User { get; set; }

        public void Initialize(IGameUser user, Packet msg)
        {
            User = user;
            sendType = (SEND_TYPE)msg.PopSendType();
            remoteId = msg.PopInt32();
            vec = msg.PopVector();
            rotateY = msg.PopFloat();
            speed = msg.PopFloat();
            animationType = msg.PopInt32();
        }

        public void Process()
        {
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop((short)PROTOCOL.PositionAck, (short)SEND_TYPE.BroadcastWithoutMe);
            response.Push(remoteId);
            response.Push(vec);
            response.Push(rotateY);
            response.Push(speed);
            response.Push(animationType);
            User.Send(response);
        }
    }
}
