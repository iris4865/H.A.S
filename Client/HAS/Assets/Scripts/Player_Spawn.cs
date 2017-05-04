using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Spawn : MonoBehaviour {
    public GameObject player = new GameObject();
    Player5 a = new Player5();
    Vector3 posi;
    
    float range = 10.0f;
    //생성자
    // Use this for initialization
    void Start()
    {
        player_create();
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void player_create()
    {
        int i;

        for (i = 0; i < 4; i++)
        {
            posi = new Vector3(Random.Range(-range, range), 0f, Random.Range(-range, range));
            player.tag= "hahaha";
            print(i + "번째 이름:" + player.name);
            print(i + "번째 태그: " + player.tag);
            Instantiate(a, posi, transform.rotation);
        }
    }
}
