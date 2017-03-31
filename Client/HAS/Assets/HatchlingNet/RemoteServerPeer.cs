using HatchlingNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RemoteServerPeer : IPeer {

    public UserToken serverToken { get; private set; }
    public WeakReference hatchlingnetEventManager;
    //WeakReference란? http://kookiandkiki.blogspot.kr/2014/01/c-weakreference-idisosalbe.html
    //메모리가 가비지컬렉터에 의해서 깔끔하게 관리될것같지만 
    //의도하지않게 어디선가 계속 참조가 일어날수 있음....
    //매니저에서 삭제했지만 다른데서 계속 참조하고있을경우...
    //그러한경우를 방지해 준다.


    public RemoteServerPeer(UserToken serverToken)
    {
        this.serverToken = serverToken;

        this.serverToken.SetPeer(this);
    }

    //멤버변수로 갖는 이벤트매니저가 WeakReference라서 객체만들기 위해 프로퍼티(변수옆에 get;set; 으로 겟터셋터만드는거)사용안하고 별도로 만든다
    public void SetEventManager(HatchlingNetEventManager eventManager)
    {
        hatchlingnetEventManager = new WeakReference(eventManager);
    }

    public void OnMessage(byte[] buffer)
    {
        byte[] app_buffer = new byte[buffer.Length];
        Array.Copy(buffer, app_buffer, buffer.Length);

        Packet msg = new Packet(app_buffer, this);

        //PROTOCOL protocol = (PROTOCOL)msg.PopProtocolType();
        //Debug.Log("온메세지에서 프로토콜 확인 : " + protocol);
            
        //Debug.Log("온메세지" + System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length));

        (this.hatchlingnetEventManager.Target as HatchlingNetEventManager).EnqueueNetworkMessage(msg);

        //PROTOCOL protocol_id = (PROTOCOL)msg.PopProtocol_id();
        //switch (protocol_id)
        //{
        //    case PROTOCOL.ChatAck:
        //        {
        //            string text = msg.PopString();
        //            Console.WriteLine(string.Format("text {0}", text));
        //        }
        //        break;
        //}
    }


    public void Destroy()
    {
        (this.hatchlingnetEventManager.Target as HatchlingNetEventManager).EnqueueNetworkEvent(NETWORK_EVENT.disconnected);
    }

    public void Disconnect()
    {

    }


    public void Receive()
    {
        throw new NotImplementedException();
    }

    public void Send(Packet msg)
    {
        this.serverToken.Send(msg);
    }

    public void ProcessUserOperation(Packet msg)
    {
        throw new NotImplementedException();
    }
}
