using HatchlingNet;
using Header;

namespace Server
{
    public class Login : IResponse
    {
        public string Id { get; private set; }
        public string Password { get; private set; }
        public IGameUser User { get; set; }

        bool isSucess;

        public void Initialize(IGameUser user, Packet msg)
        {
            User = user;
            Id = msg.PopString();
            Password = msg.PopString();
        }

        public void Process()
        {
            //isSucess = command.CheckLogin(id, password);
            isSucess = true;

            if(isSucess)
            {
                User.UserID = Id;
                UserList.Instance.AddUser(User as GameUser);
            }
        }

        public void Send()
        {
            Packet response;
            if (isSucess)
                response = PacketBufferManager.Instance.Pop(PROTOCOL.LoginAck);
            else
                response = PacketBufferManager.Instance.Pop(PROTOCOL.LoginRej);

            User.SendTo(User.UserID, response);
        }
    }
}
