using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[RequireComponent(typeof(WeaponSystem))]
	[RequireComponent(typeof(HealthSystem))]
	public class EnemyAI : MonoBehaviour{		
		[Header("General")]
		[SerializeField] float chaseRadius = 6f;
		[SerializeField] WaypointContainer patrolPath = null;
		[SerializeField] float waypointTolerance = 2.0f;
		[SerializeField] float waitAtWaypoint = 2.0f;

		[Header("Skills")]
		[SerializeField] AbilityConfig[] abilities = null;
		[SerializeField] float timeWaitBetweenCasts = 5f;		

		[Header("Ranged Enemy")]
		[SerializeField] GameObject fleeWaypoint = null;

		float timeLastCast;
		bool isReadyToCastAbility = false;
		bool channelingAbility = false;
		bool isRunningAway = false;

		int nextWaypointIndex;
		float currentWeaponRange;
		float distanceToPlayer;
		float enemyStoppingDistanceForAttack;
		float timeLastHit;
		float timeToWait;
		Vector3 enemyOriginalPosition;
		PlayerControl player;
		Character character;
		WeaponSystem weaponSystem;
		GameManager gameManager;

		const float DESTINATION_STOPPING_DISTANCE = 0.25f;

		public enum State { idle, attacking, chasing, patroling, fleeing, castingAbility }
		State state = State.idle;

		void Start(){
			character = GetComponent<Character> ();
			weaponSystem = GetComponent<WeaponSystem> ();
			player = FindObjectOfType<PlayerControl> ();
			gameManager = FindObjectOfType<GameManager> ();
			gameManager.onPlayerRespawn += OnPlayerRespawn;

			enemyOriginalPosition = transform.position;

			SetupAbilitiesBehaviour ();
		}

		void OnPlayerRespawn ()
		{
			State state = State.idle;
		}

		void SetupAbilitiesBehaviour () {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities[abilityIndex].AttachAbilityTo (gameObject);
			}
		}

		void Update(){
			if (character.GetIsAlive () == true) {
				distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
				currentWeaponRange = weaponSystem.GetCurrentWeaponConfig ().GetMaxAttackRange ();
				ToggleOffRunningAway ();

				if (distanceToPlayer > chaseRadius && state != State.patroling && !isReadyToCastAbility) {
					StopAllCoroutines ();
					character.SetStoppingDistance (DESTINATION_STOPPING_DISTANCE); 
					StartCoroutine (Patrol ());
				}
				if (distanceToPlayer <= chaseRadius && distanceToPlayer > currentWeaponRange && state != State.chasing && !channelingAbility && !isRunningAway) {
					StopAllCoroutines ();
					character.SetStoppingDistance (enemyStoppingDistanceForAttack);
					StartCoroutine (ChasePlayer ());
				}
				if (distanceToPlayer <= currentWeaponRange && state != State.attacking && !isReadyToCastAbility && !isRunningAway && player.GetComponent<Character> ().GetIsAlive ()) {
					StopAllCoroutines ();
					character.SetStoppingDistance (enemyStoppingDistanceForAttack);
					StartCoroutine (AttackPlayer ());
				}
				if (distanceToPlayer <= currentWeaponRange && state != State.fleeing && !isReadyToCastAbility && isRunningAway) {
					StopAllCoroutines ();
					StartCoroutine (FleeFromPlayer ());
				}
				if (distanceToPlayer <= currentWeaponRange && state != State.castingAbility && isReadyToCastAbility && player.GetComponent<Character> ().GetIsAlive ()) {
					StopAllCoroutines ();
					weaponSystem.StopAttacking ();
					int chosenAbility = Random.Range (0, abilities.Length);
					StartCoroutine (CastAbility (chosenAbility));
				}

			}
		}

		void ToggleOffRunningAway(){
			if (isRunningAway) {
				bool isTimeToStopRunning = (Time.time - timeLastHit) > timeToWait;
				if (isTimeToStopRunning) {
					isRunningAway = false;
				}
			}
		}

		IEnumerator Patrol(){
			state = State.patroling;
			if (patrolPath == null) {
				character.SetDestination (enemyOriginalPosition);
			}

			while (patrolPath != null) {
				Vector3 nextWaypointPos = patrolPath.transform.GetChild (nextWaypointIndex).position;
				character.SetDestination (nextWaypointPos);
				CycleWaypointWhenClose (nextWaypointPos);
				yield return new WaitForSeconds (waitAtWaypoint);
			}
		}

		void CycleWaypointWhenClose (Vector3 nextWaypointPos){
			// Only increase index if enemy is sufficiently close to the last waypoint (eg after chase, run back to original spot before proceed)
			if (Vector3.Distance (transform.position, nextWaypointPos) <= waypointTolerance) {		
				nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
			}
		}

		IEnumerator ChasePlayer(){
			state = State.chasing;
			while (distanceToPlayer >= currentWeaponRange) {
				character.SetDestination (player.transform.position);
				CheckForAbilityAvailability ();
				yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator AttackPlayer(){
			state = State.attacking;
			while (distanceToPlayer <= currentWeaponRange) {
				transform.LookAt (player.transform);
				weaponSystem.AttackTarget (player.gameObject);
				CheckForAbilityAvailability ();
				yield return new WaitForEndOfFrame ();
			}
		}

		void CheckForAbilityAvailability ()
		{
			if (abilities.Length != 0) {				// If there is ability in the enemy
				float timeWaitForCast = timeWaitBetweenCasts;
				bool isTimeToCast = (Time.time - timeLastCast) > timeWaitForCast;
				if (isTimeToCast) {
					isReadyToCastAbility = true;
				}
			}
		}

		IEnumerator FleeFromPlayer () {
			state = State.fleeing;
			Vector3 normalizedFleePos = (transform.position - player.transform.position).normalized;
			Vector3 fleePos = (10f * normalizedFleePos) + transform.position;
			character.SetDestination (fleePos);
			yield return new WaitForEndOfFrame ();
		}

		IEnumerator CastAbility(int abilityNumber){
			state = State.castingAbility;
			// TODO Add Audio
			channelingAbility = true;
			abilities [abilityNumber].Use (null);
			float channelTime = abilities [abilityNumber].GetChannelTime ();
			yield return new WaitForSeconds (channelTime);
			timeLastCast = Time.time;
			yield return new WaitForSeconds (0.75f);			// Make a slight delay between done casting ability -> attacking to prevent damageDelay from messing up
			isReadyToCastAbility = false;
			channelingAbility = false;
		}

		public void EnemyStopAllAction(){
			StopAllCoroutines ();
		}

		public void SetIsRunningAway (bool status){
			isRunningAway = status;
		}

		public void SetTimingToDisableRun (float lastHitTime, float waitingTime) {
			timeLastHit = lastHitTime;
			timeToWait = waitingTime;
		}

		public void SetStoppingDistanceForAttack (float stoppingDistance){
			enemyStoppingDistanceForAttack = stoppingDistance;
		}

		void OnDrawGizmos(){
			// Drawing Attack Radius
			Gizmos.color = new Color(255,0,0,0.5f);
			Gizmos.DrawWireSphere (transform.position, currentWeaponRange);

			// Drawing Move Radius
			Gizmos.color = new Color(0,255,0,0.5f);
			Gizmos.DrawWireSphere (transform.position, chaseRadius);
		}
	}
}
