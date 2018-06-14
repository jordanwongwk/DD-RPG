using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusicManager : MonoBehaviour {
	static MenuMusicManager instance = null;

	// Use this for initialization
	void Start () {
		if (instance != null) {
			Destroy (gameObject);
			return;
		} else {
			instance = this;
		}
			
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
