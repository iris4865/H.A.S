using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player4 : MonoBehaviour {

    public Camera cam;

    Animator ani;
    Vector3 addPosition;

    float speed = 4.0f;

    // Use this for initialization
    void Awake() {
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
        if (addPosition.x == 0 && addPosition.z == 0)
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
        //float m_rotation_x = 0f;
        m_rotation_y += (Input.GetAxis("Mouse X") * 10.0f);
        //if(cam.transform.rotation.x > -0.4 && cam.transform.rotation.x < 0.4){
        //    m_rotation_x += (Input.GetAxis("Mouse Y") * 5.0f);
        //}
        
        transform.Rotate(0, m_rotation_y, 0f, 0);
        //cam.transform.Rotate(-m_rotation_x, 0f, 0f, 0);
    }
}
