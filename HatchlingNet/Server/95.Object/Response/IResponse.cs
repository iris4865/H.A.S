using HatchlingNet;
using System;

namespace Server
{
    public interface IResponse
    {
        IGameUser Self { get; set; }
        void Initialize(IGameUser user, Packet msg);
        void Process();
        void Send();
    }
}
