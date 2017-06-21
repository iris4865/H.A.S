using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour {

    public GameObject[] way_point;

    Animator npc_animator;
    NavMeshAgent move_agent;

    float npc_speed;

    int way;

	// Use this for initialization
	void Start () {
        npc_animator = GetComponentInChildren<Animator>();
        npc_speed = 2.0f;
        

        move_agent = GetComponent<NavMeshAgent>();

        way = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (this.transform.position.x == way_point[way].transform.position.x && this.transform.position.y == way_point[way].transform.position.y)
        {
            npc_speed = 0f;
        }else
        {
            npc_speed = 2.0f;
        }

        npc_animator.SetFloat("speed", npc_speed);
        move_agent.destination = way_point[way].transform.position;
    }
}
