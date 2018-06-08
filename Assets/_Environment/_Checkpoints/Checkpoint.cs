using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

public class Checkpoint : MonoBehaviour {

	MeshRenderer meshRenderer;
	CheckpointManager checkpointManager;

	// Use this for initialization
	void Start () {
		meshRenderer = GetComponent<MeshRenderer> ();
		checkpointManager = FindObjectOfType<CheckpointManager> ();
	}
	
	public void SetCheckpointON (Material checkpointON) {
		meshRenderer.material = checkpointON;
	}

	public void SetCheckpointOFF (Material checkpointOFF) {
		meshRenderer.material = checkpointOFF;
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject == FindObjectOfType<PlayerControl> ().gameObject) {
			checkpointManager.SettingUpTheCheckpoint (this);
		}
	}
}
