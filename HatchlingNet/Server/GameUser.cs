using DataBase;
using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameUser : IPeer
    {
        UserToken userToken;

        public GameUser(UserToken userToken)
        {
            this.userToken = userToken;

            this.userToken.SetPeer(this);
        }
        
        public void OnMessage(byte[] buffer)//서버에서 사용하게될 GameUser클래스와
        {                                   //클라에서 사용하게될 RemoteServerPeer의 차이는
                                            //수신패킷 완성후 호출되는 OnMessage에서 되돌려주기 위해
                                            //send를 호출하냐 안하냐 차이 이다
                                            //RemoteServerPeer는 주로 서버의 응답에 따라 동작방향을 결정하는
                                            //로직을 주로 짜게될테고
                                            //GaemUser클래스는 클라의 요청에 따라 어떻게 동작할지 결정하게 되겠지
            Packet msg = new Packet(buffer, this);
            PROTOCOL protocol = (PROTOCOL)msg.PopProtocolType();
            SEND_TYPE sendType = (SEND_TYPE)msg.PopSendType();


            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("protocolType " + protocol);

            switch (protocol)
            {
                case PROTOCOL.SignupReq:
                    {
                        string id = msg.PopString();
                        string password = msg.PopString();

                        MySQLConnecter mysql = new MySQLConnecter("localhost", "anstjd");
                        mysql.Open();

                        string databaseName = "HatchlingDB";//디비 생성
                        if (!mysql.ConnectDatabase(databaseName))
                        {
                            mysql.CreateDatabase(databaseName);

                            if (!mysql.ConnectDatabase(databaseName))
                                Console.WriteLine("where is database?");
                        }


                        string tableName = "userinfo";          //테이블 생성
                        if (!mysql.ConnectTable(tableName))
                        {
                            mysql.CreateTable(tableName, "no");

                            if (!mysql.ConnectTable("userinfo"))//혹시모를 예외처리
                                Console.WriteLine("where is table?");

                            mysql.AddColumn(MySQLDataType.VARCHAR, "id", 20, false);
                            mysql.AddColumn(MySQLDataType.VARCHAR, "password", 20, false);
                        }



                        bool isSignup = mysql.SignUp(id, password);

                        Packet response;

                        if (isSignup == true)
                        {
                            response = PacketBufferManager.Pop((short)PROTOCOL.SignupAck, (short)SEND_TYPE.Single);
                        }
                        else
                        {
                            response = PacketBufferManager.Pop((short)PROTOCOL.SignupRej, (short)SEND_TYPE.Single);
                        }
                        Send(response);

                    }
                    break;

                case PROTOCOL.LoginReq:
                    {
                        Console.WriteLine("들어옴");

                        bool isUser = true;




                        if (isUser == true)
                        {
                            Packet loginResult = PacketBufferManager.Pop((short)PROTOCOL.LoginAck, (short)SEND_TYPE.Single);
                            Send(loginResult);
                        }
                        else
                        {
                            Packet loginResult = PacketBufferManager.Pop((short)PROTOCOL.LoginRej, (short)SEND_TYPE.Single);
                            Send(loginResult);
                        }

                    }
                    break;


                case PROTOCOL.ChatReq:
                    {
                        string text = msg.PopString();
                        Console.WriteLine(string.Format("text {0}", text));

                        Packet response = PacketBufferManager.Pop((short)PROTOCOL.ChatAck, (short)sendType);

                        response.Push(text);
                        Send(response);
                    }
                    break;

            }
        }



        public void Send(Packet msg)
        {
            switch ((SEND_TYPE)msg.PeekSendType())
            {
                case SEND_TYPE.Single:
                    userToken.Send(msg);
                    break;

                case SEND_TYPE.BroadcastWithoutMe:

                    break;

                case SEND_TYPE.BroadcastWithMe:
                    userToken.callbackBroadcast(msg);

                    break;

            }

        }
        
        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            Program.RemoveUser(this);
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
