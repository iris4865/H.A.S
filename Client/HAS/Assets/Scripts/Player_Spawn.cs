using System.Collections.Generic;
using UnityEngine;

public class Player_Spawn : MonoBehaviour {
    public GameObject player;
    Vector3 spawn_position;

    int nonPlayerCount = 4;
    //private List<Player5> playerList = new List<Player5>();
    
    float range = 10.0f;

    void Start()
    {
        CreatePlayer();
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreatePlayer()
    {
        player.GetComponent<Player5>().main_camera.enabled = false;
        //GameObject gb = GameObject.Find("Player").gameObject;
        //playerList.Add(gb.GetComponent<Player5>());

        //sc.ControlInstanceId = gb.GetInstanceID();
        for (int i = 0; i < nonPlayerCount; i++)
        {
            spawn_position = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
            
            GameObject copy = Instantiate(player, spawn_position, transform.rotation);
            //playerList.Add(copy.GetComponent<Player5>());
            //copy.GetComponent<Player5>().main_camera.enabled = false;
        }

        //player.GetComponent<Player5>().main_camera.enabled = true;
    }
}
