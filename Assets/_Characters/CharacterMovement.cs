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
		[SerializeField] float moveSpeedMultiplier = 1.5f;

		NavMeshAgent agent;
		ThirdPersonCharacter character;   // A reference to the ThirdPersonCharacter on the object
		CameraRaycaster cameraRaycaster;
		GameObject walkTarget;
		Animator animator;
		Rigidbody myRigidbody;

	   	void Start()
	    {
	        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
	        character = GetComponent<ThirdPersonCharacter>();

			animator = GetComponent<Animator> ();
			myRigidbody = GetComponent<Rigidbody> ();

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

		public void OnAnimatorMove(){
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (Time.deltaTime > 0){
				Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity. Then apply the velocity.
				velocity.y = myRigidbody.velocity.y;
				myRigidbody.velocity = velocity;
			}
		}
	}
}

