using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class winlose : MonoBehaviour {

    public int item_count = 3;
    public int thief_count = 2;

    public int user_job = 0; //1이면 경찰 2이면 도둑

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //win 5 lose 6
		if(item_count == 0)
        {
            if(user_job == 1)
            {
                SceneManager.LoadScene(6);
            }
            else if(user_job == 2)
            {
                SceneManager.LoadScene(5);
            }
        }else if(thief_count == 0)
        {
            if (user_job == 1)
            {
                SceneManager.LoadScene(5);
            }
            else if (user_job == 2)
            {
                SceneManager.LoadScene(6);
            }
        }
	}
}
