using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	[Header("Audio Source Settings")]
	[SerializeField] float volumeLevel = 0.5f;

	[Header("Clips Placement")]
	[SerializeField] AudioClip levelMusic = null;
	[SerializeField] AudioClip bossMusic = null;

	AudioSource audioSource;
	GameManager gameManager;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.loop = true;
		audioSource.volume = volumeLevel;
		ChangeFieldBGM ();

		gameManager = GetComponent<GameManager> ();
		gameManager.triggerBossBattle += ChangeBossBGM;
		gameManager.triggerBossEnd += ChangeFieldBGM;
	}

	// Update is called once per frame
	void Update () {
		if (!audioSource.isPlaying) {
			audioSource.Play ();
		}
	}

	void ChangeBossBGM(){
		audioSource.clip = bossMusic;
	}

	void ChangeFieldBGM(){
		audioSource.clip = levelMusic;
	}
}
