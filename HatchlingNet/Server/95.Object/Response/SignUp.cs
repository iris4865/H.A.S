using HatchlingNet;
using Header;
using MySqlDataBase;
using System;

namespace Server
{
    public class SignUp : IResponse
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
            MysqlCommand command = new MysqlCommand();
            //db id, password 입력
            if (command.ConnectMysql("localhost", "root", "anstjd"))
            {
                command.OpenDatabase("hatchlingdb");
                command.OpenTable("userinfo");

                isSucess = command.CheckLogin(Id, Password);
            }
        }

        public void Send(Action<Packet> send)
        {
            Packet response;
            if (isSucess)
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.SignUpAck, (short)SEND_TYPE.Single);
            else
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.SignUpRej, (short)SEND_TYPE.Single);

            send(response);
        }
    }
}
