using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsController : MonoBehaviour
{
    public static SettingsController instance { get; private set; }
    
    GameController gc;
    PlayerController pc;
    GraphicsSettingsController gsc;

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
        gc = GameController.instance;
        pc = PlayerController.instance;
        gsc = GraphicsSettingsController.instance;
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

    public void ReturnToLevelSelect()
    {
        Time.timeScale = 1.0f; // Make sure that we set the game speed back to normal
        SceneManager.LoadScene(0); // The first scene should *always* be the main menu
    }

    // Quit game
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenGraphicsSettings ()
    {
        gsc.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
