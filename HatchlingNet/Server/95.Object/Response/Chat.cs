﻿using HatchlingNet;
using Header;

namespace Server
{
    public class Chat : IResponse
    {
        SEND_TYPE sendType;
        string text = "";

        public IGameUser Self { get; set; }

        //그룹일때 상정해서 코드 수정 요망
        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            sendType = (SEND_TYPE)msg.PopInt16();
            text = msg.PopString();
        }

        public void Process()
        {
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.ChatAck);
            response.Push((short)sendType);
            response.Push(text);
            switch(sendType)
            {
                case SEND_TYPE.Single:
                    Self.SendTo(Self, response);
                    break;
                case SEND_TYPE.BroadcastWithMe:
                    //
                    break;
                case SEND_TYPE.BroadcastWithoutMe:
                    Self.SendAllWithoutMe(response);
                    break;
            }
        }
    }
}
