using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float Move_z_front;
    public float Move_z_back;
    public float Move_x_front;
    public float Move_x_back;

    public Animator ani;

    public Camera camera_object;

    // Use this for initialization
    void Start () {
        ani = GetComponentInChildren<Animator>();
        camera_object = GetComponentInChildren<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        move();
    }

    void move()
    {
        /*{
            float speed = 90.0f;

            if (Input.GetKey(KeyCode.A) == true)
            {
                transform.Rotate(Vector3.down * Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.D) == true)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * speed);
            }
            //transform.Rotate(Input.GetAxis("Mouse X") * speed);
            
        }*/

        Vector3 addPosition = Vector3.zero;
        {
            float s = 1.0f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //addPosition.z = 10;
                s = 2.5f;
            }
            if (Input.GetKey(KeyCode.W) == true)
            {
                addPosition.z = Move_z_front * s;
            }
            if (Input.GetKey(KeyCode.S) == true)
            {
                addPosition.z = Move_z_back * s;
            }
            if (Input.GetKey(KeyCode.A) == true)
            {
                addPosition.x = Move_x_front * s;
            }
            if (Input.GetKey(KeyCode.D) == true)
            {
                addPosition.x = Move_x_back * s;
            }
            if (Input.GetKeyDown(KeyCode.S) == true)
            {
                //Quaternion?
            }
            if (Input.GetKeyDown(KeyCode.A) == true)
            {

            }
            if (Input.GetKeyDown(KeyCode.D) == true)
            {

            }
        }
        
        transform.position += ((transform.rotation * addPosition) * Time.deltaTime);
        transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * 5.0f);
        //camera_object.transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * 5.0f);
        //카메라 Y축 회전...
        //camera_object.transform.RotateAround(transform.position, Vector3.right, Input.GetAxis("Mouse Y") * 5.0f);
        {
            ani.SetFloat("speed", addPosition.z);
            ani.SetFloat("speed_r", addPosition.z);
            ani.SetFloat("speed_x", addPosition.x);
            ani.SetFloat("speed_x_r", addPosition.x);
        }
    }
}
