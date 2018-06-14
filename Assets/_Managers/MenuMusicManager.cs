using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusicManager : MonoBehaviour {
	[SerializeField] AudioClip mainMenuTheme = null;
	[SerializeField] float audioVolume = 0.6f;

	MenuUIManager uiManager;
	static MenuMusicManager instance = null;

	// Use this for initialization
	void Start () {
		uiManager = FindObjectOfType<MenuUIManager> ();

		if (instance != null) {
			Destroy (gameObject);
			return;
		} else {
			uiManager.PanelFadeIn ();
			instance = this;
		}

		AudioSource audioSource;
		audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.volume = audioVolume;
		audioSource.playOnAwake = true;
		audioSource.loop = true;
		audioSource.PlayOneShot (mainMenuTheme);
			
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
