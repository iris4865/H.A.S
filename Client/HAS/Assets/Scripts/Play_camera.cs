using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play_camera : MonoBehaviour {
    
    Vector3 camera_rotateVector;
    float camera_x;
    float camera_speed;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        
        camera_speed = 2.0f;
    }

    void Update()
    {
        
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        turn();
    }

    void turn()
    {
        camera_x = Input.GetAxis("Mouse Y");
        if (transform.rotation.eulerAngles.x - camera_x <= 40f || transform.rotation.eulerAngles.x - camera_x >= 320f)
        {
            camera_rotateVector = new Vector3(-camera_x, 0, 0);
            transform.Rotate(camera_rotateVector * camera_speed);
        }
    }
}
