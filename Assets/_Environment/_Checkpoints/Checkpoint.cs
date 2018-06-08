using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

public class Checkpoint : MonoBehaviour {

	MeshRenderer meshRenderer;
	CheckpointManager checkpointManager;
	bool currentCheckpoint = false;

	// Use this for initialization
	void Start () {
		meshRenderer = GetComponent<MeshRenderer> ();
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
		if (col.gameObject == FindObjectOfType<PlayerControl> ().gameObject && currentCheckpoint == false) {
			checkpointManager.SettingUpTheCheckpoint (this);
		}
	}
}
