using HatchlingNet;
using Header;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Server
{
    public class GameUser : IPeer, IGameUser
    {
        UserToken userToken;
        public string UserID { get; set; }
        public int Job { get; set; }

        public GameUser(UserToken userToken)
        {
            this.userToken = userToken;

            userToken.Peer = this;
        }

        public void OnMessage(byte[] buffer)
        {
            Packet msg = new Packet(buffer);
            PROTOCOL protocol = msg.Protocol;

            string protocolName = "Server." + protocol.ToString();
            try
            {
                IResponse response = (IResponse)Activator.CreateInstance(Type.GetType(protocolName));
                response.Initialize(this, msg);
                response.Process();
                response.Send();
            }
            catch
            {
                Trace.WriteLine($"ip: {userToken.socket.LocalEndPoint} user: {UserID} protocol: {protocol.ToString()}");
            }
        }

        public void SendTo(IGameUser target, Packet msg)
        {
            if (target is GameUser)
                (target as GameUser).Send(msg);
            else
                Trace.TraceError("not find GameUser");
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
            response.Push(UserID);
            SendAllWithoutMe(response);

            UserList.Instance.RemoveUser(this.UserID);
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
