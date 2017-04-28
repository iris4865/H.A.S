using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using HatchlingNet;



public class HatchlingNetUnityService : MonoBehaviour
{
    HatchlingNetEventManager eventManager;
    IPeer gameserver;       //연결된 서버 객체
    NetworkService service;//tcp통신을 위한 서비스 객체

    //접속상태 변화시 호출될 델리게이트
    public delegate void StatusChangeHandler(NETWORK_EVENT status);
    public StatusChangeHandler callbackAppStatusChanged;

    //네트워크 메시지 수신시 호출할 델리게이트
    public delegate void MessageHandler(Packet msg);
    public MessageHandler callbackAppReceiveMessage;

    public void Awake()
    {
        PacketBufferManager.Initialize(10);
        this.eventManager = new HatchlingNetEventManager();
    }

    public void Connect(string host, int port)
    {
        this.service = new NetworkService();


        Connector connector = new Connector(service);
        connector.callbackConnect += on_connected_gameserver;

        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        connector.connect(endpoint);
    }

    public void on_connected_gameserver(UserToken server_token)
    {
        this.gameserver = new RemoteServerPeer(server_token);//서버에 대한 토큰 보관하고

        //서버객체에서 이벤트매니저로 인큐, 디큐 자주할꺼니까 객체 넘겨주고
        ((RemoteServerPeer)this.gameserver).SetEventManager(this.eventManager);

        this.eventManager.EnqueueNetworkEvent(NETWORK_EVENT.connected);
    }

    void Update()
    {
        if (this.eventManager.HasMessage() == true)
        {
            Packet msg = this.eventManager.DequeueNetworkMessage();
            //            this.callbackAppReceiveMessage(msg);

            if (this.callbackAppReceiveMessage != null)
            {
                this.callbackAppReceiveMessage(msg);
            }
        }

        if (this.eventManager.HasEvent() == true)//네트워크이벤트는 상대방이 보내주는게 아니라 
                                            //서버와 통신하면서 돌아가는 로직에따라 자연스럽게 생김
        {
            NETWORK_EVENT status = this.eventManager.DequeueNetworkEvent();
            this.callbackAppStatusChanged(status);
        }
    }

    public void Send(Packet msg)
    {
        try
        {
            this.gameserver.Send(msg);
            PacketBufferManager.Push(msg);
        }
        catch (Exception e)
        {

        }
    }

    //정상종료시 아래메소드에서 disconnect를 호출해줘야 유니티가 hang(?)되지 않는다고 한다
    void OnApplicationQuit()
    {
        Debug.Log("앱콰이트1");

        if (this.gameserver != null)
        {
            Debug.Log("앱콰이트2");
            ((RemoteServerPeer)this.gameserver).serverToken.disconnect();
        }
    }

    //void OnDestroy()
    //{
    //    Debug.Log("앱콰이트1");

    //    if (this.gameserver != null)
    //    {
    //        Debug.Log("앱콰이트2");
    //        ((RemoteServerPeer)this.gameserver).serverToken.disconnect();
    //    }
    //}

}
