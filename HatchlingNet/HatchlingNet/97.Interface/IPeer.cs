using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HatchlingNet
{
    public interface IPeer
    {
        void Send(Packet msg);

        void Receive();

        void OnMessage(byte[] buffer);//Send와 Receive만 남기고 싶은데...
                                    //매개변수가 어떤식으로 바뀔지 모르니 일단 둔다

        void Destroy();         //각 클라와 서버의 로직에 따라 구현되는 부분....
        void Disconnect();      //통신이 직접적으로 끊기는 부분...dll에서 구현
    }
}
