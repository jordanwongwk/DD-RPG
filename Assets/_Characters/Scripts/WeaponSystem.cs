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

		void Start () {
			character = GetComponent<Character> ();
			animator = GetComponent<Animator> ();

			ChangeWeaponInHand (currentWeaponConfig);	
			SetAttackAnimation (); 
		}

		void Update() {
			// TODO check if the target is dead / run off
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
				float weaponHitRate = currentWeaponConfig.GetTimeBetweenHits();
				float timeToWait = weaponHitRate * character.GetAnimSpeedMultiplier ();

				bool isTimeToHit = (Time.time - timeLastHit) > timeToWait;
				if (isTimeToHit) {
					AttackTargetOnce ();
					timeLastHit = Time.time;
				}
				yield return new WaitForSeconds (timeToWait);
			}
		}

		void AttackTargetOnce(){
			transform.LookAt (target.transform);
			animator.SetTrigger (ATTACK_TRIGGER);
			float damageDelay = 1.0f; 		// TODO set a damage delay for each weapon
			SetAttackAnimation();
			StartCoroutine (DamageAfterDelay (damageDelay));
		}

		IEnumerator DamageAfterDelay (float damageDelay){
			yield return new WaitForSecondsRealtime (damageDelay);
			target.GetComponent<HealthSystem> ().TakeDamage (CalculateDamage());
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
			Assert.IsFalse (numberOfDominantHands >  1, "Multiple dominantHands detected in Player, please remove one.");
			return dominantHands[0].gameObject;
		}
			
		float CalculateDamage ()
		{
			return baseDamage + currentWeaponConfig.GetAdditionalDamage ();
		}

	}
}