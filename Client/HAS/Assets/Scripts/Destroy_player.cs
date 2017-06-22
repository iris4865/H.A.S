using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_player : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        GameObject winlose = GameObject.Find("WinLose");
        winlose winlose_component = winlose.GetComponent<winlose>();

        winlose_component.thief_count -= 1;
    }
}
