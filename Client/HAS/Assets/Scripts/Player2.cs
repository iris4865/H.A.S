using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour {

    public float speed = 10f;
    float h;
    float v;

    Rigidbody rigdbody;
    Animator ani;

    Vector3 movement;

    NetworkManager networkManager;

	// Use this for initialization
	void Awake () {
        rigdbody = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
        networkManager = GameObject.FindWithTag("eNetworkManager").GetComponent<NetworkManager>() ;


    }
	
	// Update is called once per frame
	void FixedUpdate () {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        run(h, v);

        NetUpdate();
    }

    void AnimationUpdate()
    {
        if(h == 0 && v == 0)
        {
            ani.SetBool("isRunning", false);
        }
        else
        {
            ani.SetBool("isRunning", true);
        }
    }

    void run(float h, float v)
    {
        movement.Set(h, 0, v);
        movement = movement.normalized * speed * Time.deltaTime;

        rigdbody.MovePosition(transform.position + movement);
    }

    void NetUpdate()
    {
        //Vector3 position = transform.position;

        //Packet msg = PacketBufferManager.Pop((short)PROTOCOL.ChatReq, (short)SEND_TYPE.BroadcastWithMe);
        //msg.Push(position.x, position.y, position.z);

        //networkManager.Send(msg);
    }
}
