using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[RequireComponent(typeof(WeaponSystem))]
	public class EnemyAI : MonoBehaviour{		
		[SerializeField] float chaseRadius = 6f;
		[SerializeField] WaypointContainer patrolPath;
		[SerializeField] float waypointTolerance = 2.0f;
		[SerializeField] float waitAtWaypoint = 2.0f;

		int nextWaypointIndex;
		float currentWeaponRange;
		float distanceToPlayer;
		PlayerControl player;
		Character character;
		WeaponSystem weaponSystem;

		public enum State { idle, attacking, chasing, patroling }
		State state = State.idle;

		void Start(){
			character = GetComponent<Character> ();
			weaponSystem = GetComponent<WeaponSystem> ();
			player = FindObjectOfType<PlayerControl>();
		}

		void Update(){
			distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
			WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
			currentWeaponRange = weaponSystem.GetCurrentWeaponConfig ().GetMaxAttackRange ();

			if (distanceToPlayer > chaseRadius && state != State.patroling) {
				StopAllCoroutines ();
				StartCoroutine (Patrol ());
			}
			if (distanceToPlayer <= chaseRadius && state != State.chasing) {
				StopAllCoroutines ();
				StartCoroutine(ChasePlayer());
			}
			if (distanceToPlayer <= currentWeaponRange && state != State.attacking) {
				StopAllCoroutines ();
				StartCoroutine (AttackPlayer ());
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
				weaponSystem.AttackTarget (player.gameObject);
				yield return new WaitForEndOfFrame ();
			}
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
