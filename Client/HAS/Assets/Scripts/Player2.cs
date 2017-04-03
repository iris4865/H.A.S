using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour {

    float speed = 5f;
    float h;
    float v;
    float rotateSpeed = 10f;

    Rigidbody rigdbody;
    Animator ani;

    Vector3 movement;

	// Use this for initialization
	void Awake () {
        rigdbody = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
	}
	
    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        AnimationUpdate();
    }

	// Update is called once per frame
	void FixedUpdate () {
        run(h, v);
        turn();
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

    void turn()
    {
        if(h == 0 && v == 0)
        {
            return;
        }

        Quaternion newRotation = Quaternion.LookRotation(movement);

        rigdbody.rotation = Quaternion.Slerp(rigdbody.rotation, newRotation, rotateSpeed * Time.deltaTime);
    }
}
