using HatchlingNet;
using Header;
using MySqlDataBase;

namespace Server
{
    public class SignUp : IResponse
    {
        public IGameUser User { get; set; }
        SEND_TYPE sendType;
        public string Id { get; private set; }
        public string Password { get; private set; }

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
            MysqlCommand command = new MysqlCommand();
            //db id, password 입력
            if (command.ConnectMysql("localhost", "root", "anstjd"))
            {
                command.OpenDatabase("hatchlingdb");
                command.OpenTable("userinfo");

                isSucess = command.CheckLogin(Id, Password);
            }
        }

        public void Send()
        {
            Packet response;
            if (isSucess)
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.SignUpAck, (short)SEND_TYPE.Single);
            else
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.SignUpRej, (short)SEND_TYPE.Single);

            User.Send(response);
        }
    }
}
