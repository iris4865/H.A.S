using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//클라에게 제공하게될 인터페이스가 되겠네


public class NetworkManager : MonoBehaviour {

    private static NetworkManager instance = null;
    public static NetworkManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;

                if (instance == null)
                {
                    Debug.LogError("no active NetworkManager ");
                }
            }

            return instance;
        }
    }

    HatchlingNetUnityService gameserver;

    void Awake()
    {
        //DontDestroyOnLoad(this);
        this.gameserver = gameObject.AddComponent<HatchlingNetUnityService>();
        this.gameserver.callbackAppStatusChanged += CallStatusChange;
        this.gameserver.callbackAppReceiveMessage += CallMessage;

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
                    Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ChatReq, (short)SEND_TYPE.BroadcastWithMe);
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

    void CallMessage(Packet msg)//클라이언트 수신부
    {
        PROTOCOL protocolType = (PROTOCOL)msg.PopProtocolType();
        SEND_TYPE sendType = (SEND_TYPE)msg.PopSendType();

        Debug.Log("콜메세지 " + protocolType);

        switch (protocolType)
        {
            case PROTOCOL.ChatAck:
                {
                    string text = msg.PopString();
                }
                break;

            case PROTOCOL.LoginAck:
                {
                    Debug.Log("로그인액크");
//                    SceneManager.LoadScene(3);
                    SceneManager.LoadScene(4);
                }
                break;

            case PROTOCOL.LoginRej:
                {
                    //다시입력하라고
                }
                break;

            case PROTOCOL.PositionAck:
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
