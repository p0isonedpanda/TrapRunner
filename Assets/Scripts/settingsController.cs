using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingsController : MonoBehaviour
{
    public static settingsController instance { get; private set; }
    
    gameController gc;
    playerController pc;

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
        gc = gameController.instance;
        pc = playerController.instance;
        gameObject.SetActive(false);
	}

    // Function to be called when the look sensitivity is changed
    public void ChangeLookSensitivity()
    {
        pc.lookSpeed = GameObject.Find("HUD/pauseMenu/lookSensitivity/lookSensitivitySlider").
            GetComponent<Slider>().value * pc.lookSpeedMax;
        GameObject.Find("HUD/pauseMenu/lookSensitivity/value").GetComponent<Text>().
            text = (Mathf.Floor(pc.lookSpeed) / pc.lookSpeedMax).ToString();
    }

    // Used to invert Y look
    public void InvertYLook()
    {
        if (GameObject.Find("HUD/pauseMenu/invertY").GetComponent<Toggle>().isOn)
            pc.invertedLook = true;
        else
            pc.invertedLook = false;

        pc.mouseYLook *= -1; // Here we flip the mouseYlook so that when the setting is changed, the player's view isn't flipped
    }

    // Used to resume play from the pause menu
    public void ResumePlay()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        gc.paused = false;
    }

    // Quit game
    public void ExitGame()
    {
        Application.Quit();
    }
}
