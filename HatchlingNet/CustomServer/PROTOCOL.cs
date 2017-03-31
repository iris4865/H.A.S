using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HatchlingNet
{
    public enum PROTOCOL
    {
        LoginReq = 1000, LoginAck, LoginRej,
        ChatReq = 2000, ChatAck,

        END
    }
}
