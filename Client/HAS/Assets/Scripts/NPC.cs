using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour {
    
    Animator npc_animator;
    NavMeshAgent move_agent;
    
    Vector3[] way_point = new Vector3[20];

    float npc_speed;

    public int way;

	// Use this for initialization
	void Start () {
        npc_animator = GetComponentInChildren<Animator>();
        npc_speed = 0f;

        move_agent = GetComponent<NavMeshAgent>();

        way = 0;

        way_point[0] = new Vector3(0f, 0f, 40f);
        way_point[1] = new Vector3(15f, 0f, 40f);
        way_point[2] = new Vector3(30f, 0f, 40f);
        way_point[3] = new Vector3(-15f, 0f, 40f);
        way_point[4] = new Vector3(-30f, 0f, 40f);
        way_point[5] = new Vector3(0f, 0f, -40f);
        way_point[6] = new Vector3(15f, 0f, -40f);
        way_point[7] = new Vector3(30f, 0f, -40f);
        way_point[8] = new Vector3(-15f, 0f, -40f);
        way_point[9] = new Vector3(-30f, 0f, -40f);

        way_point[10] = new Vector3(0f, 0f, 20f);
        way_point[11] = new Vector3(15f, 0f, 20f);
        way_point[12] = new Vector3(30f, 0f, 20f);
        way_point[13] = new Vector3(-15f, 0f, 20f);
        way_point[14] = new Vector3(-30f, 0f, 20f);
        way_point[15] = new Vector3(0f, 0f, -20f);
        way_point[16] = new Vector3(15f, 0f, -20f);
        way_point[17] = new Vector3(30f, 0f, -20f);
        way_point[18] = new Vector3(-15f, 0f, -20f);
        way_point[19] = new Vector3(-30f, 0f, -20f);
    }
	
	// Update is called once per frame
	void Update () {
        if(way == -1)
        {
            npc_speed = 0f;
            npc_animator.SetFloat("speed", npc_speed);
            //move_agent.Stop();
            move_agent.destination = transform.position;
            return;
        }else
        {
            //move_agent.Resume();
        }

        if (this.transform.position.x == way_point[way].x && this.transform.position.y == way_point[way].y)
        {
            npc_speed = 0f;
        }else
        {
            npc_speed = 2.0f;
        }

        npc_animator.SetFloat("speed", npc_speed);
        move_agent.destination = way_point[way];
    }
}
