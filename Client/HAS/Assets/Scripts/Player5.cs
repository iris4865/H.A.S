using HatchlingNet;
using Header;
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

    //경찰인지 도둑인지 구별...해야한다.
    int player_job = 1; //1 or 2 = 도둑 or 경찰...

    public GameObject pressE_key_canvas;

    public float player_speed;

    public ANIMATION_TYPE currentAnimation = ANIMATION_TYPE.Wait;

    public Camera main_camera;

    public AudioClip attack_sound;
    public AudioClip walk_sound;
    //public AudioClip sound_3;
    private AudioSource audio;

    int inputKeyCount;
    bool isMove;
    KeyCode inputKey;

    void Awake()
    {
        player_animator = GetComponentInChildren<Animator>();
        pressE_key_canvas.SetActive(false);
        player_speed = 2.0f;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        AnimationUpdate();

        if (isPlayer)
        {
            Run();
            Turn();
            if (player_job == 1)
                Action();

            if (isMove)
                NetUpdate();
        }
    }

    void NetUpdate()
    {
        Packet msg = PacketBufferManager.Instance.Pop(PROTOCOL.Position);
        msg.Push((short)inputKey);
        msg.Push();
        msg.Push(transform.position.x, transform.position.y, transform.position.z);

        MyVector3 vec;
        vec.x = transform.rotation.x; vec.y = transform.rotation.y; vec.z = transform.rotation.z;
        msg.Push(vec);
        msg.Push(player_speed);
        msg.Push((short)currentAnimation);
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
        switch (currentAnimation)
        {
            case ANIMATION_TYPE.Wait:
                player_animator.SetBool("isforwarding", false);
                player_animator.SetBool("isRunning", false);
                player_animator.SetFloat("speed", player_speed);
                break;
            case ANIMATION_TYPE.Walk:
                player_animator.SetBool("isforwarding", true);
                player_animator.SetBool("isRunning", false);
                player_animator.SetFloat("speed", player_speed);
                break;
            case ANIMATION_TYPE.Run:
                player_animator.SetBool("isforwarding", false);
                player_animator.SetBool("isRunning", true);
                player_animator.SetFloat("speed", player_speed);
                break;
            case ANIMATION_TYPE.Attack:
                player_animator.SetBool("isaction", true);
                break;
        }
    }

    void Run()
    {
        player_moveVector = Vector3.zero;
        currentAnimation = ANIMATION_TYPE.Wait;
        isMove = false;

        if (Input.GetKey(KeyCode.W) == true)
        {
            player_moveVector.z = player_speed;
            isMove = true;
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            player_moveVector.z = -player_speed;
            isMove = true;
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

        if (isMove)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                player_speed = 8.0f;
                currentAnimation = ANIMATION_TYPE.Run;
            }
            else
            {
                player_speed = 2.0f;
                currentAnimation = ANIMATION_TYPE.Walk;
            }
        }
        transform.position += ((transform.rotation * player_moveVector) * Time.deltaTime);
    }

    void Turn()
    {
        player_rotateVector = new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.Rotate(player_rotateVector * 2.0f);
    }

    void Action()
    {
        Collider collider = foot.GetComponent<SphereCollider>();

        if (Input.GetKey(KeyCode.E))
        {
            this.audio.clip = this.attack_sound;
            audio.Play();
            collider.isTrigger = true;
            currentAnimation = ANIMATION_TYPE.Attack;
        }
        else
        {
            //collider.isTrigger = false;
            //player_animator.SetBool("isaction", false);
        }
    }
}
