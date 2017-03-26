using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTERFACE
{
    public interface Peer
    {
        void Send(Packet msg);

        void Receive();

        void OnMessage(byte[] buffer);//Send와 Receive만 남기고 싶은데...
                                    //매개변수가 어떤식으로 바뀔지 모르니 일단 둔다

        void Destroy();
        void Disconnect();
    }
}
