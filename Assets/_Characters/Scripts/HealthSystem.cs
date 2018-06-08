﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters{
	public class HealthSystem : MonoBehaviour {
		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float deathVanishInSeconds = 2.0f;
		[SerializeField] Image healthBar = null;
		[SerializeField] AudioClip[] damageSounds = null;
		[SerializeField] AudioClip[] deathSounds = null;

		const string DEATH_TRIGGER = "isDead";
		const float DEATH_DELAY = 1.0f;

		float currentHealthPoints;
		Animator animator;
		AudioSource audioSource;
		Character characterMovement;
		float regenAmount = 0f;
		[SerializeField] bool enemyBrinkOfDeath = false;

		public float healthAsPercentage	{ get {	return currentHealthPoints / maxHealthPoints; }}

		void Start () {
			animator = GetComponent<Animator> ();
			audioSource = GetComponent<AudioSource> ();
			characterMovement = GetComponent<Character> ();

			currentHealthPoints = maxHealthPoints;
		}

		void Update () {
			UpdateHealthBar ();
			RegenHealth (regenAmount);			// TODO Consider a better alternative if laggy
		}

		void UpdateHealthBar(){
			if (healthBar) {
				healthBar.fillAmount = healthAsPercentage;
			}
		}

		public void SetRegenAmount (float regenHealth){
			regenAmount = regenHealth;
		}

		public void SetRespawnFullHealth () {
			currentHealthPoints = maxHealthPoints;
		}

		public void TakeDamage (float damage){
			bool characterDies = (currentHealthPoints - damage <= 0);
			currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
			var clip = damageSounds [Random.Range (0, damageSounds.Length)];
			audioSource.PlayOneShot (clip);
			if (characterDies) {	
				StartCoroutine (KillCharacter ());
			} 
		} 

		public void Heal (float amount) {
			currentHealthPoints = Mathf.Clamp (currentHealthPoints + amount, 0f, maxHealthPoints);
		}

		void RegenHealth (float amount) {
			float pointsToAdd = amount * Time.deltaTime;
			Heal (pointsToAdd);
		}

		IEnumerator KillCharacter() {
			characterMovement.Kill ();
			animator.SetTrigger (DEATH_TRIGGER);
			audioSource.clip = deathSounds [Random.Range (0, deathSounds.Length)];
			audioSource.Play ();	// Don't use PlayOneShot, we need this clip to override the current clip

			var playerComponent = GetComponent<PlayerControl> ();
			if (playerComponent && playerComponent.isActiveAndEnabled) {	// Relying on lazy evaluation
				yield return new WaitForSeconds (audioSource.clip.length + DEATH_DELAY);
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			}
			else //consider changing this if NPC is involved, otherwise it is assumed to be enemies 
			{
				GetComponent<CapsuleCollider> ().enabled = false;
				yield return new WaitForSeconds (deathVanishInSeconds);
				gameObject.SetActive (false);
				enemyBrinkOfDeath = true;
//				DestroyObject (gameObject, deathVanishInSeconds);
			}
		}

		public bool GetBrinkOfDeathStatus (){
			return enemyBrinkOfDeath;
		}
	}
}
