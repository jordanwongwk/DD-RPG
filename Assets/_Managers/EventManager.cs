using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.CameraUI;

public class EventManager : MonoBehaviour {

	[Header ("Secret Event 1 Objects")]
	[SerializeField] GameObject charPrefab = null;
	[SerializeField] GameObject charStandPosition = null;

	GameObject derrickNPC;
	GameObject eventCharacter;
	CameraFollow cameraFollow;


	// Use this for initialization
	void Start () {
		derrickNPC = GameObject.FindGameObjectWithTag ("Derrick");
		cameraFollow = FindObjectOfType<CameraFollow> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartSecretEvent1() {
		eventCharacter = Instantiate (charPrefab, charStandPosition.transform.position, Quaternion.identity);
		eventCharacter.transform.LookAt (derrickNPC.transform);

		cameraFollow.SetCameraFollowingDerrick ();
	}

	public void EndEvents() {
		Destroy (eventCharacter);
		cameraFollow.SetCameraBackToPlayer ();
	}
}
