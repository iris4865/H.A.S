using HatchlingNet;
using Header;
using System.Collections.Generic;

namespace Server
{
    public class DestroyObj : IResponse
    {
        string type;
        string remoteId;
        public IGameUser Self { get; set; }

        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            type = msg.PopString();
            remoteId = msg.PopString();

 
        }

        public void Process()
        {
//            removeResult = UserList.Instance.RemoveUser(remoteId);
        }

        public void Send()
        {
            Packet msg;
            msg = PacketBufferManager.Instance.Pop(PROTOCOL.DestroyObjAck);
            msg.Push(type);
            msg.Push(remoteId);
            Self.SendAllWithoutMe(msg);
        }
    }
}
