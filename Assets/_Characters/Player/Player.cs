using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters{
	public class Player : MonoBehaviour, IDamageable{

		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] AudioClip[] damageSounds = null;
		[SerializeField] AudioClip[] deathSounds = null;

		// Serialized for debugging
		[SerializeField] SpecialAbility[] abilities = null;

		const string ATTACK_TRIGGER = "isAttacking";
		const string DEATH_TRIGGER = "isDead";
		const float DEATH_DELAY = 1.0f;

		float timeLastHit = 0f;
		float currentHealthPoints;
		CameraRaycaster cameraRayCaster;
		Animator animator;
		AudioSource audioSource;
		Enemy enemy;

		public float healthAsPercentage	{ get {	return currentHealthPoints / maxHealthPoints; }}

		void Start(){
			audioSource = GetComponent<AudioSource> ();

			SetMaxHealth ();
			RegisterInDelegates ();
			PutWeaponInHand ();
			SetupRuntimeAnimator ();
			SetupAbilitiesBehaviour ();
		}

		void SetupAbilitiesBehaviour () {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities[abilityIndex].AttachComponentTo (gameObject);
			}
		}

		void SetMaxHealth ()
		{
			currentHealthPoints = maxHealthPoints;
		}

		void SetupRuntimeAnimator(){
			animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController ["DEFAULT ATTACK"] = weaponInUse.GetAttAnimClip ();
		}

		void PutWeaponInHand ()
		{
			var weaponPrefab = weaponInUse.GetWeaponPrefab ();
			GameObject armToHoldWeapon = SelectDominantHand ();
			var usingWeapon = Instantiate(weaponPrefab, armToHoldWeapon.transform);
			usingWeapon.transform.localPosition = weaponInUse.gripTransform.transform.localPosition;
			usingWeapon.transform.localRotation = weaponInUse.gripTransform.transform.localRotation;
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
			if (healthAsPercentage > Mathf.Epsilon) {
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

		public void AdjustHealth (float changeAmount){
			bool playerDies = (currentHealthPoints - changeAmount <= 0); // must ask first
			ReduceHealth (changeAmount);
			if (playerDies) {	
				StartCoroutine (KillPlayer ());
			} 
		} 

		void ReduceHealth (float damage) {
			if (!audioSource.isPlaying && damage > 0) {
				audioSource.clip = damageSounds [Random.Range (0, damageSounds.Length)];
				audioSource.Play ();
			}
			currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
		}

		IEnumerator KillPlayer() {
			animator.SetTrigger (DEATH_TRIGGER);

			audioSource.clip = deathSounds [Random.Range (0, deathSounds.Length)];
			audioSource.Play ();
			yield return new WaitForSeconds (audioSource.clip.length + DEATH_DELAY);

			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
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
			if (Time.time - timeLastHit > weaponInUse.GetTimeBetweenHits()) {
				enemy.AdjustHealth (baseDamage);
				animator.SetTrigger (ATTACK_TRIGGER);
				timeLastHit = Time.time;
			}
		}

		bool IsTargetInRange (GameObject target){
			float distanceDiff = Vector3.Distance (transform.position, target.transform.position);
			return distanceDiff <= weaponInUse.GetMaxAttackRange();
		}
	}
}