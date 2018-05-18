﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters {
	public class Enemy : MonoBehaviour, IDamageable {		//TODO Remove Interface

		[SerializeField] float attackRadius = 5f;
		[SerializeField] float moveRadius = 6f;
		[SerializeField] float projectileDamage = 9f;
		[SerializeField] float projectileShotDelay = 0.5f;
		[SerializeField] float variationDelay = 0.1f;
		[SerializeField] GameObject projectileToUse = null;
		[SerializeField] GameObject projectileSpawnPoint = null;
		[SerializeField] Vector3 aimOffset = new Vector3(0,1f,0);

		bool isAttacking = false;
		Player player;

		void Start(){
			player = FindObjectOfType<Player>();
		}

		public void TakeDamage (float amount){
			Debug.Log ("OUCH!");
			//TODO Remove this
		}

		void Update(){
			float distanceDiff = Vector3.Distance (player.transform.position, transform.position);

			// For attack radius
			if (distanceDiff <= attackRadius) {
				if (!isAttacking) {
					isAttacking = true;
					float randomizedDelay = Random.Range ((projectileShotDelay - variationDelay), (projectileShotDelay + variationDelay));
					InvokeRepeating ("SpawnProjectiles", 0f, randomizedDelay);
				}
				// Make enemy keep looking at player
				transform.LookAt (player.transform.position);
			} 

			if (distanceDiff > attackRadius) {
				isAttacking = false;
				CancelInvoke ();
			}	

			//TODO Move and attack
			// For move radius
			if (distanceDiff <= moveRadius) {
				// aiCharacterControl.SetTarget (player.transform);
			} else {
				// aiCharacterControl.SetTarget (transform);
			}
		}

		void SpawnProjectiles ()
		{
			GameObject projectile = Instantiate (projectileToUse, projectileSpawnPoint.transform.position, Quaternion.identity);
			Projectile projComponent = projectile.GetComponent<Projectile> ();
			projComponent.SetDamage(projectileDamage);
			projComponent.SetShooter (gameObject);

			Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSpawnPoint.transform.position).normalized;
			float projectileSpeed = projComponent.GetProjectileSpeed();
			projectile.GetComponent<Rigidbody> ().velocity = unitVectorToPlayer * projectileSpeed;
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
