using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void PlayGame()
	{

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame()
	{
		Debug.Log("Quit Button Clicked");
		Application.Quit();
	}

	public static bool GameIsPaused = false;
	public GameObject pauseMenuUI;


	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameIsPaused)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
	}

	public void Resume()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		GameIsPaused = false;
	}

	public void Pause()
	{
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0;
		GameIsPaused = true;
	}

	public void LoadMenu()
	{
		Debug.Log("Load Game Triggered");
		SceneManager.LoadScene("StartMenu");
		Time.timeScale = 1f;
	}


}
