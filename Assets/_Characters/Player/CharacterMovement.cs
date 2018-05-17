using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;

namespace RPG.Characters {
	[RequireComponent(typeof (NavMeshAgent))]
	[RequireComponent(typeof (ThirdPersonCharacter))]
	public class CharacterMovement : MonoBehaviour
	{
		[SerializeField] float stoppingDistance = 1f;

		NavMeshAgent agent;
		ThirdPersonCharacter character;   // A reference to the ThirdPersonCharacter on the object
		CameraRaycaster cameraRaycaster;
		GameObject walkTarget;

	   	void Start()
	    {
	        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
	        character = GetComponent<ThirdPersonCharacter>();

			agent = GetComponent<NavMeshAgent> ();
			agent.updateRotation = false;
			agent.updatePosition = true;
			agent.stoppingDistance = stoppingDistance;

			walkTarget = new GameObject ("WalkTarget");
			cameraRaycaster.onMouseOverWalkable += MouseOverWalkable;
			cameraRaycaster.onMouseOverEnemy += MouseOverEnemy;
	    }

		void Update(){
			if (agent.remainingDistance > agent.stoppingDistance) {
				character.Move (agent.desiredVelocity);
			} else {
				character.Move (Vector3.zero);
			}
		}
			
		void MouseOverWalkable (Vector3 destination){
			if (Input.GetMouseButton (0)) {
				agent.SetDestination (destination);
			}
		}

		void MouseOverEnemy (Enemy enemy){
			if (Input.GetMouseButton (0) || Input.GetMouseButton (1)) {
				agent.SetDestination (enemy.transform.position);
			}
		}
	}
}

