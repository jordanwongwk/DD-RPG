using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	[SerializeField] int loadingSceneNumber = 0;
	[SerializeField] Text loadingText = null;

	bool loadScene = false;

	// Use this for initialization
	void Start () {
		loadingText.text = "Press SPACE to begin the game.";
	}
	
	// Update is called once per frame
	void Update () {
		if (!loadScene) {
			loadScene = true;
			loadingText.text = "Loading game...";
			StartCoroutine (loadingNewScene ());
		}
	}

	IEnumerator loadingNewScene(){
		yield return new WaitForSeconds (3f);
		AsyncOperation async = SceneManager.LoadSceneAsync (loadingSceneNumber);

		while (!async.isDone) {
			yield return null;
		}
	}
}
