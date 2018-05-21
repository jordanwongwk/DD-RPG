using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters{
	public class WeaponSystem : MonoBehaviour {
		[SerializeField] float baseDamage = 10f;
		[SerializeField] WeaponConfig currentWeapon;

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

			ChangeWeaponInHand (currentWeapon);	
			SetAttackAnimation (); 
		}

		public WeaponConfig GetCurrentWeaponConfig(){
			return currentWeapon;
		}

		public void AttackTarget(GameObject targetToAttack){
			var target = targetToAttack;
			Debug.Log ("Attacking " + target);
		}
			
		void SetAttackAnimation(){
			animator = GetComponent<Animator> ();
			var animatorOverrideController = character.GetOverrideController ();

			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController [DEFAULT_ATTACK] = currentWeapon.GetAttAnimClip ();
		}
			
		public void ChangeWeaponInHand (WeaponConfig weaponToChange)
		{
			currentWeapon = weaponToChange;		// Initializing that the equipped weapon is now the pickup weapon
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
			
		void AttackingTarget (){
			var enemy = GetComponent<EnemyAI> ();

			if (Time.time - timeLastHit > currentWeapon.GetTimeBetweenHits()) {
				SetAttackAnimation ();
				animator.SetTrigger (ATTACK_TRIGGER);
				timeLastHit = Time.time;
			}
		}
			
		float CalculateDamage ()
		{
			return baseDamage + currentWeapon.GetAdditionalDamage ();
		}

	}
}