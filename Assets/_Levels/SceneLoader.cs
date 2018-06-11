using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	[SerializeField] int loadingSceneNumber = 0;
	[SerializeField] Text loadingText = null;

	// Use this for initialization
	void Start () {
		loadingText.text = "Loading game...";
		StartCoroutine (loadingNewScene ());
	}

	IEnumerator loadingNewScene(){
		yield return null;
		AsyncOperation async = SceneManager.LoadSceneAsync (loadingSceneNumber);
		async.allowSceneActivation = false;

		while (!async.isDone) {
			if (async.progress >= 0.9f) {
				loadingText.text = "Press 'Space' to start game.";

				if (Input.GetKeyDown (KeyCode.Space)) {
					async.allowSceneActivation = true;
				}
			}

			yield return null;
		}
	}
}
