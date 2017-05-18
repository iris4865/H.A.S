using HatchlingNet;
using UnityEngine;

public class Player5 : MonoBehaviour
{
    Animator player_animator;
    Vector3 player_moveVector;
    Vector3 player_rotateVector;

    public int UniqueId { get; set; }
    public bool isPlayer = false;
    public static int count = 0;

    //경찰인지 도둑인지 구별...해야한다.
    int player_job; //1 or 2 = 도둑 or 경찰...

    public GameObject pressE_key_canvas;

    float player_speed = 2.0f;

    public Camera main_camera;

    void Awake()
    {
        player_animator = GetComponentInChildren<Animator>();
        pressE_key_canvas.SetActive(false);
    }

    void Start()
    {
//        if (count == 0)
//        {
////            main_camera.enabled = true;
//            isPlayer = true;
//        }
        
//        UniqueId = count++;

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

        if (isPlayer)
        {
            NetUpdate();
        }
    }

    void FixedUpdate()
    {
        if (isPlayer)
        {
            run();
            turn();
        }
    }

    void NetUpdate()
    {
        Packet msg = PacketBufferManager.Pop((short)PROTOCOL.PositionReq, (short)SEND_TYPE.BroadcastWithoutMe);
        //        msg.Push(NetworkManager.GetInstance.networkID);//id...나중에가면 유저id가 아니라 각 객체마다 서버에서 id를 할당해주고 그걸 기준으로 객체의 정보 통신...
        //하나의 객체에 여러 상호작용이 일어날수 있으니 나중에 해당 메시지를 보낸 시간도 추가해야할것 같다.

        int remoteid = GetComponent<NetworkObj>().remoteId;
        Debug.Log("플레안속 네트워크오브젝트 수 : " + NetworkManager.GetInstance.networkObj.Count);
        Debug.Log("나 " + remoteid + "위치전송: " );
        msg.Push(remoteid);
        msg.Push(transform.position.x, transform.position.y, transform.position.z);
        NetworkManager.GetInstance.Send(msg);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item1")
        {
            pressE_key_canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pressE_key_canvas.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.E) == true)
        {
            Destroy(other.gameObject);
            pressE_key_canvas.SetActive(false);
        }
    }

    void AnimationUpdate()
    {
        if (player_moveVector.x == 0 && player_moveVector.z == 0)
        {
            player_animator.SetBool("isforwarding", false);
            player_animator.SetBool("isRunning", false);
        }
        else
        {
            player_animator.SetBool("isforwarding", true);
        }
    }

    void run()
    {
        player_moveVector = Vector3.zero;
        {
            float s = 1.0f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //addPosition.z = 10;
                s = 3.0f;
            }
            if (Input.GetKey(KeyCode.W) == true)
            {
                player_moveVector.z = player_speed * s;
                player_animator.SetBool("isRunning", true);
            }
            if (Input.GetKey(KeyCode.S) == true)
            {
                player_moveVector.z = -player_speed;
                player_animator.SetBool("isRunning", false);
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
            player_animator.SetFloat("speed", player_speed * s);
        }
        transform.position += ((transform.rotation * player_moveVector) * Time.deltaTime);
    }

    void turn()
    {
        player_rotateVector = new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.Rotate(player_rotateVector * player_speed);
    }
}
