using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//클라에게 제공하게될 인터페이스가 되겠네

public class NetworkManager : MonoBehaviour {

    HatchlingNetUnityService gameserver;
    object targetComponent;

    void Awake()
    {
        this.gameserver = gameObject.AddComponent<HatchlingNetUnityService>();
        this.gameserver.callbackAppStatusChanged += CallStatusChange;
        this.gameserver.callbackAppReceiveMessage += CallMessage;
//        guide = GameObject.Find("networkGuide").GetComponent<NetworkGuide>();

    }

    void Connect()
    {
        this.gameserver.Connect("127.0.0.1", 7979);
    }

    void Start () {
        Connect();		
	}

    void CallStatusChange(NETWORK_EVENT status)
    {
        switch (status)
        {
            case NETWORK_EVENT.connected:
                {
                    Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ChatReq);
                    msg.Push("Hello~!");

                    this.gameserver.Send(msg);
                }
                break;

            case NETWORK_EVENT.disconnected:
                {

                }
                break;
        }
    }

    void CallMessage(Packet msg)
    {
        //        msg.position = 2;
//        Packet msg = new Packet(test.buffer, null);
        PROTOCOL protocolType = (PROTOCOL)msg.PopProtocolType();
        Debug.Log("콜메세지 " + protocolType);

        switch (protocolType)
        {
            case PROTOCOL.ChatAck:
                {
                    string text = msg.PopString();
//                    GameObject.Find("networkGuide").GetComponent<NetworkGuide>().CallReceiveChatMsg(text);
                    //굳이 이렇게 매번 파인드할필요가 있나? 
                    //거슬러 가는게 원칙에 위배되서 그런걸수도...
                }
                break;

            case PROTOCOL.LoginAck:
                {
                    Debug.Log("로그인액크");
                    SceneManager.LoadScene(3);
                }
                break;

            case PROTOCOL.LoginRej:
                {
                    //다시입력하라고
                }
                break;


            default:
                {
                    string text = msg.PopString();
                    Debug.Log("그외 " + text);
                }
                break;


        }
    }

    public void Send(Packet msg)
    {
        this.gameserver.Send(msg);
    }

	
}
