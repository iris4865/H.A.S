using HatchlingNet;
using Header;
using System.Collections.Generic;

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
        int count;
        Dictionary<short, short> inputEvent = new Dictionary<short, short>();
        float mouseAxis;

        public IGameUser Self { get; set; }

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            remoteId = msg.PopInt32();

            count = msg.PopInt32();
            for (int i = 0; i <= count; i++)
                inputEvent[msg.PopInt16()] = msg.PopInt16();
            mouseAxis = msg.PopFloat();
        }

        public void Process()
        {
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.PositionAck);

            response.Push(remoteId);
            response.Push(count);
            foreach (var playerEvent in inputEvent)
            {
                response.Push(playerEvent.Key);
                response.Push(playerEvent.Value);
            }
            response.Push(mouseAxis);

            Self.SendAllWithoutMe(response);
        }
    }
}
