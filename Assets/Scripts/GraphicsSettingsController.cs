using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsController : MonoBehaviour
{
    public static GraphicsSettingsController instance { get; private set; }
    
	SettingsController sc;

	public Toggle vsyncToggle, anisotropicFilteringToggle, screenSizeToggle;
	public Dropdown antiAliasingDropdown, textureDetailDropdown;

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
        // Set graphics settings to show the default settings
		if (QualitySettings.vSyncCount == 1)
	        vsyncToggle.isOn = true;
		else
		    vsyncToggle.isOn = false;

		switch (QualitySettings.antiAliasing)
		{
            case 0:
			    antiAliasingDropdown.value = 0;
				break;
			case 2:
			    antiAliasingDropdown.value = 1;
				break;
			case 4:
			    antiAliasingDropdown.value = 2;
				break;
			case 8:
			    antiAliasingDropdown.value = 3;
				break;
		}

		if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
		    anisotropicFilteringToggle.isOn = true;
		else
		    anisotropicFilteringToggle.isOn = false;

		switch (QualitySettings.masterTextureLimit)
		{
			case 0:
			    textureDetailDropdown.value = 3;
				break;
			case 1:
			    textureDetailDropdown.value = 2;
				break;
			case 2:
			    textureDetailDropdown.value = 1;
				break;
			case 3:
			    textureDetailDropdown.value = 0;
				break;
		}

		screenSizeToggle.isOn = Screen.fullScreen;

		sc = SettingsController.instance;
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

	public void ChangeAA()
	{
        switch (antiAliasingDropdown.value)
		{
			case 0:
			    QualitySettings.antiAliasing = 0;
				break;
			case 1:
			    QualitySettings.antiAliasing = 2;
				break;
			case 2:
			    QualitySettings.antiAliasing = 4;
				break;
			case 3:
			    QualitySettings.antiAliasing = 8;
				break;
		}
	}

	public void ChangeAnisotropicFiltering()
	{
        if (anisotropicFilteringToggle.isOn)
		    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
		else
		    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
	}

	public void ChangeTextureDetail()
	{
        QualitySettings.masterTextureLimit = 3 - textureDetailDropdown.value;
	}

	public void ChangeScreenSize()
	{
		Screen.fullScreen = screenSizeToggle.isOn;
	}
}
