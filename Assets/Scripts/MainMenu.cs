using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	GameManagerScript GM_MNG_SCR;

	void Awake()
	{
		GM_MNG_SCR = GetComponent<GameManagerScript>();
	}

	public void PlayGame()
	{
		GM_MNG_SCR.SetGameState(GameState.MainMenu);

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame()
	{
		Debug.Log("Quit Button Clicked");
		Application.Quit();
	}

}
