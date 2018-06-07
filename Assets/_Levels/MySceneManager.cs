using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {
	[Header("Level Management")]
	[SerializeField] int mainMenuSceneIndex;
	[SerializeField] int startGameSceneIndex;
	[SerializeField] int rankingSceneIndex;

	public void StartGameScene(){
		SceneManager.LoadScene (startGameSceneIndex);
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

}
