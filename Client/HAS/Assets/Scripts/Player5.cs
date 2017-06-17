﻿using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player5 : MonoBehaviour
{
    public GameObject foot;
    Animator player_animator;
    Vector3 player_moveVector;
    Vector3 player_rotateVector;

    public int UniqueId { get; set; }
    public bool isPlayer;
    public static int count;

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

    public Dictionary<KeyCode, bool> inputEventKey = new Dictionary<KeyCode, bool>();
    public float mouseAxis;
    bool hasEvent;
    float prevAxis;

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
            InputPlayerEvent();
            Action();
            NetUpdate();
        }
        else
            Action();
    }

    public void AnimationUpdate()
    {
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

    void InputPlayerEvent()
    {
        hasEvent = false;
        prevAxis = mouseAxis;

        IsEventKey(KeyCode.W);
        IsEventKey(KeyCode.S);
        IsEventKey(KeyCode.A);
        IsEventKey(KeyCode.D);
        IsEventKey(KeyCode.E);
        IsEventKey(KeyCode.LeftShift);

        mouseAxis = Input.GetAxis("Mouse X");
    }

    void IsEventKey(KeyCode code)
    {
        bool pressKey = Input.GetKey(code);
        if (inputEventKey.ContainsKey(code))
        {
            if (inputEventKey[code] != pressKey)
            {
                inputEventKey[code] = pressKey;
                hasEvent = true;
            }
        }
        else
            inputEventKey[code] = pressKey;
    }

    public void Action()
    {
        Move();
        Turn();
        if (player_job == 1)
            Motion();
    }

    void Move()
    {
        player_speed = 2.0f;
        player_moveVector = Vector3.zero;
        currentAnimation = ANIMATION_TYPE.Wait;

        if (inputEventKey[KeyCode.W])
        {
            if (inputEventKey[KeyCode.LeftShift])
            {
                currentAnimation = ANIMATION_TYPE.Run;
                player_speed = 8.0f;
            }
            else
                currentAnimation = ANIMATION_TYPE.Walk;
            player_moveVector.z = player_speed;
        }
        if (inputEventKey[KeyCode.S])
        {
            currentAnimation = ANIMATION_TYPE.Walk;
            player_moveVector.z = -player_speed;
        }
        if (inputEventKey[KeyCode.A])
        {
            transform.Rotate(Vector3.down * Time.deltaTime * 100f);
        }
        if (inputEventKey[KeyCode.D])
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 100f);
        }

        transform.position += ((transform.rotation * player_moveVector) * Time.deltaTime);
    }

    void Turn()
    {
        player_rotateVector = new Vector3(0, mouseAxis, 0);
        transform.Rotate(player_rotateVector * 2.0f);
    }

    void Motion()
    {
        Collider footCollider = foot.GetComponent<SphereCollider>();

        if (inputEventKey[KeyCode.E])
        {
            this.audio.clip = this.attack_sound;
            audio.Play();
            footCollider.isTrigger = true;
            currentAnimation = ANIMATION_TYPE.Attack;
        }
        else
        {
            //collider.isTrigger = false;
            //player_animator.SetBool("isaction", false);
        }
    }

    void NetUpdate()
    {
        if (hasEvent)
            Send();
        else if (!Equals(mouseAxis, prevAxis))
            Send();
    }

    void Send()
    {

        Packet msg = PacketBufferManager.Instance.Pop(PROTOCOL.Position);
        msg.Push(GetComponent<NetworkObj>().remoteId);

        /*
        MyVector3 position = new MyVector3();
        position.Vector = transform.position;
        msg.Push(position);
        */

        msg.Push(inputEventKey.Count);
        foreach (var playerEvent in inputEventKey)
        {
            msg.Push((short)playerEvent.Key);
            //true -> 1, false -> 0
            short value = (short)(playerEvent.Value ? 1 : 0);
            msg.Push(value);
        }
        msg.Push(mouseAxis);

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
}
