using System;
using HatchlingNet;

namespace client
{
    public class RemoteServerPeer : IPeer
    {
        public UserToken serverToken { get; private set; }
        public string userID;

        public RemoteServerPeer(UserToken serverToken)
        {
            this.serverToken = serverToken;

            this.serverToken.SetPeer(this);
        }


        public void OnMessage(byte[] buffer)
        {
            Packet msg = new Packet(buffer, this);
            PROTOCOL protocolType = (PROTOCOL)msg.PopProtocolType();
            SEND_TYPE sendType = (SEND_TYPE)msg.PopSendType();

            switch (protocolType)
            {
                case PROTOCOL.ChatAck:
                    {
                        string text = msg.PopString();
                        Console.WriteLine(string.Format("text {0}", text));
                    }
                    break;

                case PROTOCOL.SignupAck:
                    {
                        Console.WriteLine(string.Format("가입완료!"));
                    }
                    break;

                case PROTOCOL.SignupRej:
                    {
                        Console.WriteLine(string.Format("가입불허...!"));
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