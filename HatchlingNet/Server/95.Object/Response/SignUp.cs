using HatchlingNet;
using Header;
using MySqlDataBase;
using System.Diagnostics;

namespace Server
{
    public class SignUp : IResponse
    {
        public IGameUser Self { get; set; }
        public string Id { get; private set; }
        public string Password { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public string Mail { get; private set; }
        public string BirthDay { get; private set; }

        bool isSucess;

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            Id = msg.PopString();
            Password = msg.PopString();
            LastName = msg.PopString();
            FirstName = msg.PopString();
            Mail = msg.PopString();
            BirthDay = msg.PopString();
        }

        public void Process()
        {
            MysqlCommand command = new MysqlCommand();
            //db id, password 입력
            if (command.ConnectMysql("localhost", "root", "apmsetup"))
            {
                command.OpenDatabase("hatchlingdb");
                command.OpenTable("userinfo");

                //isSucess = command.CheckLogin(Id, Password);
                Trace.WriteLine("가입 시도");
                isSucess = command.SignUp(Id, Password, LastName, FirstName, Mail, BirthDay);
                Trace.WriteLine("가입 결과" + isSucess);
            }
        }

        public void Send()
        {
            Packet response;
            if (isSucess)
                response = PacketBufferManager.Instance.Pop(PROTOCOL.SignUpAck);
            else
                response = PacketBufferManager.Instance.Pop(PROTOCOL.SignUpRej);

            Self.SendTo(Self, response);
        }
    }
}
