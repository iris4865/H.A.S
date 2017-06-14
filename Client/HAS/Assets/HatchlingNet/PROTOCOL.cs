using System;


namespace HatchlingNet
{
    public enum ANIMATION_TYPE : Int16
    {
        Wait = 0,
        Walk,
        Run,
        Attack
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
