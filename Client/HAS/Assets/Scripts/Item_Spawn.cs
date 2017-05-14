using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Spawn : MonoBehaviour {

    int size = 10;
    public GameObject[] item_object = new GameObject[10]; 
    Vector3 spawn_position;
    float range = 30.0f;

    // Use this for initialization
    void Start () {
        //item_create();
    }

    void Awake(){

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void item_create()
    {
        for (int i = 0; i < size; i++)
        {
            spawn_position = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));

            Collider collider = item_object[i].GetComponent<BoxCollider>();
            collider.isTrigger = true;
            ((BoxCollider)collider).size = new Vector3(2f, 2f, 2f);

            Instantiate(item_object[i], spawn_position, transform.rotation);
        }
    }
}
