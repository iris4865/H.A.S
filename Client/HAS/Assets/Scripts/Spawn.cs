using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    int size = 10;
    public GameObject trigger_object;
    public GameObject[] spawn = new GameObject[10];    
    Vector3 posi;
    float range = 30.0f;

    // Use this for initialization
    void Start () {
        int i;
        
        for (i = 0; i < size; i++)
        {
            posi = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));

            Collider collider = spawn[i].GetComponent<BoxCollider>();
            collider.isTrigger = true;
            ((BoxCollider)collider).size = new Vector3(2f,2f,2f);

            Instantiate(spawn[i], posi, transform.rotation);
        }
    }

    void Awake(){

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
