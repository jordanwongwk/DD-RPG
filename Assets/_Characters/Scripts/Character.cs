using System;
using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;

namespace RPG.Characters {
	[SelectionBase]
	public class Character : MonoBehaviour
	{
		[Header("Capsule Collider Settings")]
		[SerializeField] Vector3 colliderCenter = new Vector3(0, 1.0f, 0);
		[SerializeField] float colliderRadius = 0.2f;
		[SerializeField] float colliderHeight = 2.0f;
		[SerializeField] bool isTriggerCollider = true;

		[Header("Rigidbody Settings")]
		[SerializeField] bool isThisKinematic = false;

		[Header("Nav Mesh Settings")]
		[SerializeField] float steeringSpeed = 1.0f;
		[SerializeField] float stoppingDistance = 1f;
		[SerializeField] float obstacleAvoidanceRadius = 0.1f;

		[Header("Animator Setup Settings")]
		[SerializeField] RuntimeAnimatorController animatorController = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] Avatar characterAvatar = null;

		[Header("Audio Source Settings")]
		[Range(0, 1.0f)][SerializeField] float audioVolume = 0.8f;
		[Range(0, 1.0f)][SerializeField] float audioSpatialBlend = 0f;

		[Header("Movement Settings")]
		[SerializeField] float moveSpeedMultiplier = 1.5f;
		[SerializeField] float animationSpeedMultiplier = 1.5f;
		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float moveThreshold = 1f;
		[Tooltip("0.5 to walk; 1.0 to run")] [SerializeField] [Range(0.1f, 1.0f)] float characterForwardMove = 1.0f;

		NavMeshAgent agent;
		CameraRaycaster cameraRaycaster;
		Animator animator;
		Rigidbody myRigidbody;
		Vector3 initialPosition;
		float turnAmount;
		float forwardAmount;
		bool isAlive = true;

		void Awake(){
			AddRequiredComponent ();
		}

		void AddRequiredComponent(){
			var capsuleCollider = gameObject.AddComponent<CapsuleCollider> ();
			capsuleCollider.center = colliderCenter;
			capsuleCollider.radius = colliderRadius;
			capsuleCollider.height = colliderHeight;
			capsuleCollider.isTrigger = isTriggerCollider;

			myRigidbody = gameObject.AddComponent<Rigidbody> ();
			myRigidbody.isKinematic = isThisKinematic;
			myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

			agent = gameObject.AddComponent<NavMeshAgent> ();
			agent.speed = steeringSpeed;
			agent.stoppingDistance = stoppingDistance;
			agent.radius = obstacleAvoidanceRadius;
			agent.updateRotation = false;
			agent.updatePosition = true;
			agent.autoBraking = false;

			animator = gameObject.AddComponent<Animator> ();
			animator.runtimeAnimatorController = animatorController;
			animator.avatar = characterAvatar;

			var audioSource = gameObject.AddComponent<AudioSource> ();
			audioSource.volume = audioVolume;
			audioSource.spatialBlend = audioSpatialBlend;
			audioSource.playOnAwake = false;

			initialPosition = transform.position;
		}

		public void SetInitialPosition (Vector3 newInitialPos){
			initialPosition = newInitialPos;
		}

		void Update(){
			if (agent.remainingDistance > agent.stoppingDistance && isAlive) {
				Move (agent.desiredVelocity);
			} else {
				Move (Vector3.zero);
			}
		}

		public float GetAnimSpeedMultiplier(){
			return animationSpeedMultiplier;
		}

		public void SetIsAlive(bool status){
			isAlive = status;
		}

		public bool GetIsAlive () {
			return isAlive;
		}

		public void RespawningSetup() {
			isAlive = true;
			GetComponent<CapsuleCollider> ().enabled = true;
			GetComponent<HealthSystem> ().SetRespawnFullHealth ();
			SetWarpPosition (initialPosition);
			SetDestination (initialPosition);
		}

		public void PlayerRespawnSetup() {
			isAlive = true;
			SetWarpPosition (initialPosition);
			SetDestination (initialPosition);
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

		public void SetWarpPosition (Vector3 teleportPos){
			agent.Warp (teleportPos);
		}

		public void SetDestination(Vector3 worldPos){
			agent.destination = worldPos;
		}

		public void SetStoppingDistance (float newStopDistance){
			agent.stoppingDistance = newStopDistance;
		}

		public float GetStoppingDistance (){
			return agent.stoppingDistance;
		}

		public AnimatorOverrideController GetOverrideController (){
			return animatorOverrideController;
		}

		void Move(Vector3 movement)
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
			animator.SetFloat("Forward", forwardAmount*characterForwardMove, 0.1f, Time.deltaTime);
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

