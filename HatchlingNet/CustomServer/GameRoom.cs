using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomServer
{
    class GameRoom
    {
        enum PLAYER_STATE
        {
            ENTER_ROOM,
            LOADING_COMPLETE
        }

        List<Player> playerList;

        public GameRoom()
        {
            this.playerList = new List<PLAYER_STATE>();
        }

        void broadcast(Packet msg)
        {
            this.playerList.ForEach(player => player.SendForBroadcast(msg));
            PacketBufferManager.Push(msg);
        }

        public void EnterRoom(GameUser user)
        {
            Player player = new Player(user);
            playerList.Add(player);

            //플레이어들의 초기상태 지정 함수...
        }

        ...근데 이건 게임방을 어떤식으로 만드냐에 따라 필요없을것 같기도...
            만약 게임대기방이 플레이할떄와 다르게 동작하는게 많다면...크아 대기실같은경우는
            필요할지도 모르지만 우리는 아니지않나?접속하면 바로 움직이고 떄리고
            가능한거 아닌가?그렇다면 필요없을거같은데...
            ...
            ..
            ..
            있어야될듯...방이 하나는 아니니까 있긴 있어야되는데
            주목적이 방에 참가한 유저들의 리스트와 브로드캐스팅을 위한거고


    }
}
