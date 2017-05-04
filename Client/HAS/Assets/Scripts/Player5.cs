using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player5 : MonoBehaviour
{
    Animator ani;
    Vector3 addPosition;
    Vector3 V3;

    //경찰인지 도둑인지 구별...해야한다.
    int player_job; //1 or 2 = 도둑 or 경찰...
    
    public GameObject can;

    float speed = 2.0f;
    //int player_number;

    // Use this for initialization
    void Awake()
    {
        ani = GetComponentInChildren<Animator>();
        can.SetActive(false);
    }

    void Start()
    {
        //Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ObjNumberingReq, (short)SEND_TYPE.Single);
        //msg.Push(this.tag);
        //Queue<GameObject> queue = NetworkManager.GetInstance.numberingWaitObj;
        //queue.Enqueue(this.gameObject);
        //NetworkManager.GetInstance.Send(msg);
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();

        //NetUpdate();
    }

    void FixedUpdate()
    {
        run();
        turn();
    }

    void NetUpdate()
    {
        Packet msg = PacketBufferManager.Pop((short)PROTOCOL.PositionReq, (short)SEND_TYPE.BroadcastWithoutMe);
        //        msg.Push(NetworkManager.GetInstance.networkID);//id...나중에가면 유저id가 아니라 각 객체마다 서버에서 id를 할당해주고 그걸 기준으로 객체의 정보 통신...
        //하나의 객체에 여러 상호작용이 일어날수 있으니 나중에 해당 메시지를 보낸 시간도 추가해야할것 같다.
        msg.Push(NetworkManager.GetInstance.networkID);
        msg.Push(transform.position.x, transform.position.y, transform.position.z);
        NetworkManager.GetInstance.Send(msg);

    }

    private void OnTriggerEnter(Collider other)
    {
        /*if(other.gameObject.name == "item")
        {
            Debug.Log("2");
            can.SetActive(true);
        }*/
        if (other.gameObject.tag == "item1")
        {
            can.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        can.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKey(KeyCode.E) == true)
        {
            Destroy(other.gameObject);
            can.SetActive(false);
        }
    }

    void AnimationUpdate()
    {
        if (addPosition.x == 0 && addPosition.z == 0)
        {
            ani.SetBool("isforwarding", false);
            ani.SetBool("isRunning", false);
        }
        else
        {
            ani.SetBool("isforwarding", true);
        }
    }

    void run()
    {
        addPosition = Vector3.zero;
        {
            float s = 1.0f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //addPosition.z = 10;
                s = 3.0f;
            }
            if (Input.GetKey(KeyCode.W) == true)
            {
                addPosition.z = speed * s;
                ani.SetBool("isRunning", true);
            }
            if (Input.GetKey(KeyCode.S) == true)
            {
                addPosition.z = -speed;
                ani.SetBool("isRunning", false);
            }
            if (Input.GetKey(KeyCode.A) == true)
            {
                transform.Rotate(Vector3.down * Time.deltaTime * 100f);
            }
            if (Input.GetKey(KeyCode.D) == true)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * 100f);
            }
            if (Input.GetKey(KeyCode.E) == true)
            {
                
            }
            ani.SetFloat("speed", speed * s);
        }
        transform.position += ((transform.rotation * addPosition) * Time.deltaTime);
    }

    void turn()
    {
        V3 = new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.Rotate(V3 * speed);
    }
}
