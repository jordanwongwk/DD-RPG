using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {

	[SerializeField] Material checkpointON = null;
	[SerializeField] Material checkpointOFF = null;

	List<GameObject> checkpoints = new List<GameObject>();

	GameObject currentCheckpointRegistered = null;

	// Use this for initialization
	void Start () {
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
		currentCheckpointRegistered = currentCP.gameObject;
		currentCP.SetCheckpointON (checkpointON);
	}
}
