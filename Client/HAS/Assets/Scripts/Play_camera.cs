using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play_camera : MonoBehaviour {
    
    Vector3 V3;
    float X;
    float speed;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        
        speed = 2.0f;
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
        X = Input.GetAxis("Mouse Y");
        print("1"+X);
        if (transform.rotation.eulerAngles.x - X <= 40f || transform.rotation.eulerAngles.x - X >= 320f)
        {
            V3 = new Vector3(-X, 0, 0);
            transform.Rotate(V3 * speed);
            print("2" + (transform.rotation.eulerAngles.x - X));
        }
    }
}
