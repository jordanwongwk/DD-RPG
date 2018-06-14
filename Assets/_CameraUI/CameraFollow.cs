using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI{
	public class CameraFollow : MonoBehaviour {

		[SerializeField] float transitionSpeed = 2.0f;
		[SerializeField] bool isFollowingDerrick = false;

		GameObject player;
		GameObject derrickNPC;
		GameManager gameManager;

		float lastRecordTimeOnPlayer = 0;
		float lastRecordTimeNotOnPlayer = 0;

		// Use this for initialization
		void Start () {
			player = GameObject.FindGameObjectWithTag ("Player");
			derrickNPC = GameObject.FindGameObjectWithTag ("Derrick");

			gameManager = FindObjectOfType<GameManager> ();
			gameManager.onPlayerRespawn += SetLastRecordNotOnPlayer;

			transform.position = new Vector3 (player.transform.position.x, player.transform.position.y + 0.1f, player.transform.position.z); 
			// Made a slight offset to prevent divide by 0
		}

		public void SetCameraFollowingDerrick () {
			isFollowingDerrick = true;
		}

		public void SetCameraBackToPlayer () {
			isFollowingDerrick = false;
			// All other follow is false here
		}

		void LateUpdate () {
			if (isFollowingDerrick) {
				Vector3 derrickNPCPos = derrickNPC.transform.position;
				float distance = Vector3.Distance (transform.position, derrickNPCPos);
				float distCover = (Time.time - lastRecordTimeOnPlayer) * transitionSpeed;

				transform.position = Vector3.Lerp (transform.position, derrickNPCPos, distCover / distance);
				lastRecordTimeNotOnPlayer = Time.time;
			} else {
				float distanceToLastPlayerPos = Vector3.Distance (transform.position, player.transform.position);
				float distCoverToLastPlayerPos = (Time.time - lastRecordTimeNotOnPlayer) * transitionSpeed;

				transform.position = Vector3.Lerp(transform.position, player.transform.position, distCoverToLastPlayerPos / distanceToLastPlayerPos);
				lastRecordTimeOnPlayer = Time.time;
			}
		}

		void SetLastRecordNotOnPlayer(){
			lastRecordTimeNotOnPlayer = Time.time;
		}
	}
}