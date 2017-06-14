using HatchlingNet;
using Header;
using System.Collections.Generic;
using UnityEngine;

public class Player_Spawn : MonoBehaviour {
    public GameObject player;
    //GameObject player1 = GameObject.Find("player");
    //GameObject[] players = GameObject.FindGameObjectsWithTag("player");
    //GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
    Queue<string> numberingWaitObjTag = null;

    int nonPlayerCount = 4;
    private List<Player5> playerList = new List<Player5>();
    
    float range = 10.0f;
    //생성자
    // Use this for initialization
    void Start()
    {
        numberingWaitObjTag = NetworkManager.GetInstance.numberingWaitObjTag; 
        numberingWaitObjTag.Enqueue(player.tag);
        Vector3 createPosition = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));

        Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ObjNumberingReq, (short)SEND_TYPE.Single);
        msg.Push(player.tag);
        msg.Push(createPosition.x, createPosition.y, createPosition.z);

        NetworkManager.GetInstance.Send(msg);

        //for (int i = 0; i < nonPlayerCount + 1; ++i)
        //{
        //    numberingWaitObjTag.Enqueue(player.tag);
        //    Vector3 createPosition = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));

        //    Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ObjNumberingReq, (short)SEND_TYPE.Single);
        //    msg.Push(player.tag);
        //    msg.Push(createPosition.x, createPosition.y, createPosition.z);

        //    NetworkManager.GetInstance.Send(msg);
        //}

        //int myPlayerIndex = Random.Range(0, nonPlayerCount + 1);




        //        GameObject myPlayer = CreateMyPlayer();
        //        print("밖에서 : " + NetworkManager.GetInstance.numberingWaitObj.Count);

        //        myPlayer.SetActive(false);
        //        myPlayer.GetComponent<Player5>().headCamera.SetActive(true);




        //        //for (int i = 0; i < nonPlayerCount; ++i)
        //        //{
        //        //    GameObject remotePlayer = CreateRemotePlayer();
        //        //    remotePlayer.SetActive(false);

        //        //    msg = PacketBufferManager.Pop((short)PROTOCOL.ObjNumberingReq, (short)SEND_TYPE.Single);
        //        //    msg.Push(remotePlayer.tag);
        //        //    queue.Enqueue(remotePlayer);
        //        //    NetworkManager.GetInstance.Send(msg);
        //        //}
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject CreateMyPlayer(Vector3 position)
    {
//        Vector3 position = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
        string objTag = player.tag;

        GameObject myPlayer = Instantiate(player, position, transform.rotation);
        //myPlayer.GetComponent<Player5>().headCamera.SetActive(true); //디폴트를 비활성화 시킴 
//                                                                      //NetworkManager.GetInstance.numberingWaitObj.Enqueue(myPlayer);
//                                                                      //  print("짐" + NetworkManager.GetInstance.numberingWaitObj.Count);


        
//        NetworkManager.GetInstance.numberingWaitObj.Enqueue(Instantiate(player, position, transform.rotation));
//        print("안에서 : " + NetworkManager.GetInstance.numberingWaitObj.Count);


////        GameObject myPlayer = new GameObject();
        return myPlayer;
    }


    //이건 뭥미?
    GameObject CreateRemotePlayer()
    {
        Vector3 position = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
        GameObject remotePlayer = Instantiate(player, position, transform.rotation);
//        remotePlayer.GetComponent<Player5>().headCamera.SetActive(false);

        return remotePlayer;
    }

    void CreatePlayer()
    {
 //       player.GetComponent<Player5>().main_camera.enabled = false;
        //GameObject gb = GameObject.Find("Player").gameObject;
        //playerList.Add(gb.GetComponent<Player5>());

        //sc.ControlInstanceId = gb.GetInstanceID();
        for (int i = 0; i < nonPlayerCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
            
            GameObject copy = Instantiate(player, position, transform.rotation);
            //playerList.Add(copy.GetComponent<Player5>());
            //copy.GetComponent<Player5>().main_camera.enabled = false;
        }

  //      player.GetComponent<Player5>().main_camera.enabled = true;
    }

    void RemovePlayer()
    {

    }
}
