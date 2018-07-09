using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

public class Checkpoint : MonoBehaviour {

	MeshRenderer meshRenderer;
	CheckpointManager checkpointManager;
	GameObject player;
	bool currentCheckpoint = false;

	// Use this for initialization
	void Start () {
		meshRenderer = GetComponent<MeshRenderer> ();
		player = FindObjectOfType<PlayerControl> ().gameObject;
		checkpointManager = FindObjectOfType<CheckpointManager> ();
	}
	
	public void SetCheckpointON (Material checkpointON) {
		meshRenderer.material = checkpointON;
		currentCheckpoint = true;
	}

	public void SetCheckpointOFF (Material checkpointOFF) {
		meshRenderer.material = checkpointOFF;
		currentCheckpoint = false;
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject == player && currentCheckpoint == false) {
			checkpointManager.SettingUpTheCheckpoint (this);
		}
	}
}
