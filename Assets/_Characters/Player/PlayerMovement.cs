using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;

namespace RPG.Characters {
	[RequireComponent(typeof (NavMeshAgent))]
	[RequireComponent(typeof (AICharacterControl))]
	[RequireComponent(typeof (ThirdPersonCharacter))]
	public class PlayerMovement : MonoBehaviour
	{
		ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
		CameraRaycaster cameraRaycaster;
		AICharacterControl aiCharacterControl;
		GameObject walkTarget;

	   	void Start()
	    {
	        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
	        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
			aiCharacterControl = GetComponent<AICharacterControl> ();

			walkTarget = new GameObject ("WalkTarget");
			cameraRaycaster.onMouseOverWalkable += MouseOverWalkable;
			cameraRaycaster.onMouseOverEnemy += MouseOverEnemy;
	    }
			
		void MouseOverWalkable (Vector3 destination){
			if (Input.GetMouseButton (0)) {
				walkTarget.transform.position = destination;
				aiCharacterControl.SetTarget (walkTarget.transform);
			}
		}

		void MouseOverEnemy (Enemy enemy){
			if (Input.GetMouseButton (0) || Input.GetMouseButton (1)) {
				aiCharacterControl.SetTarget (enemy.transform);
			}
		}

	//	TODO Make this work again
		void ProcessDirectMovement(){
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			// calculate camera relative direction to move:
			Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
			Vector3 move = v*camForward + h*Camera.main.transform.right;

			thirdPersonCharacter.Move (move, false, false);
		}
	}
}

