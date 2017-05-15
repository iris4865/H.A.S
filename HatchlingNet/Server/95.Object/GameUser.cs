using HatchlingNet;
using Header;
using MySqlDataBase;
using System;
using System.Collections.Generic;

namespace Server
{
    public class GameUser : IPeer
    {
        static NumberingPool objNumberingPool = null;
        static Dictionary<int, string> objList = new Dictionary<int, string>();//<remoteID, 객체 태그>
        static object csObjList = new object();
        static object csObjNumberingPool = new object();

        UserToken userToken;
        MysqlCommand command;
        public string UserID { get; set; }


        public GameUser(UserToken userToken)
        {
            if (objNumberingPool == null)
            {
                objNumberingPool = new NumberingPool(20000);

                for (int i = 0; i < objNumberingPool.capacity; ++i)
                {
                    int number = new int();
                    number = i;

                    objNumberingPool.Push(number);
                }
            }

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

            switch (protocol)
            {
                case PROTOCOL.SignupReq:
                    {
                        string id = msg.PopString();
                        string password = msg.PopString();

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



                        bool isSignup = command.SignUp(id, password);

                        Packet response;

                        if (isSignup)
                            response = PacketBufferManager.Pop((short)PROTOCOL.SignupAck, (short)SEND_TYPE.Single);
                        else
                            response = PacketBufferManager.Pop((short)PROTOCOL.SignupRej, (short)SEND_TYPE.Single);

                        Send(response);
                    }
                    break;

                case PROTOCOL.LoginReq:
                    {
                        Console.WriteLine("들어옴");

                        string id = msg.PopString();
                        string password = msg.PopString();

                        //                        bool isUser = command.CheckLogin(id, password);
                        bool isUser = true;

                        if (isUser == true)
                        {
                            Packet loginResult = PacketBufferManager.Pop((short)PROTOCOL.LoginAck, (short)SEND_TYPE.Single);
                            loginResult.Push(id);
                            Send(loginResult);

                            //                            userID = new string(id);
                            UserID = id;

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

                case PROTOCOL.PositionReq:
                    {
                        //Packet response = PacketBufferManager.Pop((short)PROTOCOL.PositionAck, (short)SEND_TYPE.BroadcastWithoutMe);
                        //int networkID = msg.PopInt32();

                        Packet response = msg;
                        response.SetProtocol((short)PROTOCOL.PositionAck);
                        response.SetSendType((short)SEND_TYPE.BroadcastWithoutMe);

                        Send(msg);
                    }
                    break;

                case PROTOCOL.ObjNumberingReq:
                    {
                        Packet response = PacketBufferManager.Pop((short)PROTOCOL.ObjNumberingAck, (short)SEND_TYPE.BroadcastWithMe);
                        string objTag = msg.PopString();
                        MyVector3 position; position.x = msg.PopFloat(); position.y = msg.PopFloat(); position.z = msg.PopFloat();

                        response.Push(objTag);//태그
                        response.Push(position.x);//위치
                        response.Push(position.y);//위치
                        response.Push(position.z);//위치

                        int number = 0;
                        lock (objNumberingPool)
                        {
                            number = objNumberingPool.Pop();
                        }

                        Console.WriteLine("태그 : " + objTag + " 위치 x : " + position.x + " y : " + position.y + " z : " + position.z + " remoteID : " + number);


                        response.Push(number);        //remote ID
                        response.Push(UserID);         //만약 이 메세지를 받은 클라의 userID와 같으면 그건 그사람이 주체적으로 만든거고
                                                       //그 플레이어 조종하려고 만든거일 확률이 높음

                        Send(response);

                        int otherPlayerNum = objList.Count;



                        foreach (var iter in objList)
                        {
                            Packet otherPlayer = PacketBufferManager.Pop((short)PROTOCOL.ObjNumberingAck, (short)SEND_TYPE.Single);
                            string otherPlayerTag;

                            otherPlayerTag = iter.Value;

                            MyVector3 otherPlayerPos; otherPlayerPos.x = 0f; otherPlayerPos.y = 10f; otherPlayerPos.z = 0f;
                            otherPlayer.Push(objTag);//태그
                            otherPlayer.Push(otherPlayerPos.x);//위치
                            otherPlayer.Push(otherPlayerPos.y);//위치
                            otherPlayer.Push(otherPlayerPos.z);//위치
                            otherPlayer.Push(iter.Key);//remoteID
                            otherPlayer.Push("None");//remoteID
                            Send(otherPlayer);
                        }

                        objList.Add(number, objTag);
                    }
                    break;

                //case PROTOCOL.CreateObjReq:
                //    {
                //        int objNumbering = msg.PopInt32();
                //        string objTag = msg.PopString();

                //        lock(objList)
                //            objList.Add(objNumbering, objTag);

                //        Packet response = PacketBufferManager.Pop((short)PROTOCOL.CreateObjAck, (short)SEND_TYPE.BroadcastWithMe);
                //        response.Push(objNumbering);
                //        response.Push(objTag);
                //        Send(response);
                //    }

                //    break;
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
