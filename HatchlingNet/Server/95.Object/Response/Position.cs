using HatchlingNet;
using Header;
using System.Diagnostics;

namespace Server
{
    public class Position : IResponse
    {
        int remoteId;
        MyVector3 vec;
        MyVector3 rotation;
        float speed;
        int animationType;

        public IGameUser User { get; set; }

        public void Initialize(IGameUser user, Packet msg)
        {
            User = user;
            remoteId = msg.PopInt32();
            vec = msg.PopVector();
            rotation = msg.PopVector();
            speed = msg.PopFloat();
            animationType = msg.PopInt32();
        }

        public void Process()
        {
            //Trace.WriteLine("rotateY: "+rotateY);
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop((short)PROTOCOL.PositionAck);
            response.Push(remoteId);
            response.Push(vec);
            response.Push(rotation);
            response.Push(speed);
            response.Push(animationType);
            User.SendAllWithoutMe(response);
        }
    }
}
