using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server
{
    public class GameUser : IPeer, IGameUser
    {
        public static NumberingPool objNumberingPool;
        public static Dictionary<int, string> objList = new Dictionary<int, string>();//<remoteID, 객체 태그>

        UserToken userToken;
        public string UserID { get; set; }
        public int GameUserID { get; set; }

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

        public void OnMessage(byte[] buffer)
        {
            Packet msg = new Packet(buffer);
            PROTOCOL protocol = msg.Protocol;

            string protocolName = "Server." + protocol.ToString();
            IResponse response = (IResponse)Activator.CreateInstance(Type.GetType(protocolName));
            if (response != null)
            {
                response.Initialize(this, msg);
                response.Process();
                response.Send();
            }
        }

        public void SendTo(string userId, Packet msg)
        {
            GameUser target = UserList.Instance.GetUser(userId);
            target.Send(msg);
        }


        public void SendTo(string[] userId, Packet msg)
        {
            Parallel.ForEach(
                userId, (id) =>
                {
                    SendTo(id, msg);
                }
            );
        }

        public void SendAll(Packet msg)
        {
            GameUser[] users = UserList.Instance.GetAllUser();
            Parallel.ForEach(
                users, (user) =>
                {
                    user.Send(msg);
                }
            );
        }
        public void SendAllWithoutMe(Packet msg)
        {
            GameUser[] users = UserList.Instance.GetAllUser();
            Parallel.ForEach(
                users, (user) =>
                {
                    if (UserID != user.UserID)
                        user.Send(msg);
                }
            );
        }

        public void Send(Packet msg)
        {
            userToken.Send(msg);
        }

        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.PlayerExit);
            response.Push(GameUserID);
            SendAllWithoutMe(response);

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
