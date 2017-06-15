using HatchlingNet;

namespace Server
{
    public interface IGameUser
    {
        int GameUserID { get; set; }
        string UserID { get; set; }
        
        void SendTo(string userId, Packet msg);
        void SendTo(string[] userId, Packet msg);

        void SendAll(Packet msg);
        void SendAllWithoutMe(Packet msg);
    }
}