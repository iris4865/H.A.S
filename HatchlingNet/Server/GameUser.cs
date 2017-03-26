using HatchlingNet;
using INTERFACE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameUser : Peer
    {
        UserToken token;

        public GameUser(UserToken token)
        {
            this.token = token;
            this.token.SetPeer(this);
        }

        public void Destroy()
        {
            Program.RemoveUser(this);
        }

        public void Disconnect()
        {
            Destroy();
            this.token.socket.Disconnect(false);
        }

        public void OnMessage(byte[] buffer)
        {
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

        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void Send()
        {
            throw new NotImplementedException();
        }

        public void Send(Packet msg)
        {
            token.Send(msg);
        }
    }
}
