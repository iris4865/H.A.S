using HatchlingNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameUser : IPeer
    {
        UserToken userToken;

        public GameUser(UserToken userToken)
        {
            this.userToken = userToken;

            this.userToken.SetPeer(this);
        }
        
        public void OnMessage(byte[] buffer)//서버에서 사용하게될 GameUser클래스와
        {                                   //클라에서 사용하게될 RemoteServerPeer의 차이는
                                            //수신패킷 완성후 호출되는 OnMessage에서 되돌려주기 위해
                                            //send를 호출하냐 안하냐 차이 이다
                                            //RemoteServerPeer는 주로 서버의 응답에 따라 동작방향을 결정하는
                                            //로직을 주로 짜게될테고
                                            //GaemUser클래스는 클라의 요청에 따라 어떻게 동작할지 결정하게 되겠지
            Packet msg = new Packet(buffer, this);
            PROTOCOL protocol = (PROTOCOL)msg.PopProtocol_id();

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("protocol id " + protocol);

            switch (protocol)
            {
                case PROTOCOL.ChatReq:
                    {
                        string text = msg.PopString();
                        Console.WriteLine(string.Format("text {0}", text));
                        
                        Packet response = PacketBufferManager.Pop((short)PROTOCOL.ChatAck);

                        response.Push(text);
                        Send(response);
                    }
                    break;
                    
            }
        }

        public void Send(Packet msg)
        {
            userToken.Send(msg);
        }
        
        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            Program.RemoveUser(this);
        }

        public void Disconnect()
        {
            Destroy();
            this.userToken.socket.Disconnect(false);
        }

    }
}
