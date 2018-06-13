using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

public class CheckpointManager : MonoBehaviour {

	[Header("CP Material On & Off")]
	[SerializeField] Material checkpointON = null;
	[SerializeField] Material checkpointOFF = null;

	[Header("Audio Source Settings (Only Initial)")]
	[SerializeField] float audioSourceVolume = 0.2f;
	[SerializeField] AudioClip checkpointFoundClip = null;

	List<GameObject> checkpoints = new List<GameObject>();
	AudioSource audioSource;
	EnemyManager enemyManager;
	Character playerCharacter;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.AddComponent<AudioSource> ();
		audioSource.playOnAwake = false;
		audioSource.volume = audioSourceVolume;

		enemyManager = FindObjectOfType<EnemyManager> ();
		playerCharacter = FindObjectOfType<PlayerControl> ().GetComponent<Character> ();

		for (int i = 0; i < transform.childCount; i++) {
			checkpoints.Add (transform.GetChild (i).gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SettingUpTheCheckpoint(Checkpoint currentCP){
		foreach (GameObject cp in checkpoints) {
			cp.GetComponent<Checkpoint> ().SetCheckpointOFF (checkpointOFF);
		}
		currentCP.SetCheckpointON (checkpointON);
		audioSource.PlayOneShot (checkpointFoundClip);

		enemyManager.DestroyEnemyOnBrinkOfDeath ();							// Completely Destroy enemies that are dead
		playerCharacter.SetInitialPosition(currentCP.gameObject.transform.position);	// Player Checkpoint spawn position
	}
}
