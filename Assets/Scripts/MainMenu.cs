using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	GameManager GM_MNG_SCR;

	void Awake()
	{
		GM_MNG_SCR = GetComponent<GameManager>();
	}

	public void PlayGame()
	{

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame()
	{
		Debug.Log("Quit Button Clicked");
		Application.Quit();
	}

}
