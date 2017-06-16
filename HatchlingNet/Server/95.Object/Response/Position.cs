using HatchlingNet;
using Header;
using UnityEngine;

namespace Server
{
    public class Position : IResponse
    {
        /*
         * 위치
         * 속도
         * 마우스 각도
         * (갯수만큼 하는 방법)
         */
        int remoteId;
        MyVector3 position;
        short animationType;
        float mouseAxis;

        public IGameUser Self { get; set; }

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            remoteId = msg.PopInt32();
            position = msg.PopVector();
            animationType = msg.PopInt16();
            mouseAxis = msg.PopFloat();
        }

        public void Process()
        {
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.PositionAck);
            response.Push(remoteId);
            response.Push(position);
            response.Push(animationType);
            response.Push(mouseAxis);
            Self.SendAllWithoutMe(response);
        }
    }
}
