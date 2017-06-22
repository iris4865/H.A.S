using HatchlingNet;
using Header;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//클라에게 제공하게될 인터페이스가 되겠네


public sealed class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance = null;
    private static GameObject container = null;
    private static readonly object padlock = new object();

    public static NetworkManager GetInstance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {

                    //   instance = tag(typeof(NetworkManager)) as NetworkManager;
                    //instance = new NetworkManager();                    출처: http://unityindepth.tistory.com/38 [UNITY IN DEPTH]

                    instance = GameObject.FindObjectOfType(typeof(NetworkManager)) as NetworkManager; //http://unityindepth.tistory.com/38
                                                                                                      //


                    //container = new GameObject();
                    //container.name = "NetworkManager";
                    //instance = container.AddComponent(typeof(NetworkManager)) as NetworkManager;


                    if (instance == null)
                        Debug.LogError("no active NetworkManager ");
                    else
                        Debug.Log("active NetworkManager ");

                }
            }

            return instance;
        }
    }

    private NetworkManager()
    {
    }

    HatchlingNetUnityService gameserver;
    public Dictionary<string, GameObject> networkObj;
    object csNetworkObj = new object();
    public Queue<string> numberingWaitObjTag { get; set; }
    object csNumberingWaitObj = new object();
    public int networkID { get; set; }
    public string userID { get; set; }

    public GameObject[] numberingNPC = new GameObject[20];

    void Awake()
    {
        //if (instance != null)
        //    return;
        DontDestroyOnLoad(this);
        this.gameserver = gameObject.AddComponent<HatchlingNetUnityService>();
        this.gameserver.callbackAppStatusChanged += CallStatusChange;
        this.gameserver.callbackAppReceiveMessage += CallMessage;
        networkObj = new Dictionary<string, GameObject>();
        numberingWaitObjTag = new Queue<string>();


        //Debug.Log("ㅇㅇ");
    }

    void Start()
    {
        //Debug.Log("ㅇㅇㅁ");
        //userID = "test" + Random.Range(1, 100);

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
                    //뭐여 이건
                    //Packet msg = PacketBufferManager.Instance.Pop((short)PROTOCOL.Chat);
                    //msg.Push("Hello~!");

                    //this.gameserver.Send(msg);
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
        PROTOCOL protocolType = msg.Protocol;

        //Debug.Log("콜메세지 " + protocolType);

        switch (protocolType)
        {
            case PROTOCOL.ChatAck:
                {
                    SEND_TYPE sendType = (SEND_TYPE)msg.PopInt16();
                    string text = msg.PopString();
                }
                break;

            case PROTOCOL.LoginAck:
                {
                    SceneManager.LoadScene(3);

                    Packet sendmsg = PacketBufferManager.Instance.Pop(PROTOCOL.JoinRoom);
                    sendmsg.Push(0);
                    
                    Send(sendmsg);
                }
                break;

            case PROTOCOL.LoginRej:
                {
                    //다시입력하라고
                }
                break;

            case PROTOCOL.JoinRoomRes:
                {
                    int current_user_count = msg.PopInt32();

                    GameObject waitdisplay = GameObject.Find("Wait");
                    game_wait_click wait_script = waitdisplay.GetComponent<game_wait_click>();
                    
                    wait_script.user_count = current_user_count;

                    if (current_user_count == 4)
                        SceneManager.LoadScene(4);
                }
                break;

            case PROTOCOL.GameStart:
                {
                    for(int i = 0; i < 1; i++)
                    {
                        int user_position = msg.PopInt32();
                        string msgUserLoginID = msg.PopString();
                        
                        GameObject objSpawner = GameObject.Find("Player_Spawn");
                        Player_Spawn componentSpawner = objSpawner.GetComponent<Player_Spawn>();

                        //캐릭터 생성
                        GameObject myPlayer = componentSpawner.CreateMyPlayer(1, user_position);
                        //myPlayer.GetComponent<Player5>().headCamera.SetActive(false); //디폴트를 비활성화 시킴 
                        NetworkObj objNetInfo = myPlayer.GetComponent<NetworkObj>();
                        objNetInfo.remoteId = msgUserLoginID;

                        lock (csNetworkObj)
                        {
                            networkObj.Add(objNetInfo.remoteId, myPlayer);
                            print("추가됨");
                        }

                        if (userID == objNetInfo.remoteId)
                        {
                            myPlayer.GetComponent<Player5>().isPlayer = true;
                            myPlayer.GetComponent<Player5>().main_camera.gameObject.SetActive(true);

                            GameObject winlose = GameObject.Find("WinLose");
                            winlose winlose_component = winlose.GetComponent<winlose>();

                            winlose_component.user_job = myPlayer.GetComponent<Player5>().player_job;
                        }
                    }
                    for (int i = 0; i < 1; i++)
                    {
                        int user_position = msg.PopInt32();
                        string msgUserLoginID = msg.PopString();
                        
                        GameObject objSpawner = GameObject.Find("Player_Spawn");
                        Player_Spawn componentSpawner = objSpawner.GetComponent<Player_Spawn>();

                        //캐릭터 생성
                        GameObject myPlayer = componentSpawner.CreateMyPlayer(2, user_position);
                        //myPlayer.GetComponent<Player5>().headCamera.SetActive(false); //디폴트를 비활성화 시킴 
                        NetworkObj objNetInfo = myPlayer.GetComponent<NetworkObj>();
                        objNetInfo.remoteId = msgUserLoginID;

                        lock (csNetworkObj)
                        {
                            networkObj.Add(objNetInfo.remoteId, myPlayer);
                            print("추가됨");
                        }

                        if (userID == objNetInfo.remoteId)
                        {
                            myPlayer.GetComponent<Player5>().isPlayer = true;
                            myPlayer.GetComponent<Player5>().main_camera.gameObject.SetActive(true);

                            GameObject winlose = GameObject.Find("WinLose");
                            winlose winlose_component = winlose.GetComponent<winlose>();

                            winlose_component.user_job = myPlayer.GetComponent<Player5>().player_job;
                        }
                    }

                    for(int i = 0; i < 3; i++)
                    {
                        int item_position = msg.PopInt32();

                        GameObject itemSpawner = GameObject.Find("Item_Spawn");
                        Item_Spawn componentSpawner = itemSpawner.GetComponent<Item_Spawn>();
                        
                        componentSpawner.item_create(item_position);
                    }
                    
                    for (int i = 0; i < 20; i++)
                    {
                        GameObject npcSpawner = GameObject.Find("NPC_Spawn");
                        NPC_Spawn componentSpawner = npcSpawner.GetComponent<NPC_Spawn>();

                        numberingNPC[i] = componentSpawner.create_npc(i);
                    }
                }
                break;

            case PROTOCOL.NPCPosition:
                {
                    for(int i = 0; i < 10; i++)
                    {
                        int position = msg.PopInt32();

                        numberingNPC[i].GetComponent<NPC>().way = position;
                        numberingNPC[i + 10].GetComponent<NPC>().way = position;
                    }
                }

                break;
                
            case PROTOCOL.PositionAck:
                {
                    string remoteID = msg.PopString();
                    lock (networkObj) //thread 안정적으로 사용하려고 
                    {
                        if (networkObj.ContainsKey(remoteID))
                        {
                            Player5 userPlayer = networkObj[remoteID].GetComponent<Player5>();

                            userPlayer.transform.position = msg.PopVector().Vector;
                            //Quaternion rot = Quaternion.identity;
                            //rot. = msg.PopVector().Vector;
                            Quaternion rotation = userPlayer.transform.rotation;
                            rotation.eulerAngles = msg.PopVector().Vector;
                            userPlayer.transform.rotation = rotation;
                            //userPlayer.transform.Rotate(msg.PopVector().Vector);

                            int count = msg.PopInt32();
                            for (int i = 0; i < count; i++)
                            {
                                KeyCode key = (KeyCode)msg.PopInt16();

                                bool press = msg.PopInt16() == 1;
                                userPlayer.inputEventKey[key] = press;
                            }
                            //userPlayer.mouseAxis = msg.PopFloat();
                        }
                    }
                }
                break;

            case PROTOCOL.PlayerExit:
                {
                    string remoteID = msg.PopString();
                    lock (csNetworkObj)
                    {
                        Destroy(networkObj[remoteID]);
                        networkObj.Remove(remoteID);
                    }
                }
                break;

            //case PROTOCOL.CreateObjAck:
            //    {
            //        int objNumbering = msg.PopInt32();
            //        string objTag = msg.PopString();
            //        Vector3 position; position.x = msg.PopFloat(); position.y = msg.PopFloat(); position.z = msg.PopFloat();

            //        lock (csNumberingWaitObj)
            //        {
            //            print(numberingWaitObj.Count);
            //            GameObject createObj = numberingWaitObj.Peek();
            //            numberingWaitObj.Clear();
            //            NetworkObj netInfoObj = createObj.GetComponent<NetworkObj>();
            //            netInfoObj.remoteId = networkID;

            //            lock (networkObj)
            //            {
            //                networkObj.Add(networkID, createObj);
            //                createObj.SetActive(true);
            //            }
            //        }



            //    }
            //    break;

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
