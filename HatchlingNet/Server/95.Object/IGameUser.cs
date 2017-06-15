using HatchlingNet;

namespace Server
{
    public interface IGameUser
    {
        int GameUserID { get; set; }
        string UserID { get; set; }

        void Send(Packet msg);
    }
}