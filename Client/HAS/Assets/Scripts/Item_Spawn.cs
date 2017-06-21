using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Spawn : MonoBehaviour {
    public GameObject item_object;
    Vector3 spawn_position;

    public GameObject[] point;

    // Use this for initialization
    void Start () {
        
    }

    void Awake(){

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void item_create(int position)
    {
        spawn_position = point[position].transform.position;

        Collider collider = item_object.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        ((BoxCollider)collider).size = new Vector3(2f, 2f, 2f);

        Instantiate(item_object, spawn_position, transform.rotation);
    }
}
