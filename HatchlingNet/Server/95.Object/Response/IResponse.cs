using HatchlingNet;
using System;

namespace Server
{
    public interface IResponse
    {
        IGameUser User { get; set; }
        void Initialize(IGameUser user, Packet msg);
        void Process();
        void Send();
    }
}
