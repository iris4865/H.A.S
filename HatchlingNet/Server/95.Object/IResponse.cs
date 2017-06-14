using HatchlingNet;
using System;

namespace Server
{
    public interface IResponse
    {
        void Initialize(Packet msg);
        void Process(GameUser user);
        void Send(Action<Packet> send);
    }
}
