using System;
using HatchlingNet;
using INTERFACE;

namespace client
{
    public class RemoteServerPeer : Peer
    {
        public UserToken serverToken { get; private set; }

        public RemoteServerPeer(UserToken serverToken)
        {
            this.serverToken = serverToken;

            this.serverToken.SetPeer(this);
        }


        public void OnMessage(byte[] buffer)
        {
            Packet msg = new Packet(buffer, this);
            PROTOCOL protocol_id = (PROTOCOL)msg.PopProtocol_id();

            switch (protocol_id)
            {
                case PROTOCOL.ChatAck:
                    {
                        string text = msg.PopString();
                        Console.WriteLine(string.Format("text {0}", text));
                    }
                    break;
            }
        }


        public void Destroy()
        {
            
        }

        public void Disconnect()
        {
            Destroy();

            this.serverToken.socket.Disconnect(false);
        }


        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void Send(Packet msg)
        {
            this.serverToken.Send(msg);
        }
    }
}