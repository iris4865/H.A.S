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
        public IGameUser User { get; set; }

        bool isSucess;

        public void Initialize(IGameUser user, Packet msg)
        {
            User = user;
            sendType = (SEND_TYPE)msg.PopSendType();
            Id = msg.PopString();
            Password = msg.PopString();
        }

        public void Process()
        {
            //isSucess = command.CheckLogin(id, password);
            isSucess = true;

            if(isSucess)
                User.UserID = Id;
        }

        public void Send()
        {
            Packet response;
            if (isSucess)
            {
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.LoginAck, (short)SEND_TYPE.Single);
                response.Push(Id);
            }
            else
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.LoginRej, (short)SEND_TYPE.Single);

            User.Send(response);
        }
    }
}
