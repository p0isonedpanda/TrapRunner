using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsController : MonoBehaviour
{
    public static GraphicsSettingsController instance { get; private set; }
    
	settingsController sc;

	public Toggle vsyncToggle;

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

	public void CloseGraphicsSettings()
	{
        sc.gameObject.SetActive(true);
		gameObject.SetActive(false);
	}

	public void ChangeVSync()
	{
        if (vsyncToggle.isOn)
	        QualitySettings.vSyncCount = 1;
		else
		    QualitySettings.vSyncCount = 0;
	}
}
