using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;

namespace RPG.Characters {
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof (NavMeshAgent))]
	public class CharacterMovement : MonoBehaviour
	{
		[SerializeField] float stoppingDistance = 1f;
		[SerializeField] float moveSpeedMultiplier = 1.5f;
		[SerializeField] float animationSpeedMultiplier = 1.5f;
		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float moveThreshold = 1f;

		NavMeshAgent agent;
		CameraRaycaster cameraRaycaster;
		Animator animator;
		Rigidbody myRigidbody;
		float turnAmount;
		float forwardAmount;

	   	void Start()
	    {
	        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();

			animator = GetComponent<Animator> ();

			myRigidbody = GetComponent<Rigidbody> ();
			myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

			agent = GetComponent<NavMeshAgent> ();
			agent.updateRotation = false;
			agent.updatePosition = true;
			agent.stoppingDistance = stoppingDistance;

			cameraRaycaster.onMouseOverWalkable += MouseOverWalkable;
			cameraRaycaster.onMouseOverEnemy += MouseOverEnemy;
	    }

		void Update(){
			if (agent.remainingDistance > agent.stoppingDistance) {
				Move (agent.desiredVelocity);
			} else {
				Move (Vector3.zero);
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

		public void Move(Vector3 movement)
		{
			SetForwardAndTurn (movement);
			ApplyExtraTurnRotation();
			UpdateAnimator(); 
		}

		void SetForwardAndTurn (Vector3 movement)
		{
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired direction.
			if (movement.magnitude > moveThreshold) {
				movement.Normalize ();
			}
			var localMove = transform.InverseTransformDirection (movement);
			turnAmount = Mathf.Atan2 (localMove.x, localMove.z);
			forwardAmount = localMove.z;
		}

		void UpdateAnimator()
		{
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			animator.speed = animationSpeedMultiplier;
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}
	}
}

