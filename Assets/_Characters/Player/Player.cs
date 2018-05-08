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

		float timeLastHit;
		float currentHealthPoints;
		CameraRaycaster cameraRayCaster;
		Animator animator;
		AudioSource audioSource;

		public float healthAsPercentage	{ get {	return currentHealthPoints / maxHealthPoints; }}

		void Start(){
			SetMaxHealth ();
			RegisterInDelegates ();
			PutWeaponInHand ();
			SetupRuntimeAnimator ();
			audioSource = GetComponent<AudioSource> ();
			abilities[0].AttachComponentTo (gameObject);
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

		public void TakeDamage (float damage){
			bool playerDies = (currentHealthPoints - damage <= 0); // must ask first
			ReduceHealth (damage);
			if (playerDies) {	
				StartCoroutine (KillPlayer ());
			} 
		} 

		void ReduceHealth (float damage) {
			if (!audioSource.isPlaying) {
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

		void MouseOverEnemy (Enemy enemy){
			if (Input.GetMouseButton (0) && IsTargetInRange (enemy.gameObject)) {
				AttackTarget (enemy);
			} else if (Input.GetMouseButtonDown (1) && IsTargetInRange (enemy.gameObject)) {
				AttemptSpecialAbility (0, enemy);
			}
		}

		void AttemptSpecialAbility (int abilityNumber, Enemy enemy)
		{
			var energy = GetComponent<Energy> ();
			float energyCost = abilities [abilityNumber].GetEnergyCost ();

			if (energy.IsEnergyAvailable (energyCost)) {		//TODO Read from SO
				energy.ConsumeEnergy (energyCost);
				var paramsToUse = new AbilityUseParams(enemy, baseDamage);
				abilities [abilityNumber].Use (paramsToUse);
			}
		}

		void AttackTarget (Enemy enemy){
			if (Time.time - timeLastHit > weaponInUse.GetTimeBetweenHits()) {
				enemy.TakeDamage (baseDamage);
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