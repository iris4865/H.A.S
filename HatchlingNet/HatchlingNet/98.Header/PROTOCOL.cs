using System;


namespace Header
{
    public enum PROTOCOL : Int16
    {
        Login = 1000, LoginAck, LoginRej,
        SignUp = 1100, SignUpAck, SignUpRej,

        Chat = 2000, ChatAck,
        Position = 3000, PositionAck,
        CreateObj = 4000, CreateObjAck, CreateObjRej,
        DestroyObj, DestroyObjAck, DestroyObjRej,
        ObjectNumbering, ObjectNumberingAck, ObjectNumberingRej,
        PlayerExit,

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
