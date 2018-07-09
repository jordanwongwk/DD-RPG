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
	[SerializeField] GameObject playerStandPositionSecret2 = null;

	[Header ("Secret Event 3 Objects")]
	[SerializeField] GameObject bossPrefab = null;
	[SerializeField] GameObject bossSpawnPoint = null;
	[SerializeField] GameObject playerStandPositionSecret3 = null;
	[SerializeField] GameObject derrickRunOffPoint = null;

	const float DESTROY_DELAY = 20f;

	GameObject player;
	GameObject derrickNPC;
	GameObject eventCharacter;
	GameObject mainBoss;
	GameObject optionalBoss;
	CameraFollow cameraFollow;
	MusicManager musicManager;

	bool secretEvent1Initiated = false;
	bool secretEvent2Initiated = false;
	bool secretEvent3Initiated = false;
	bool secretEvent3Ended = false;

	// Use this for initialization
	void Start () {
		derrickNPC = GameObject.FindGameObjectWithTag ("Derrick");
		mainBoss = GameObject.FindGameObjectWithTag ("Boss");
		cameraFollow = FindObjectOfType<CameraFollow> ();
		player = FindObjectOfType<PlayerControl> ().gameObject;
		musicManager = GetComponent<MusicManager> ();
	}

	public void StartSecretEvent1() {
		secretEvent1Initiated = true;
		eventCharacter = Instantiate (charPrefab, charStandPosition.transform.position, Quaternion.identity);
		eventCharacter.transform.LookAt (derrickNPC.transform);
		musicManager.SetSecretBGM ();

		cameraFollow.SetCameraFollowingDerrick ();
	}

	public void StartSecretEvent2() {
		secretEvent2Initiated = true;
		mainBoss.GetComponent<EnemyAI> ().enabled = false;
		mainBoss.transform.LookAt (player.transform);

		player.GetComponent<Character> ().SetDestination (playerStandPositionSecret2.transform.position);
	}

	public bool GetSecretEvent2Initiation() {	// To raise the collider
		return secretEvent2Initiated;
	}

	public void StartSecretEvent3(){
		secretEvent3Initiated = true;
		optionalBoss = Instantiate (bossPrefab, bossSpawnPoint.transform.position, Quaternion.identity);
		optionalBoss.transform.LookAt (player.transform);
		optionalBoss.transform.parent = FindObjectOfType<EnemyManager> ().gameObject.transform;
		optionalBoss.GetComponent<EnemyAI> ().enabled = false;

		player.GetComponent<Character> ().SetDestination (playerStandPositionSecret3.transform.position);
	}

	public bool GetSecretEvent3Initiation() {
		return secretEvent3Initiated;
	}

	public bool GetSecretEvent3Ended() {  	// Raise the trigger for boss battle
		return secretEvent3Ended;
	}

	public void EndEvents() {
		if (secretEvent1Initiated) {
			Destroy (eventCharacter, DESTROY_DELAY);
			musicManager.SetBackToFieldBGM ();
			cameraFollow.SetCameraBackToPlayer ();
			secretEvent1Initiated = false;
		}

		if (secretEvent2Initiated) {
			mainBoss.GetComponent<EnemyAI> ().enabled = true;
			secretEvent2Initiated = false;
		}

		if (secretEvent3Initiated) {
			optionalBoss.GetComponent<EnemyAI> ().enabled = true;
			derrickNPC.GetComponent<Character> ().SetDestination (derrickRunOffPoint.transform.position);
			Destroy (derrickNPC, DESTROY_DELAY);
			secretEvent3Initiated = false;
			secretEvent3Ended = true;
		}
	}
}
