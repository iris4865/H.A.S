using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    //public GameObject player = new GameObject();



    int size = 10;
    public GameObject[] spawn = new GameObject[10]; 
    Vector3 posi;
    float range = 30.0f;

    // Use this for initialization
    void Start () {
        //player_create();
        item_create();
    }

    void Awake(){

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /*void player_create()
    {
        int i;
        for(i = 0; i < 4; i++)
        {
            posi = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));

            Instantiate(player, posi, transform.rotation);
        }
    }*/

    void item_create()
    {
        int i;

        for (i = 0; i < size; i++)
        {
            posi = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));

            Collider collider = spawn[i].GetComponent<BoxCollider>();
            collider.isTrigger = true;
            ((BoxCollider)collider).size = new Vector3(2f, 2f, 2f);

            Instantiate(spawn[i], posi, transform.rotation);
        }
    }
}
