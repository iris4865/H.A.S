using HatchlingNet;
using Header;
using System.Collections.Generic;
using UnityEngine;

public class Player_Spawn : MonoBehaviour
{
    public GameObject player;

    public GameObject[] point;

    Vector3[] posi = new Vector3[4];

    float range = 10.0f;
    //생성자
    // Use this for initialization
    void Start()
    {
        posi[0] = new Vector3(10, 1, 0);
        posi[1] = new Vector3(-10, 1, 0);
        posi[2] = new Vector3(0, 1, 10);
        posi[3] = new Vector3(0, 1, -10);
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject CreateMyPlayer(int job, int position)
    {
        GameObject myPlayer = Instantiate(player, posi[position], transform.rotation);

        Player5 createPlayer = myPlayer.GetComponent<Player5>();
        createPlayer.player_job = job;

        return myPlayer;
    }
}
