using HatchlingNet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//클라에게 제공하게될 인터페이스가 되겠네


public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance = null;
    public static NetworkManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;
//                instance = new NetworkManager();

                if (instance == null)
                    Debug.LogError("no active NetworkManager ");
                else
                    Debug.Log("active NetworkManager ");

            }

            return instance;
        }
    }

    HatchlingNetUnityService gameserver;
    Dictionary<int, GameObject> networkObj;
    public Queue<GameObject> numberingWaitObj {get; set;}
    public int networkID { get; set; }
    public string userID { get; set; }

    void Awake()
    {
        //if (instance != null)
        //    return;
        DontDestroyOnLoad(this);
        this.gameserver = gameObject.AddComponent<HatchlingNetUnityService>();
        this.gameserver.callbackAppStatusChanged += CallStatusChange;
        this.gameserver.callbackAppReceiveMessage += CallMessage;
        networkObj = new Dictionary<int, GameObject>();
        numberingWaitObj = new Queue<GameObject>();

        userID = "test";

        Debug.Log("ㅇㅇ");
    }

    void Start()
    {
        Debug.Log("ㅇㅇㅁ");
        Connect();
    }

    void Connect()
    {
        this.gameserver.Connect("127.0.0.1", 7979);
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
                    //SceneManager.LoadScene(3);
                    SceneManager.LoadScene(4);
                }
                break;

            case PROTOCOL.LoginRej:
                {
                    //다시입력하라고
                }
                break;

            case PROTOCOL.PositionAck:
                {
                    int networkID = msg.PopInt32();
                    Vector3 position;
                    position.x = msg.PopFloat(); position.y = msg.PopFloat(); position.z = msg.PopFloat();

                    lock (networkObj)
                    {
                        if (networkObj.ContainsKey(networkID) == true)
                        {
                            networkObj[networkID].GetComponent<Transform>().position = position;
                        }
                    }
                }
                break;

            case PROTOCOL.ObjNumberingAck:
                {
                    GameObject obj = null;
                    networkID = msg.PopInt32();

                    lock (numberingWaitObj)
                    {
                        obj = numberingWaitObj.Dequeue();
                    }

                    lock (networkObj)
                    {
                        networkObj.Add(networkID, obj);
                    }

                    PacketBufferManager.Push(msg);
                    msg = PacketBufferManager.Pop((short)PROTOCOL.CreateObjReq, (short)SEND_TYPE.Single);
                    msg.Push(networkID);
                    msg.Push(obj.tag);//tag는 유니티 내에서 각 객체에 설정된거임...
                                      //자세한건 유니티 실행 후  윈도우탭/inspector탭/  을 참고
                    msg.Push(transform.position.x, transform.position.y, transform.position.z);

                    Send(msg);

                    
                }
                break;

            case PROTOCOL.CreateObjAck:
                {
                    int objNumbering = msg.PopInt32();
                    string objTag = msg.PopString();
                    Vector3 position;   position.x = msg.PopFloat(); position.y = msg.PopFloat(); position.z = msg.PopFloat();
                    
                    lock (networkObj)
                    {
                        //   GameObject obj = Instantiate(Resources.Load("Prefabs/" + "Player") as GameObject, position, new Quaternion(0, 0, 0, 0));
                        GameObject obj = Instantiate(Resources.Load("Prefabs/" + "Player") as GameObject, position, new Quaternion(0, 0, 0, 0));

                        networkObj.Add(networkID, obj);
                    }
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
