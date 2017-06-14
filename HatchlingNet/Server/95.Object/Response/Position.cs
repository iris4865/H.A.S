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

        public void Initialize(Packet msg)
        {
            sendType = (SEND_TYPE)msg.PopSendType();
            remoteId = msg.PopInt32();
            vec = msg.PopVector();
            rotateY = msg.PopFloat();
            speed = msg.PopFloat();
            animationType = msg.PopInt32();
        }

        public void Process(GameUser user)
        {
        }

        public void Send(Action<Packet> send)
        {
            Packet response = PacketBufferManager.Instance.Pop((short)PROTOCOL.PositionAck, (short)SEND_TYPE.BroadcastWithoutMe);
            response.Push(remoteId);
            response.Push(vec);
            response.Push(rotateY);
            response.Push(speed);
            response.Push(animationType);
            send(response);
        }
    }
}
