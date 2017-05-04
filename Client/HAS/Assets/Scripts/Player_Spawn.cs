using System.Collections.Generic;
using UnityEngine;

public class Player_Spawn : MonoBehaviour {
    public GameObject player;
    //GameObject player1 = GameObject.Find("player");
    //GameObject[] players = GameObject.FindGameObjectsWithTag("player");
    //GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
    Vector3 position;

    int nonPlayerCount = 4;
    private List<Player5> players = new List<Player5>();
    
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
        GameObject gb = GameObject.Find("Player").gameObject;
        players.Add(gb.GetComponent<Player5>());
            
        //sc.ControlInstanceId = gb.GetInstanceID();
        for (int i = 0; i < nonPlayerCount; i++)
        {
            position = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));

            GameObject copy = Instantiate(player, position, transform.rotation);
            players.Add(copy.GetComponent<Player5>());
        }
    }
}
