using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawn : MonoBehaviour {

    public GameObject npc;

    GameObject npc_p;

    public GameObject[] point;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject create_npc(int position)
    {
        npc_p = Instantiate(npc, point[position].transform.position, transform.rotation);

        return npc_p;
    }
}
