using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class game_login_click : MonoBehaviour {

	void Start () {
    }
	
	void Update () {
		
	}

    public void click()
    {
        GameObject networkManager = GameObject.Find("networkManager");

        if (networkManager != null)
        {
            Packet msg = PacketBufferManager.Pop((short)PROTOCOL.LoginReq, (short)SEND_TYPE.Single);
            msg.Push("abc|abcd");

            networkManager.GetComponent<NetworkManager>().Send(msg);
        }
        else
        {
            SceneManager.LoadScene(3);
        }
    }
    public void click_1()
    {
        SceneManager.LoadScene(2);
    }
}
