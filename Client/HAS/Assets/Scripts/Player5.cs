using HatchlingNet;
using UnityEngine;

public class Player5 : MonoBehaviour
{
    public GameObject foot;
    Animator player_animator;
    Vector3 player_moveVector;
    Vector3 player_rotateVector;

    public int UniqueId { get; set; }
    public bool isPlayer = false;
    public static int count = 0;

    bool caninput {get; set;}

    //경찰인지 도둑인지 구별...해야한다.
    int player_job  = 1; //1 or 2 = 도둑 or 경찰...

    public GameObject pressE_key_canvas;
    
    public float player_speed;

    public int animation_type = (short)ANIMATION_TYPE.Wait;

    public Camera main_camera;

    public AudioClip attack_sound;
    public AudioClip walk_sound;
    //public AudioClip sound_3;
    private AudioSource audio;

    void Awake()
    {
        player_animator = GetComponentInChildren<Animator>();
        pressE_key_canvas.SetActive(false);
        player_speed = 2.0f;
        caninput = true;
    }

    void Start()
    {

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
            if (caninput)
            {
                run();
                turn();
                if (player_job == 1)
                {
                    action();
                }
            }
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
        msg.Push(transform.rotation.y);
        msg.Push(player_speed);
        msg.Push(animation_type);
        NetworkManager.GetInstance.Send(msg);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item1")
        {
            if (player_job == 2)//도둑
            {
                pressE_key_canvas.SetActive(true);
            }
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
            if (player_job == 2)//도둑
            {
                Destroy(other.gameObject);
                pressE_key_canvas.SetActive(false);
            }
        }
    }

    void AnimationUpdate()
    {
        /*if (player_moveVector.x == 0 && player_moveVector.z == 0)
        {
            player_animator.SetBool("isforwarding", false);
            player_animator.SetBool("isRunning", false);
        }
        else
        {
            player_animator.SetBool("isforwarding", true);
        }*/

        if(animation_type == (short)ANIMATION_TYPE.Wait)
        {
            player_animator.SetBool("isforwarding", false);
            player_animator.SetBool("isRunning", false);
            player_animator.SetFloat("speed", player_speed);
        }
        else if(animation_type == (short)ANIMATION_TYPE.Walk)
        {
            player_animator.SetBool("isforwarding", true);
            player_animator.SetBool("isRunning", false);
            player_animator.SetFloat("speed", player_speed);
        }
        else if(animation_type == (short)ANIMATION_TYPE.Run)
        {
            //
            player_animator.SetBool("isforwarding", false);
            player_animator.SetBool("isRunning", true);
            player_animator.SetFloat("speed", player_speed);
        }
        else if(animation_type == (short)ANIMATION_TYPE.Attack)
        {
            player_animator.SetBool("isaction", true);
        }
    }

    void run()
    {
        player_moveVector = Vector3.zero;
        {
            animation_type = (short)ANIMATION_TYPE.Wait;
            bool isMove = false;
            if (Input.GetKey(KeyCode.W) == true)
            {
                player_moveVector.z = player_speed;
                isMove = true;
                //animation_type = (short)ANIMATION_TYPE.Walk;
            }
            if (Input.GetKey(KeyCode.S) == true)
            {
                player_moveVector.z = -player_speed;
                //player_animator.SetBool("isRunning", false);
                isMove = true;
                //animation_type = (short)ANIMATION_TYPE.Walk;
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
            //player_animator.SetFloat("speed", player_speed * s);
            
            if (isMove)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //addPosition.z = 10;
                    //player_animator.SetBool("isRunning", true);
                    player_speed = 8.0f;
                    animation_type = (short)ANIMATION_TYPE.Run;
                }
                else
                {
                    player_speed = 2.0f;
                    animation_type = (short)ANIMATION_TYPE.Walk;
                }
            }
            
        }
        transform.position += ((transform.rotation * player_moveVector) * Time.deltaTime);
    }

    void turn()
    {
        player_rotateVector = new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.Rotate(player_rotateVector * 2.0f);
    }

    void action()
    {
        Collider collider = foot.GetComponent<SphereCollider>();

        if (Input.GetKey(KeyCode.E))
        {
            this.audio.clip = this.attack_sound;
            audio.Play();
            collider.isTrigger = true;
            animation_type = (short)ANIMATION_TYPE.Attack;
        }
        else
        {
            //collider.isTrigger = false;
            //player_animator.SetBool("isaction", false);
        }
    }
}
