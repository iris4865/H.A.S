using HatchlingNet;
using Header;
using MySqlDataBase;
using System.Diagnostics;

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
            //isSucess = true;

            MysqlCommand command = new MysqlCommand();
            //db id, password 입력
            if (command.ConnectMysql("localhost", "root", "apmsetup"))
            {
                command.OpenDatabase("hatchlingdb");
                command.OpenTable("userinfo");

                //isSucess = command.CheckLogin(Id, Password);
                isSucess = command.CheckLogin(Id, Password);
            }

           
            if (isSucess)
            {
                Self.UserID = Id;
                UserList.Instance.AddUser(Self as GameUser);
                Trace.WriteLine($"{Self.UserID} is Login.");
            }

        }

        public void Send()
        {
            Packet response;
            if (isSucess)
                response = PacketBufferManager.Instance.Pop(PROTOCOL.LoginAck);
            else
                response = PacketBufferManager.Instance.Pop(PROTOCOL.LoginRej);

            Self.SendTo(Self, response);
        }
    }
}
