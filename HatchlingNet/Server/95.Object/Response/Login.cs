using HatchlingNet;
using Header;
using System;

namespace Server
{
    public class Login : IResponse
    {
        SEND_TYPE sendType;
        public string Id { get; private set; }
        public string Password { get; private set; }

        bool isSucess;

        public void Initialize(Packet msg)
        {
            sendType = (SEND_TYPE)msg.PopSendType();
            Id = msg.PopString();
            Password = msg.PopString();
        }

        public void Process(GameUser user)
        {
            //isSucess = command.CheckLogin(id, password);
            isSucess = true;

            if(isSucess)
                user.UserID = Id;
        }

        public void Send(Action<Packet> send)
        {
            Packet response;
            if (isSucess)
            {
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.LoginAck, (short)SEND_TYPE.Single);
                response.Push(Id);
            }
            else
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.LoginRej, (short)SEND_TYPE.Single);

            send(response);
        }
    }
}
