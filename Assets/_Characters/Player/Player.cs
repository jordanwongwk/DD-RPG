using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Core;

namespace RPG.Characters{
	public class Player : MonoBehaviour, IDamageable{

		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon currentWeapon = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] AudioClip[] damageSounds = null;
		[SerializeField] AudioClip[] deathSounds = null;
		[Range (0.1f, 1.0f)][SerializeField] float criticalHitChance = 0.1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;
		[SerializeField] ParticleSystem criticalHitParticle = null;

		// Serialized for debugging
		[SerializeField] AbilityConfig[] abilities = null;

		const string ATTACK_TRIGGER = "isAttacking";
		const string DEATH_TRIGGER = "isDead";
		const string DEFAULT_ATTACK = "DEFAULT ATTACK";
		const float DEATH_DELAY = 1.0f;

		GameObject weaponObject;
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
			ChangeWeaponInHand (currentWeapon);
			SetAttackAnimation (currentWeapon);
			SetupAbilitiesBehaviour ();
		}
			
		void SetupAbilitiesBehaviour () {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities[abilityIndex].AttachAbilityTo (gameObject);
			}
		}

		void SetMaxHealth ()
		{
			currentHealthPoints = maxHealthPoints;
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

		public void TakeDamage (float damage){
			currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
			if (!audioSource.isPlaying) {
				audioSource.clip = damageSounds [Random.Range (0, damageSounds.Length)];
				audioSource.Play ();
			}
			if (currentHealthPoints <= 0) {	
				StartCoroutine (KillPlayer ());
			} 
		} 

		public void Heal (float amount) {
			currentHealthPoints = Mathf.Clamp (currentHealthPoints + amount, 0f, maxHealthPoints);
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