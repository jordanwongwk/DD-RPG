using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[RequireComponent(typeof(WeaponSystem))]
	public class EnemyAI : MonoBehaviour{		
		[SerializeField] float chaseRadius = 6f;

		float currentWeaponRange;
		float distanceToPlayer;
		PlayerMovement player;
		Character character;

		enum State { idle, attacking, chasing, patroling }
		State state = State.idle;

		void Start(){
			character = GetComponent<Character> ();
			player = FindObjectOfType<PlayerMovement>();
		}

		void Update(){
			distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
			WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
			currentWeaponRange = weaponSystem.GetCurrentWeaponConfig ().GetMaxAttackRange ();

			if (distanceToPlayer > chaseRadius && state != State.patroling) {
				StopAllCoroutines ();
				state = State.patroling;
			}
			if (distanceToPlayer <= chaseRadius && state != State.chasing) {
				StopAllCoroutines ();
				StartCoroutine(ChasePlayer());
			}
			if (distanceToPlayer <= currentWeaponRange && state != State.attacking) {
				StopAllCoroutines ();
				state = State.attacking;
			}
		}

		IEnumerator ChasePlayer(){
			state = State.chasing;
			while (distanceToPlayer >= currentWeaponRange) {
				character.SetDestination (player.transform.position);
				yield return new WaitForEndOfFrame();
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
