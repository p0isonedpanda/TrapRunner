﻿using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }

    [HideInInspector] public bool paused = false;

    // Used to initialise singleton
    void Awake ()
    {
        if (instance != null)
            throw new System.Exception();

        instance = this;
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
