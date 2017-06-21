using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_wait_click : MonoBehaviour {

    object networkManager;

    public int user_count = 0;

    public Image[] user = new Image[4];

    // Use this for initialization
    void Start () {
        networkManager = GameObject.FindWithTag("eNetworkManager");
        for (int i = 0; i < 4; i++)
        {
            user[i].color = new Color32(255, 255, 255, 255);
        }

        
    }
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < user_count; i++)
        {
            user[i].color = new Color32(255, 0, 0, 255);
        }
	}
    public void click()
    {
        SceneManager.LoadScene(4);
    }
    public void click_1()
    {
        SceneManager.LoadScene(1);
    }
}
