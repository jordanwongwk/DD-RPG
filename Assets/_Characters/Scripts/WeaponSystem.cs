﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters{
	public class WeaponSystem : MonoBehaviour {
		[SerializeField] float baseDamage = 10f;
		[SerializeField] WeaponConfig currentWeaponConfig;

		const string DEFAULT_ATTACK = "DEFAULT ATTACK";
		const string ATTACK_TRIGGER = "isAttacking";

		float timeLastHit;

		Animator animator;
		GameObject target;
		GameObject weaponObject;
		Character character;
		GameObject projectile;
		WeaponConfig playerWeaponInHand;
		EnemyAI enemy = null;

		const float ATTACK_DISTANCE_OFFSET = 0.2f;
		const float ENEMY_FLEE_DELAY = 0.1f;

		void Start () {
			character = GetComponent<Character> ();
			animator = GetComponent<Animator> ();
			enemy = GetComponent<EnemyAI> ();

			ChangeWeaponInHand (currentWeaponConfig);	
			SetAttackAnimation (); 
		}

		void Update() {
			bool targetIsDead;
			bool targetIsOutOfRange;

			if (target == null) {
				targetIsDead = false;
				targetIsOutOfRange = false;
			} else {
				var targetHealth = target.GetComponent<HealthSystem> ().healthAsPercentage;
				targetIsDead = (targetHealth <= Mathf.Epsilon); 

				var targetDistance = Vector3.Distance (transform.position, target.transform.position);
				targetIsOutOfRange = (targetDistance > currentWeaponConfig.GetMaxAttackRange());
			}

			float characterHealth = GetComponent<HealthSystem> ().healthAsPercentage;
			bool characterIsDead = (characterHealth <= Mathf.Epsilon);
			if (characterIsDead || targetIsDead || targetIsOutOfRange) {
				StopAllCoroutines ();
			}
		}

		public WeaponConfig GetCurrentWeaponConfig(){
			return currentWeaponConfig;
		}

		public float GetBaseDamage(){
			return baseDamage;
		}

		public void AttackTarget(GameObject targetToAttack){
			target = targetToAttack;
			StartCoroutine (AttackTargetRepeatedly ());
		}

		IEnumerator AttackTargetRepeatedly(){
			bool attackerStillAlive = GetComponent<HealthSystem> ().healthAsPercentage >= Mathf.Epsilon;
			bool targetStillAlive = target.GetComponent<HealthSystem> ().healthAsPercentage >= Mathf.Epsilon;

			while (attackerStillAlive && targetStillAlive) {
				var animationClip = currentWeaponConfig.GetAttAnimClip ();
				float animationTime = animationClip.length * character.GetAnimSpeedMultiplier ();
				float timeToWait = animationTime + currentWeaponConfig.GetTimeBetweenHits ();

				bool isTimeToHit = (Time.time - timeLastHit) > timeToWait;
				if (isTimeToHit) {
					AttackTargetOnce ();
					timeLastHit = Time.time;
					if (enemy) {
						enemy.SetTimingToDisableRun (timeLastHit, timeToWait);
					}
				}
				yield return new WaitForSeconds (timeToWait);		
			}
		}

		void AttackTargetOnce(){
			projectile = currentWeaponConfig.GetWeaponProjectile();

			transform.LookAt (target.transform.position);
			animator.SetTrigger (ATTACK_TRIGGER);
			float damageDelay = currentWeaponConfig.GetDamageDelay ();
			SetAttackAnimation();

			if (currentWeaponConfig.GetIsRangedWeapon() == true && projectile != null) {		
				float fireDelay = currentWeaponConfig.GetFiringDelay ();
				StartCoroutine (FireProjectile (target, fireDelay));
			} else {
				StartCoroutine (DamageAfterDelay (damageDelay));
			}
		}

		IEnumerator DamageAfterDelay (float damageDelay){
			yield return new WaitForSeconds (damageDelay);
			transform.LookAt (target.transform.position);				// Look back at target 
			target.GetComponent<HealthSystem> ().TakeDamage (CalculateDamage());
			PlayWeaponSFX ();
		}

		IEnumerator FireProjectile(GameObject target, float firingDelay){
			yield return new WaitForSeconds (firingDelay);
			var projectileFirePoint = GetComponentInChildren<ProjectileSpawner> ().gameObject;

			GameObject instantProj = Instantiate (projectile, projectileFirePoint.transform.position, Quaternion.Euler(270,0,0));
			var projectileComponent = instantProj.GetComponent<Projectile> ();
			projectileComponent.SetDamage (currentWeaponConfig.GetProjectileDamage ());
			projectileComponent.SetShooter (gameObject);
			PlayWeaponSFX ();

			Vector3 unitVectorToTarget = (target.transform.position - projectileFirePoint.transform.position).normalized;
			float projectileSpeed = currentWeaponConfig.GetProjectileSpeed ();
			instantProj.GetComponent<Rigidbody> ().velocity = unitVectorToTarget * projectileSpeed;

			yield return new WaitForSeconds (ENEMY_FLEE_DELAY);
			if (enemy) {
				enemy.SetIsRunningAway (true);
			}
		}

		void PlayWeaponSFX(){
			var weaponSound = currentWeaponConfig.GetWeaponSFX ();
			AudioSource audioSource = GetComponent<AudioSource> ();
			audioSource.PlayOneShot (weaponSound);
		}
			
		void SetAttackAnimation(){
			if (!character.GetOverrideController ()) {
				Debug.Break ();
				Debug.LogAssertion ("Please provide " + gameObject + " with an animator override controller.");
			} else {
				var animatorOverrideController = character.GetOverrideController ();
				animator.runtimeAnimatorController = animatorOverrideController;
				animatorOverrideController [DEFAULT_ATTACK] = currentWeaponConfig.GetAttAnimClip ();
			}
		}
			
		public void ChangeWeaponInHand (WeaponConfig weaponToChange)
		{
			currentWeaponConfig = weaponToChange;		// Initializing that the equipped weapon is now the pickup weapon
			var weaponPrefab = weaponToChange.GetWeaponPrefab ();
			GameObject armToHoldWeapon = SelectDominantHand ();
			Destroy (weaponObject);				// Empty hand (Initially when game run, null is return then only equip with starting hand)
			weaponObject = Instantiate(weaponPrefab, armToHoldWeapon.transform);
			weaponObject.transform.localPosition = weaponToChange.gripTransform.transform.localPosition;
			weaponObject.transform.localRotation = weaponToChange.gripTransform.transform.localRotation;
			SetStoppingDistanceBasedOnWeaponRange ();		
		}
			
		void SetStoppingDistanceBasedOnWeaponRange(){
			float currentWeaponRange = currentWeaponConfig.GetMaxAttackRange();
			if (currentWeaponConfig.GetMaxAttackRange () != 0) {			// IF its player and enemy but NOT NPC
				if (gameObject.GetComponent<EnemyAI> () != null) {
					gameObject.GetComponent<EnemyAI> ().SetStoppingDistanceForAttack (currentWeaponRange - ATTACK_DISTANCE_OFFSET);
				} else if (gameObject.GetComponent<PlayerControl>() != null) {
					gameObject.GetComponent<PlayerControl> ().SetPlayerStopDistanceToAttack(currentWeaponRange - ATTACK_DISTANCE_OFFSET);
				}
			}
		}
			
		GameObject SelectDominantHand(){
			var dominantHands = GetComponentsInChildren<DominantHand> ();
			int numberOfDominantHands = dominantHands.Length;
			Assert.IsFalse (numberOfDominantHands <= 0, "No dominantHands detected in Player, please add one.");
			Assert.IsFalse (numberOfDominantHands >  2, "Multiple dominantHands detected in Player, please remove one.");
			return dominantHands[0].gameObject;
		}
			
		float CalculateDamage ()
		{
			return baseDamage + currentWeaponConfig.GetAdditionalDamage ();
		}

		public void PlayImpactSFX(){
			var impactSound = currentWeaponConfig.GetImpactSFX ();
			AudioSource audioSource = GetComponent<AudioSource> ();
			audioSource.PlayOneShot (impactSound);
		}

		public void StopAttacking(){
			StopAllCoroutines ();
		}
	}
}