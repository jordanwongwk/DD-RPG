using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[RequireComponent(typeof(WeaponSystem))]
	public class EnemyAI : MonoBehaviour{		
		[SerializeField] float moveRadius = 6f;

		bool isAttacking = false;
		float attackRadius;
		PlayerMovement player;

		void Start(){
			player = FindObjectOfType<PlayerMovement>();
		}

		void Update(){
			float distanceDiff = Vector3.Distance (player.transform.position, transform.position);
			WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
			attackRadius = weaponSystem.GetCurrentWeaponConfig ().GetMaxAttackRange ();
		}

		void OnDrawGizmos(){
			// Drawing Attack Radius
			Gizmos.color = new Color(255,0,0,0.5f);
			Gizmos.DrawWireSphere (transform.position, attackRadius);

			// Drawing Move Radius
			Gizmos.color = new Color(0,255,0,0.5f);
			Gizmos.DrawWireSphere (transform.position, moveRadius);
		}
	}
}
