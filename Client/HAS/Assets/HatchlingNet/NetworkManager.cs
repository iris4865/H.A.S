﻿using HatchlingNet;
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
    public Dictionary<int, GameObject> networkObj;
    object csNetworkObj = new object();
    public Queue<string> numberingWaitObjTag {get; set;}
    object csNumberingWaitObj = new object();
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
        numberingWaitObjTag = new Queue<string>();


        Debug.Log("ㅇㅇ");
    }

    void Start()
    {
        Debug.Log("ㅇㅇㅁ");
        userID = "test" + Random.Range(1, 100);

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
                    string numberingTag;

                    lock (csNumberingWaitObj)
                    {
                        numberingTag = msg.PopString();//태그...큐에있는거랑 일치하나 검사 해줘야할것같긴 한디
//                        numberingTag = numberingWaitObjTag.Dequeue();
                    }

                    print("태그 : " + numberingTag);

                    Vector3 position; position.x = msg.PopFloat(); position.y = msg.PopFloat(); position.z = msg.PopFloat();
                    print("벡터에 담긴거 x : " + position.x + " y : " + position.y + " z : " + position.z);
                    int remoteID = msg.PopInt32();
                    print("remoteID : " + remoteID);

                    string msgUserID = msg.PopString();

                    if (msgUserID == userID)
                    {
                        print("msgID : " + msgUserID + ", userID : " + userID);
                        numberingWaitObjTag.Dequeue();
                    }


                    GameObject objSpawner = GameObject.Find("Player_Spawn");
                    Player_Spawn componentSpawner = objSpawner.GetComponent<Player_Spawn>();

                    print("너버링태그 : " + numberingTag);
                    switch (numberingTag)
                    {
                        case "Identify":
                            print("플레안까지옴");
                            GameObject myPlayer = componentSpawner.CreateMyPlayer(position);
                            //                            myPlayer.GetComponent<Player5>().headCamera.SetActive(false); //디폴트를 비활성화 시킴 
                            NetworkObj objNetInfo = myPlayer.GetComponent<NetworkObj>();
                            objNetInfo.remoteId = remoteID;

                            lock (csNetworkObj)
                            {
                                networkObj.Add(objNetInfo.remoteId, myPlayer);
                                print("추가됨");
                            }

                            if (this.userID == msgUserID)
                            {
                                myPlayer.GetComponent<Player5>().isPlayer = true;
                                myPlayer.GetComponent<Player5>().main_camera.gameObject.SetActive(true);
                                //npc나 remotePlayer의 경우에는 각각 별도의 스크립트를 만들어서 붙여주는게 낫지 않나?
                                //그래야 로직 분리도 되고...
                            }

                            break;
 
                    }



                    //PacketBufferManager.Push(msg);
                    //msg = PacketBufferManager.Pop((short)PROTOCOL.CreateObjReq, (short)SEND_TYPE.Single);
                    //msg.Push(networkID);
                    //msg.Push(obj.tag);//tag는 유니티 내에서 각 객체에 설정된거임...
                    //                  //자세한건 유니티 실행 후  윈도우탭/inspector탭/  을 참고
                    //msg.Push(transform.position.x, transform.position.y, transform.position.z);

                    //Send(msg);

                    
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
