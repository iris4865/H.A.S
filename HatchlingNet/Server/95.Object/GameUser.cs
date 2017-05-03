using HatchlingNet;
using Header;
using MySqlDataBase;
using System;

namespace Server
{
    public class GameUser : IPeer
    {
        UserToken userToken;
        MysqlCommand command;
        public string UserID { get; set; }


        public GameUser(UserToken userToken)
        {
            this.userToken = userToken;

            userToken.Peer = this;
        }

        /*
         * 서버에서 사용하게될 GameUser클래스와
         * 클라에서 사용하게될 RemoteServerPeer의 차이는
         * 수신패킷 완성후 호출되는 OnMessage에서 되돌려주기 위해
         * send를 호출하냐 안하냐 차이 이다
         * RemoteServerPeer는 주로 서버의 응답에 따라 동작방향을 결정하는
         * 로직을 주로 짜게될테고
         * GaemUser클래스는 클라의 요청에 따라 어떻게 동작할지 결정하게 되겠지
         */
        public void OnMessage(byte[] buffer)
        {
            Packet msg = new Packet(buffer, this);
            PROTOCOL protocol = (PROTOCOL)msg.PopProtocolType();
            SEND_TYPE sendType = (SEND_TYPE)msg.PopSendType();


            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("protocolType " + protocol);

            Packet response = null;
            switch (protocol)
            {
                case PROTOCOL.SignupReq:
                    {
                        //MySQLConnecter mysql = new MySQLConnecter("localhost", "apmsetup");
                        //mysql.Open();

                        command = new MysqlCommand();
                        //db id, password 입력
                        command.ConnectMysql("localhost", "id", "password");

                        command.OpenDatabase("hatchlingdb");
                        command.OpenTable("userinfo");

                        if (command.CheckLogin("a", "d"))
                            Console.WriteLine("login sucess");
                        else
                            Console.WriteLine("fail");

                        string id = msg.PopString();
                        string password = msg.PopString();
                        bool isSignup = command.SignUp(id, password);

                        if (isSignup)
                            response = PacketBufferManager.Pop((short)PROTOCOL.SignupAck, (short)SEND_TYPE.Single);
                        else
                            response = PacketBufferManager.Pop((short)PROTOCOL.SignupRej, (short)SEND_TYPE.Single);
                    }
                    break;

                case PROTOCOL.LoginReq:
                    {
                        Console.WriteLine("들어옴");

                        string id = msg.PopString();
                        string password = msg.PopString();
                        bool isUser = command.CheckLogin(id, password);

                        if (isUser == true)
                        {
                            response = PacketBufferManager.Pop((short)PROTOCOL.LoginAck, (short)SEND_TYPE.Single);
                            response.Push(id);
                            UserID = id;
                        }
                        else
                            response = PacketBufferManager.Pop((short)PROTOCOL.LoginRej, (short)SEND_TYPE.Single);

                    }
                    break;

                case PROTOCOL.ChatReq:
                    {
                        string text = msg.PopString();
                        Console.WriteLine(string.Format("text {0}", text));

                        response = PacketBufferManager.Pop((short)PROTOCOL.ChatAck, (short)sendType);

                        response.Push(text);
                    }
                    break;

                case PROTOCOL.PositionAck:
                    {

                    }
                    break;
            }
            if (response != null)
                Send(response);
        }

        public void Send(Packet msg)
        {
            switch ((SEND_TYPE)msg.PeekSendType())
            {
                case SEND_TYPE.Single:
                    userToken.Send(msg);
                    break;

                case SEND_TYPE.BroadcastWithoutMe:
                    userToken.CallbackBroadcast(msg, userToken.TokenID);
                    break;

                case SEND_TYPE.BroadcastWithMe:
                    userToken.CallbackBroadcast(msg);

                    break;

            }

        }

        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            UserList.Instance.RemoveUser(this);
        }

        public void Disconnect()
        {
            Destroy();
            this.userToken.socket.Disconnect(false);
        }

        public void ProcessUserOperation()
        {
            throw new NotImplementedException();
        }

        public void ProcessUserOperation(Packet msg)
        {
            throw new NotImplementedException();
        }
    }
}
