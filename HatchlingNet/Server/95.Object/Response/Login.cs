using HatchlingNet;
using Header;

namespace Server
{
    public class Login : IResponse
    {
        public string Id { get; private set; }
        public string Password { get; private set; }
        public IGameUser Self { get; set; }

        bool isSucess;

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            Id = msg.PopString();
            Password = msg.PopString();
        }

        public void Process()
        {
            //isSucess = command.CheckLogin(id, password);
            isSucess = true;

            if(isSucess)
            {
                Self.UserID = Id;
                UserList.Instance.AddUser(Self as GameUser);
            }
        }

        public void Send()
        {
            Packet response;
            if (isSucess)
                response = PacketBufferManager.Instance.Pop(PROTOCOL.LoginAck);
            else
                response = PacketBufferManager.Instance.Pop(PROTOCOL.LoginRej);

            Self.SendTo(Self.UserID, response);
        }
    }
}
