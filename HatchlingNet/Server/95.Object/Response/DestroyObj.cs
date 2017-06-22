using HatchlingNet;
using Header;
using System.Collections.Generic;

namespace Server
{
    public class DestroyObj : IResponse
    {

        string remoteId;
        public IGameUser Self { get; set; }
        bool removeResult = false;


        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            remoteId = msg.PopString();

 
        }

        public void Process()
        {
            removeResult = UserList.Instance.RemoveUser(remoteId);
        }

        public void Send()
        {
            Packet msg;

            if (removeResult)
            {
                msg = PacketBufferManager.Instance.Pop(PROTOCOL.DestroyObjAck);
            }
            else
            {
                msg = PacketBufferManager.Instance.Pop(PROTOCOL.DestroyObjRej);
            }

            Self.SendTo(Self, msg);
        }
    }
}
