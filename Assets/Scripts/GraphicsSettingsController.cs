using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSettingsController : MonoBehaviour
{
    public static GraphicsSettingsController instance { get; private set; }
    
	settingsController sc;

	// Used to initialise singleton
    void Awake()
    {
        if (instance != null)
            throw new System.Exception();

        instance = this;
    }

	// Use this for initialization
	void Start ()
	{
		sc = settingsController.instance;
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void CloseGraphicsSettings()
	{
        sc.gameObject.SetActive(true);
		gameObject.SetActive(false);
	}
}
