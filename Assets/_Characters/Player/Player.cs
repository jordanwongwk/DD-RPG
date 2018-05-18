using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Core;

namespace RPG.Characters{
	public class Player : MonoBehaviour{

		[SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon currentWeapon = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;
		[Range (0.1f, 1.0f)][SerializeField] float criticalHitChance = 0.1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;
		[SerializeField] ParticleSystem criticalHitParticle = null;

		// Serialized for debugging
		[SerializeField] AbilityConfig[] abilities = null;

		const string ATTACK_TRIGGER = "isAttacking";
		const string DEFAULT_ATTACK = "DEFAULT ATTACK";

		float timeLastHit = 0f;
		GameObject weaponObject;
		CameraRaycaster cameraRayCaster;
		Animator animator;
		Enemy enemy;

		void Start(){
			RegisterInDelegates ();
			ChangeWeaponInHand (currentWeapon);
			SetAttackAnimation (currentWeapon);
			SetupAbilitiesBehaviour ();
		}
			
		void SetupAbilitiesBehaviour () {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities[abilityIndex].AttachAbilityTo (gameObject);
			}
		}

		void SetAttackAnimation(Weapon currentWeaponAnim){
			animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController [DEFAULT_ATTACK] = currentWeaponAnim.GetAttAnimClip ();
		}

		public void ChangeWeaponInHand (Weapon weaponToChange)
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

		void RegisterInDelegates ()
		{
			cameraRayCaster = FindObjectOfType<CameraRaycaster> ();
			cameraRayCaster.onMouseOverEnemy += MouseOverEnemy;
		}


		void Update() {
			var healthPercentage = GetComponent<HealthSystem> ().healthAsPercentage;
			if (healthPercentage > Mathf.Epsilon) {
				ScanForAbilityKeyDown ();
			}
		}

		void ScanForAbilityKeyDown(){
			for (int abilityIndex = 1; abilityIndex < abilities.Length; abilityIndex++) {
				if (Input.GetKeyDown(abilityIndex.ToString())){
					AttemptSpecialAbility (abilityIndex);
				}
			}
		}



		void MouseOverEnemy (Enemy enemyToSet){
			enemy = enemyToSet;
			if (Input.GetMouseButton (0) && IsTargetInRange (enemy.gameObject)) {
				AttackTarget ();
			} else if (Input.GetMouseButtonDown (1) && IsTargetInRange (enemy.gameObject)) {
				AttemptSpecialAbility (0);
			}
		}

		void AttemptSpecialAbility (int abilityNumber)
		{
			var energy = GetComponent<Energy> ();
			float energyCost = abilities [abilityNumber].GetEnergyCost ();

			if (energy.IsEnergyAvailable (energyCost)) {		//TODO Read from SO
				energy.ConsumeEnergy (energyCost);
				var paramsToUse = new AbilityUseParams(enemy, baseDamage);
				abilities [abilityNumber].Use (paramsToUse);
			}
		}

		void AttackTarget (){
			if (Time.time - timeLastHit > currentWeapon.GetTimeBetweenHits()) {
				SetAttackAnimation (currentWeapon);
				enemy.TakeDamage (CalculateDamage ());		// Weapon additional damage applies to normal attack only (For now)
				animator.SetTrigger (ATTACK_TRIGGER);
				timeLastHit = Time.time;
			}
		}

		float CalculateDamage ()
		{
			bool isCriticalHit = Random.Range (0f, 1.0f) <= criticalHitChance;
			float damageBeforeCritical = baseDamage + currentWeapon.GetAdditionalDamage ();
			if (isCriticalHit) {
				criticalHitParticle.Play ();
				return damageBeforeCritical * criticalHitMultiplier;
			} else {
				return damageBeforeCritical;
			}
		}

		bool IsTargetInRange (GameObject target){
			float distanceDiff = Vector3.Distance (transform.position, target.transform.position);
			return distanceDiff <= currentWeapon.GetMaxAttackRange();
		}
	}
}