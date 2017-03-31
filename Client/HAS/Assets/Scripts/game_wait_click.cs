using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class game_wait_click : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
