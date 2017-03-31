using System;
using HatchlingNet;

namespace client
{
    public class RemoteServerPeer : IPeer
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
            PROTOCOL protocol_id = (PROTOCOL)msg.PopProtocolType();

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

        public void Send(Packet msg)
        {
            this.serverToken.Send(msg);
        }

        public void Receive()
        {
            throw new NotImplementedException();
        }
        public  void Destroy()
        {
            
        }

        public void Disconnect()
        {
            Destroy();

            this.serverToken.socket.Disconnect(false);
        }

        public void ProcessUserOperation(Packet msg)
        {
            throw new NotImplementedException();
        }
    }
}