using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class game_login_click : MonoBehaviour {

 //   NetworkManager networkManager;

	// Use this for initialization
	void Start () {
//        this.networkManager = GameObject.Find("networkManager").GetComponent<NetworkManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void click()
    {
        Packet msg = PacketBufferManager.Pop((short)PROTOCOL.LoginReq);
        msg.Push("abc|abcd");
        GameObject.Find("networkManager").GetComponent<NetworkManager>().Send(msg);

//        SceneManager.LoadScene(3);
    }
    public void click_1()
    {
        SceneManager.LoadScene(2);
    }
}
