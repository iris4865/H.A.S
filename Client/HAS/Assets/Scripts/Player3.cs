using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3 : MonoBehaviour {

    public GameObject player;
    Camera player_camera;
    Animator ani;
    Vector3 addPosition;

    float speed = 4.0f;
    //float m_rotation_y = 0f;

    // Use this for initialization
    void Awake() {
        //player = GetComponent<GameObject>();
        player_camera = GetComponentInChildren<Camera>();
        ani = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        AnimationUpdate();

    }

    void FixedUpdate()
    {
        run();
        turn();
    }

    void AnimationUpdate()
    {
        if(addPosition.x == 0 && addPosition.z == 0)
        {
            ani.SetBool("isRunning", false);
        }
        else
        {
            ani.SetBool("isRunning", true);
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
                s = 2.5f;
            }
            if (Input.GetKey(KeyCode.W) == true)
            {
                addPosition.z = speed * s;

            }
            if (Input.GetKey(KeyCode.S) == true)
            {
                addPosition.z = -speed * s;
                player.transform.Rotate(0,180f,0,0);
            }
            if (Input.GetKey(KeyCode.A) == true)
            {
                addPosition.x = -speed * s;
            }
            if (Input.GetKey(KeyCode.D) == true)
            {
                addPosition.x = speed * s;
            }
        }
        transform.position += ((transform.rotation * addPosition) * Time.deltaTime);

    }

    void turn()
    {
        float m_rotation_y = 0f;
        m_rotation_y += (Input.GetAxis("Mouse X") * 10.0f);
        transform.Rotate(0f, m_rotation_y, 0f, 0);


        //Quaternion newRotation = Quaternion.LookRotation(addPosition);
        //player.transform.rotation = Quaternion.Slerp(player.transform.rotation, newRotation, 5.0f * Time.deltaTime);


        //player.transform.rotation = Quaternion.Euler(0f, m_rotation_y, 0f);

        //transform.position = Vector3.zero;
        //transform.rotation += ((player_camera.transform.rotation * addPosition) * Time.deltaTime);

        //player_camera.transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * 5.0f);
    }
}
