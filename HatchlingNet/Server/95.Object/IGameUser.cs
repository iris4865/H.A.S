using HatchlingNet;

namespace Server
{
    public interface IGameUser
    {
        string UserID { get; set; }

        void SendTo(IGameUser target, Packet msg);
        void SendTo(string userId, Packet msg);
        void SendTo(string[] userId, Packet msg);

        void SendAll(Packet msg);
        void SendAllWithoutMe(Packet msg);
    }
}