using System.Collections;
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

		void Start () {
			character = GetComponent<Character> ();
			animator = GetComponent<Animator> ();

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
				}
				yield return new WaitForSeconds (timeToWait);
			}
		}

		void AttackTargetOnce(){
			projectile = currentWeaponConfig.GetWeaponProjectile();

			transform.LookAt (target.transform);
			animator.SetTrigger (ATTACK_TRIGGER);
			float damageDelay = currentWeaponConfig.GetDamageDelay ();
			SetAttackAnimation();

			if (projectile != null) {
				float fireDelay = currentWeaponConfig.GetFiringDelay ();
				StartCoroutine (FireProjectile (target, fireDelay));
			} else {
				StartCoroutine (DamageAfterDelay (damageDelay));
			}
		}

		IEnumerator DamageAfterDelay (float damageDelay){
			yield return new WaitForSecondsRealtime (damageDelay);
			target.GetComponent<HealthSystem> ().TakeDamage (CalculateDamage());
			PlayWeaponSFX ();
		}

		IEnumerator FireProjectile(GameObject target, float firingDelay){
			yield return new WaitForSecondsRealtime (firingDelay);
			var projectileFirePoint = GetComponentInChildren<ProjectileSpawner> ().gameObject;

			GameObject instantProj = Instantiate (projectile, projectileFirePoint.transform.position, Quaternion.Euler(270,0,0));
			var projectileComponent = instantProj.GetComponent<Projectile> ();
			projectileComponent.SetDamage (currentWeaponConfig.GetProjectileDamage ());
			projectileComponent.SetShooter (gameObject);
			PlayWeaponSFX ();

			Vector3 unitVectorToTarget = (target.transform.position - projectileFirePoint.transform.position).normalized;
			float projectileSpeed = currentWeaponConfig.GetProjectileSpeed ();
			instantProj.GetComponent<Rigidbody> ().velocity = unitVectorToTarget * projectileSpeed;
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