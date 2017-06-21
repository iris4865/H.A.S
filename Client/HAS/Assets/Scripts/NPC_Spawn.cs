using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawn : MonoBehaviour {

    public GameObject npc;

    public GameObject[] point;

    public GameObject[] npc_numbering;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void create_npc()
    {
        for(int i = 0; i < 20; i++)
        {
            npc_numbering[i] = Instantiate(npc, point[i].transform.position, transform.rotation);
        }
    }
}
