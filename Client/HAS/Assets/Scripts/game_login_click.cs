using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_login_click : MonoBehaviour {

    public InputField idField;
    public InputField passwordField;

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
            msg.Push(idField.text);
            msg.Push(passwordField.text);

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
