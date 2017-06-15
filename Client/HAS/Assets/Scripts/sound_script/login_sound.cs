﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class login_sound : MonoBehaviour {

    public AudioClip sound_1;
    public AudioClip sound_2;
    private AudioSource audio;

    public AudioClip sound_3;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(audio);

        this.audio = this.gameObject.AddComponent<AudioSource>();
        this.audio.loop = false;
    }

    public void over_sound()
    {
        this.audio.clip = this.sound_1;
        audio.Play();
    }

    public void click_sound()
    {
        this.audio.clip = this.sound_2;
        audio.Play();
    }

    public void input_text()
    {
        this.audio.clip = this.sound_3;
        audio.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}