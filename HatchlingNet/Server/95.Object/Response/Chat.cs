using HatchlingNet;
using Header;

namespace Server
{
    public class Chat : IResponse
    {
        SEND_TYPE sendType;
        string text = "";

        public IGameUser User { get; set; }

        public void Initialize(IGameUser user, Packet msg)
        {
            User = user;
            sendType = (SEND_TYPE)msg.PopSendType();
            text = msg.PopString();
        }

        public void Process()
        {
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop((short)PROTOCOL.ChatAck, (short)sendType);
            response.Push(text);
            User.Send(response);
        }
    }
}
