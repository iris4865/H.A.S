using HatchlingNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_login_click : MonoBehaviour {

    public InputField id;
    public InputField password;

    string id_s;
    string password_s;

    void Start () {
        id = GetComponent<InputField>();
        password = GetComponent<InputField>();
    }
	
	void Update () {
		
	}

    public void click()
    {
        id_s = id.text;
        password_s = password.text;

        if(id_s == null || password_s == null)
        {
            //id나password를 입력하라는 메시지 출력.
            return;
        }

        GameObject networkManager = GameObject.Find("networkManager");

        if (networkManager != null)
        {
            //Packet msg = PacketBufferManager.Pop((short)PROTOCOL.LoginReq);
            //msg.Push("abc|abcd");

            //networkManager.GetComponent<NetworkManager>().Send(msg);
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
