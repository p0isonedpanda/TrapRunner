using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsController : MonoBehaviour
{
    public static GraphicsSettingsController instance { get; private set; }
    
	settingsController sc;

	public Toggle vsyncToggle, anisotropicFilteringToggle;
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
        QualitySettings.masterTextureLimit = 4 - textureDetailDropdown.value;
	}
}
