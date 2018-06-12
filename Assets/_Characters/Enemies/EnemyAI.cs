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

		float timeLastCast;
		bool isReadyToCastAbility = false;

		int nextWaypointIndex;
		float currentWeaponRange;
		float distanceToPlayer;
		PlayerControl player;
		Character character;
		WeaponSystem weaponSystem;

		public enum State { idle, attacking, chasing, patroling, castingAbility }
		State state = State.idle;

		void Start(){
			character = GetComponent<Character> ();
			weaponSystem = GetComponent<WeaponSystem> ();
			player = FindObjectOfType<PlayerControl>();

			SetupAbilitiesBehaviour ();
		}

		void SetupAbilitiesBehaviour () {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities[abilityIndex].AttachAbilityTo (gameObject);
			}
		}

		void Update(){
			if (character.GetIsAlive () == true) {
				distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
				WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
				currentWeaponRange = weaponSystem.GetCurrentWeaponConfig ().GetMaxAttackRange ();

				if (distanceToPlayer > chaseRadius && state != State.patroling && !isReadyToCastAbility) {
					StopAllCoroutines ();
					StartCoroutine (Patrol ());
				}
				if (distanceToPlayer <= chaseRadius && state != State.chasing && !isReadyToCastAbility) {
					StopAllCoroutines ();
					StartCoroutine (ChasePlayer ());
				}
				if (distanceToPlayer <= currentWeaponRange && state != State.attacking && !isReadyToCastAbility) {
					StopAllCoroutines ();
					StartCoroutine (AttackPlayer ());
				}
				if (distanceToPlayer <= currentWeaponRange && state != State.castingAbility && isReadyToCastAbility) {
					StopAllCoroutines ();
					weaponSystem.StopAttacking ();
					Debug.Log (abilities.Length);
					int chosenAbility = Random.Range (0, abilities.Length);
					StartCoroutine (CastAbility (chosenAbility));
				}
			}
		}

		IEnumerator Patrol(){
			state = State.patroling;
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
				yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator AttackPlayer(){
			state = State.attacking;
			while (distanceToPlayer <= currentWeaponRange) {
				if (abilities.Length != 0) {				// If there is ability in the enemy
					float timeWaitForCast = timeWaitBetweenCasts;

					bool isTimeToCast = (Time.time - timeLastCast) > timeWaitForCast;
					if (isTimeToCast) {
						isReadyToCastAbility = true;
					}
				}
				weaponSystem.AttackTarget (player.gameObject);
				yield return new WaitForEndOfFrame ();
			}
		}

		IEnumerator CastAbility(int abilityNumber){
			state = State.castingAbility;
			// TODO Add Audio
			abilities [abilityNumber].Use (null);
			float channelTime = abilities [abilityNumber].GetChannelTime ();
			yield return new WaitForSeconds (channelTime);
			timeLastCast = Time.time;
			isReadyToCastAbility = false;
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
