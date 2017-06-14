using HatchlingNet;
using Header;
using MySqlDataBase;
using System;
using System.Collections.Generic;

namespace Server
{
    public class GameUser : IPeer
    {
        PacketBufferManager packetBuffer = PacketBufferManager.Instance;
        public static NumberingPool objNumberingPool;
        public static Dictionary<int, string> objList = new Dictionary<int, string>();//<remoteID, 객체 태그>
        static object csObjList = new object();
        static object csObjNumberingPool = new object();

        UserToken userToken;
        MysqlCommand command;
        public string UserID { get; set; }

        public int GameUserID { get; set; }
        Dictionary<PROTOCOL, Action<Packet>> respondseMethods = new Dictionary<PROTOCOL, Action<Packet>>();

        public GameUser(UserToken userToken)
        {
            if (objNumberingPool == null)
            {
                objNumberingPool = new NumberingPool(20000);

                for (int i = 0; i < objNumberingPool.Capacity; ++i)
                    objNumberingPool.Push(i);
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

            string protocolName = "Server."+protocol.ToString();
            IResponse response = (IResponse)Activator.CreateInstance(Type.GetType(protocolName));
            if (response != null)
            {
                response.Initialize(msg);
                response.Process(this);
                response.Send(Send);
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
                    userToken.Broadcast(msg, userToken.TokenID);
                    break;

                case SEND_TYPE.BroadcastWithMe:
                    userToken.BroadCastWithMe(msg);
                    break;
            }

        }

        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            Packet response = packetBuffer.Pop((short)PROTOCOL.PlayerExit, (short)SEND_TYPE.BroadcastWithoutMe);
            response.Push(GameUserID);
            Send(response);

            objNumberingPool.Push(GameUserID);
            UserList.Instance.RemoveUser(this);
            objList.Remove(GameUserID);
            GameUserID = -1;
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
