﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HatchlingNet
{
    public enum PROTOCOL : Int16
    {
        LoginReq = 1000, LoginAck, LoginRej,
        ChatReq = 2000, ChatAck,
        PositionReq = 3000, PositionAck,
        END
    }

    public enum SEND_TYPE : Int16
    {
        Single = 1,
        BroadcastWithoutMe,
        BroadcastWithMe
    }

    public enum NETWORK_EVENT
    {
        // 접속 완료.
        connected,

        // 연결 끊김.
        disconnected,

        // 끝.
        end
    }
}