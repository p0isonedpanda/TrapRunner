using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
	public Slider loadingBar;

    public void StartGame()
	{
        StartCoroutine(StartLoadLevel("test"));
	}

	public void ExitGame ()
	{
        Application.Quit();
	}

	IEnumerator StartLoadLevel(string scene)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			loadingBar.value = progress;
			Debug.Log(progress);
			
			yield return null;
		}
	}
}
