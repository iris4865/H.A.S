using System.Collections.Generic;
using UnityEngine;

public class Player_Spawn : MonoBehaviour {
    public GameObject player;
    //GameObject player1 = GameObject.Find("player");
    //GameObject[] players = GameObject.FindGameObjectsWithTag("player");
    //GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
    Vector3 position;

    int nonPlayerCount = 4;
    private List<Player5> playerList = new List<Player5>();
    
    float range = 10.0f;
    //생성자
    // Use this for initialization
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
            position = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
            
            GameObject copy = Instantiate(player, position, transform.rotation);
            //playerList.Add(copy.GetComponent<Player5>());
            //copy.GetComponent<Player5>().main_camera.enabled = false;
        }

        player.GetComponent<Player5>().main_camera.enabled = true;
    }
}
