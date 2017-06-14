using HatchlingNet;
using Header;
using System;

namespace Server
{
    public class Chat : IResponse
    {
        SEND_TYPE sendType;
        string text = "";

        public void Initialize(Packet msg)
        {
            sendType = (SEND_TYPE)msg.PopSendType();
            text = msg.PopString();
        }

        public void Process(GameUser user)
        {
        }

        public void Send(Action<Packet> send)
        {
            Packet response = PacketBufferManager.Instance.Pop((short)PROTOCOL.ChatAck, (short)sendType);
            response.Push(text);
            send(response);
        }
    }
}
