using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {
	[Header("Level Management")]
	[SerializeField] int mainMenuSceneIndex = 0;
	[SerializeField] int startGameSceneIndex = 1;
	[SerializeField] int rankingSceneIndex = 2;

	MenuUIManager menuUIManager;

	void Start(){
		menuUIManager = FindObjectOfType<MenuUIManager> ();
	}

	public void StartGameScene(){
		SceneManager.LoadScene (startGameSceneIndex);
		Destroy (GameObject.FindObjectOfType<MenuMusicManager> ().gameObject);
	}

	public void RankingScene(){
		SceneManager.LoadScene (rankingSceneIndex);
	}

	public void MainMenuScene(){
		SceneManager.LoadScene (mainMenuSceneIndex);
	}

	public void QuitGame(){
		Debug.Log ("Quit successful");
		Application.Quit ();
	}

	public void CancelQuitGame(){
		menuUIManager.CancelExit ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (SceneManager.GetActiveScene ().buildIndex == rankingSceneIndex) {
				SceneManager.LoadScene (mainMenuSceneIndex);
			} else if (SceneManager.GetActiveScene ().buildIndex == mainMenuSceneIndex) {
				menuUIManager.OnPressPrompExitPanel ();
			}
		}
	}
}
