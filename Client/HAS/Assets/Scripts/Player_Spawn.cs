using HatchlingNet;
using Header;
using System.Collections.Generic;
using UnityEngine;

public class Player_Spawn : MonoBehaviour
{
    public GameObject player;

    public GameObject[] point;

    float range = 10.0f;
    //생성자
    // Use this for initialization
    void Start()
    {
        
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
        GameObject myPlayer = Instantiate(player, point[position].transform.position, transform.rotation);

        Player5 createPlayer = myPlayer.GetComponent<Player5>();
        createPlayer.player_job = job;

        return myPlayer;
    }
}
