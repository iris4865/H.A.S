using HatchlingNet;
using Header;

namespace Server
{
    public class Position : IResponse
    {
        UnityEngine.KeyCode keyCode;
        MyVector3 vec;

        public IGameUser User { get; set; }

        public void Initialize(IGameUser user, Packet msg)
        {
            User = user;
            keyCode = (UnityEngine.KeyCode)msg.PopInt16();
            vec = msg.PopVector();
        }

        public void Process()
        {
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.PositionAck);
            response.Push((short)keyCode);
            response.Push(vec);
            User.SendAllWithoutMe(response);
        }
    }
}
