using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class title_sound : MonoBehaviour {

    public AudioSource source;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (Application.loadedLevel == 4)
        {
            print("stop to play sound");
            source.Stop();
        }
	}
}
