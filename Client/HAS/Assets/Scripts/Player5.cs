﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player5 : MonoBehaviour
{
    Animator ani;
    Vector3 addPosition;
    Vector3 V3;

    public GameObject can;

    float speed = 2.0f;

    // Use this for initialization
    void Awake()
    {
        ani = GetComponentInChildren<Animator>();
        can.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
    }

    void FixedUpdate()
    {
        run();
        turn();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("1");
        /*if(other.gameObject.name == "item")
        {
            Debug.Log("2");
            can.SetActive(true);
        }*/
        if (other.gameObject.tag == "item1")
        {
            Debug.Log("2");
            can.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        can.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKey(KeyCode.E) == true)
        {
            Destroy(other.gameObject);
            can.SetActive(false);
        }
    }

    void AnimationUpdate()
    {
        if (addPosition.x == 0 && addPosition.z == 0)
        {
            ani.SetBool("isforwarding", false);
            ani.SetBool("isRunning", false);
        }
        else
        {
            ani.SetBool("isforwarding", true);
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
                s = 3.0f;
            }
            if (Input.GetKey(KeyCode.W) == true)
            {
                addPosition.z = speed * s;
                ani.SetBool("isRunning", true);
            }
            if (Input.GetKey(KeyCode.S) == true)
            {
                addPosition.z = -speed;
                ani.SetBool("isRunning", false);
            }
            if (Input.GetKey(KeyCode.A) == true)
            {
                transform.Rotate(Vector3.down * Time.deltaTime * 100f);
            }
            if (Input.GetKey(KeyCode.D) == true)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * 100f);
            }
            ani.SetFloat("speed", speed * s);
        }
        transform.position += ((transform.rotation * addPosition) * Time.deltaTime);
    }

    void turn()
    {
        V3 = new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.Rotate(V3 * speed);
    }
}