using System;


namespace Header
{
    public enum PROTOCOL : Int16
    {
        LoginReq = 1000, LoginAck, LoginRej,
        SignupReq = 1100, SignupAck, SignupRej,

        ChatReq = 2000, ChatAck,
        PositionReq = 3000, PositionAck,
        CreateObjReq = 4000, CreateObjAck, CreateObjRej,
        DestroyObjReq, DestroyObjAck, DestroyObjRej,
        ObjNumberingReq, ObjNumberingAck, ObjNumberingRej,

        END
    }

    public enum SEND_TYPE : Int16
    {
        Single = 1,
        BroadcastWithoutMe,
        BroadcastWithMe
    }

    public enum SERVER_TYPE : Int16
    {


    }


}
