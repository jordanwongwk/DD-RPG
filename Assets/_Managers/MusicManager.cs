using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	[Header("Audio Source Settings")]
	[SerializeField] float volumeLevel = 0.5f;

	[Header("Clips Placement")]
	[SerializeField] AudioClip levelMusic = null;
	[SerializeField] AudioClip bossMusic = null;
	[SerializeField] AudioClip secret1CutsceneMusic = null;

	AudioSource audioSource;
	GameManager gameManager;
	bool BGMFadeOutVolume = false;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.loop = true;
		audioSource.volume = volumeLevel;
		ChangeFieldBGM ();

		gameManager = GetComponent<GameManager> ();
		gameManager.triggerBossBattle += ChangeBossBGM;
		gameManager.triggerBossEnd += ChangeFieldBGM;
		gameManager.triggerRestartBoss += ChangeFieldBGM;
		gameManager.endGameSetup += BGMFadeOut;
	}

	// Update is called once per frame
	void Update () {
		if (!audioSource.isPlaying) {
			audioSource.Play ();
		} else if (BGMFadeOutVolume) {
			audioSource.volume -= Time.deltaTime / 5.0f;
		}
	}

	void ChangeBossBGM(){
		audioSource.clip = bossMusic;
	}

	void ChangeFieldBGM(){
		audioSource.clip = levelMusic;
	}

	void BGMFadeOut (){
		BGMFadeOutVolume = true;
	}

	public void SetSecretBGM(){
		audioSource.clip = secret1CutsceneMusic;
	}

	public void SetBackToFieldBGM() {
		audioSource.clip = levelMusic;
	}
}
