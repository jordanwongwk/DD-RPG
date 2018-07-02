using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters{
	public class HealthSystem : MonoBehaviour {
		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float deathVanishInSeconds = 2.0f;
		[SerializeField] Image healthBar = null;
		[SerializeField] Text healthText = null;
		[SerializeField] AudioClip[] damageSounds = null;
		[SerializeField] AudioClip[] deathSounds = null;

		const string DEATH_TRIGGER = "isDead";
		const string REVIVE_TRIGGER = "isRevive";
		const float DEATH_DELAY = 1.0f;

		float currentHealthPoints;
		Animator animator;
		AudioSource audioSource;
		Character characterMovement;
		GameManager gameManager;
		PlayerDetectEnemy playerDetection;
		bool isCurrentlyDying = false;
		float regenAmount = 0f;

		public float healthAsPercentage	{ get {	return currentHealthPoints / maxHealthPoints; }}

		void Start () {
			animator = GetComponent<Animator> ();
			audioSource = GetComponent<AudioSource> ();
			characterMovement = GetComponent<Character> ();
			gameManager = FindObjectOfType<GameManager> ();
			playerDetection = FindObjectOfType<PlayerDetectEnemy> ();

			gameManager.onPlayerRespawn += SetRespawnAnimationAndHealth;
			currentHealthPoints = maxHealthPoints;
		}

		void Update () {
			UpdateHealthBar ();
			UpdateHealthText ();
			RegenHealth (regenAmount);			// TODO Consider a better alternative if laggy
		}

		void UpdateHealthBar(){
			if (healthBar) {
				healthBar.fillAmount = healthAsPercentage;
			}
		}

		void UpdateHealthText(){
			if (healthText) {
				healthText.text = Mathf.RoundToInt(currentHealthPoints) + " / " + maxHealthPoints;
			}
		}

		public void SetRegenAmount (float regenHealth){
			regenAmount = regenHealth;
		}

		public void SetRespawnAnimationAndHealth () {
			currentHealthPoints = maxHealthPoints;
		}

		public void TakeDamage (float damage){
			bool characterDies = (currentHealthPoints - damage <= 0);
			currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
			var clip = damageSounds [Random.Range (0, damageSounds.Length)];
			audioSource.PlayOneShot (clip);
			if (characterDies && !isCurrentlyDying) {
				isCurrentlyDying = true;	
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
			characterMovement.SetIsAlive (false);
			animator.SetTrigger (DEATH_TRIGGER);
			audioSource.clip = deathSounds [Random.Range (0, deathSounds.Length)];
			audioSource.Play ();	// Don't use PlayOneShot, we need this clip to override the current clip

			var playerComponent = GetComponent<PlayerControl> ();
			if (playerComponent && playerComponent.isActiveAndEnabled) {	// Relying on lazy evaluation
				yield return new WaitForSeconds (audioSource.clip.length + DEATH_DELAY);
				playerDetection.ResettingSelectedEnemyAndIndicator ();
				playerComponent.SetPlayerDeathCount ();
				animator.SetTrigger (REVIVE_TRIGGER);
				gameManager.StartRespawnDelegates ();
				gameManager.RestartBossBattleUponDeath ();
				isCurrentlyDying = false;
			}
			else //consider changing this if NPC is involved, otherwise it is assumed to be enemies 
			{
				GetComponent<CapsuleCollider> ().enabled = false;
				GetComponent<EnemyAI> ().EnemyStopAllAction ();
				yield return new WaitForSeconds (deathVanishInSeconds);
				playerDetection.SetIndicatorOffStillTargetted (this.gameObject);
				isCurrentlyDying = false;
				if (gameObject.tag == "Boss") {
					Destroy (gameObject);
				} else {
					gameObject.SetActive (false);
				}
			}
		}
	}
}
