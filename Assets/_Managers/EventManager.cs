using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.CameraUI;
using RPG.Characters;

public class EventManager : MonoBehaviour {

	[Header ("Secret Event 1 Objects")]
	[SerializeField] GameObject charPrefab = null;
	[SerializeField] GameObject charStandPosition = null;

	[Header ("Secret Event 2 Objects")]
	[SerializeField] GameObject playerStandPosition = null;

	GameObject player;
	GameObject derrickNPC;
	GameObject eventCharacter;
	GameObject mainBoss;
	CameraFollow cameraFollow;

	bool secretEvent1Initiated = false;
	bool secretEvent2Initiated = false;

	// Use this for initialization
	void Start () {
		derrickNPC = GameObject.FindGameObjectWithTag ("Derrick");
		mainBoss = GameObject.FindGameObjectWithTag ("Boss");
		cameraFollow = FindObjectOfType<CameraFollow> ();
		player = FindObjectOfType<PlayerControl> ().gameObject;
	}

	public void StartSecretEvent1() {
		secretEvent1Initiated = true;
		eventCharacter = Instantiate (charPrefab, charStandPosition.transform.position, Quaternion.identity);
		eventCharacter.transform.LookAt (derrickNPC.transform);

		cameraFollow.SetCameraFollowingDerrick ();
	}

	public void StartSecretEvent2() {
		secretEvent2Initiated = true;
		mainBoss.GetComponent<EnemyAI> ().enabled = false;
		mainBoss.transform.LookAt (player.transform);

		player.GetComponent<Character> ().SetDestination (playerStandPosition.transform.position);
	}

	public bool GetSecretEvent2Initiation() {	// To raise the collider
		return secretEvent2Initiated;
	}

	public void EndEvents() {
		if (secretEvent1Initiated) {
			Destroy (eventCharacter);
			cameraFollow.SetCameraBackToPlayer ();
			secretEvent1Initiated = false;
		}

		if (secretEvent2Initiated) {
			mainBoss.GetComponent<EnemyAI> ().enabled = true;
			secretEvent2Initiated = false;
		}
	}

}
