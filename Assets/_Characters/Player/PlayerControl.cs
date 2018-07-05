using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;

namespace RPG.Characters{
	public class PlayerControl : MonoBehaviour{
		[SerializeField] GameObject targetIndicator = null;
		CameraRaycaster cameraRayCaster;
		WeaponSystem weaponSystem;
		SpecialAbilities abilities;
		Character character;
		GameManager gameManager;
		CheckpointManager checkpointManager;
		HealthSystem healthSystem;
		PlayerDetectEnemy playerDetection;
		bool isPlayerStillAlive = true;	
		bool isPlayerFreeToMove = true;
		bool isPlayerInRespawnProcess = false;
		bool inBossBattle = false;
		float playerStopDistanceForAttacking;
		int playerDeathCount = 0;

		Vector3 testing;

		const float TARGET_OFFSET = 0.25f;
		const float BOSS_ENCOUNTER_DIST = 15f;
		const float INDICATION_APPEAR_TIME = 1.5f;

		void Start(){
			abilities = GetComponent<SpecialAbilities> ();
			character = GetComponent<Character> ();
			weaponSystem = GetComponent<WeaponSystem> ();
			gameManager = FindObjectOfType<GameManager> ();
			checkpointManager = FindObjectOfType<CheckpointManager> ();
			playerDetection = GetComponentInChildren<PlayerDetectEnemy> ();
			RegisterOnMouseEvents ();
		}

		public void SetPlayerStopDistanceToAttack (float currentWeaponRange) {
			playerStopDistanceForAttacking = currentWeaponRange;
		}
			
		void RegisterOnMouseEvents ()
		{
			cameraRayCaster = FindObjectOfType<CameraRaycaster> ();
			cameraRayCaster.onMouseOverEnemy += MouseOverEnemy;
			cameraRayCaster.onMouseOverWalkable += MouseOverWalkable;

			gameManager.onPlayerRespawn += OnPlayerRespawn;
			gameManager.endGameSetup += GetPlayerDeathCount;
		}

		void MouseOverEnemy (EnemyAI enemy){
			if (isPlayerStillAlive && isPlayerFreeToMove && !isPlayerInRespawnProcess) {
				if (Input.GetMouseButtonDown (0) && IsTargetInRange (enemy.gameObject)) 
				{
					playerDetection.SetEnemyTarget (enemy.gameObject);
					InterruptWalkingAction (enemy.gameObject);
					weaponSystem.AttackTarget (enemy.gameObject);
				} 
				else if (Input.GetMouseButtonDown (0) && !IsTargetInRange (enemy.gameObject)) 
				{
					playerDetection.SetEnemyTarget (enemy.gameObject);
					StartCoroutine (MoveAndAttack (enemy.gameObject));
				} 
				else if (Input.GetMouseButtonDown (1) && IsTargetInRange (enemy.gameObject)) 
				{
					playerDetection.SetEnemyTarget (enemy.gameObject);
					InterruptWalkingAction (enemy.gameObject);
					abilities.AttemptSpecialAbility (0, enemy.gameObject);
				} 
				else if (Input.GetMouseButtonDown (1) && !IsTargetInRange (enemy.gameObject)) 
				{
					playerDetection.SetEnemyTarget (enemy.gameObject);
					StartCoroutine (MoveAndSpecialAttack (enemy.gameObject));
				}
			}
		}

		IEnumerator MoveToTarget (GameObject target){
			while (!IsTargetInRange (target)) {
				StartCoroutine (MovingTargetIndication (target));
				character.SetDestination (target.transform.position);
				yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator MoveAndAttack(GameObject enemy){
			yield return StartCoroutine (MoveToTarget (enemy));
			character.SetStoppingDistance (playerStopDistanceForAttacking);				// Leave some distance for combat
			weaponSystem.AttackTarget (enemy);
		}

		IEnumerator MoveAndSpecialAttack(GameObject enemy){
			yield return StartCoroutine (MoveToTarget (enemy));
			character.SetStoppingDistance (playerStopDistanceForAttacking);
			StartCoroutine (MovingTargetIndication (enemy));
			abilities.AttemptSpecialAbility (0, enemy);
		}

		IEnumerator MovingTargetIndication (GameObject target){
			targetIndicator.SetActive (true);
			targetIndicator.transform.position = target.transform.position;
			yield return new WaitForSeconds (INDICATION_APPEAR_TIME);
			targetIndicator.SetActive (false);
		}

		// When prompt to attack nearby enemy, force player to stop its previous order to walk to the previous destination
		void InterruptWalkingAction (GameObject newTarget){
			StopAllCoroutines ();
			character.SetStoppingDistance (playerStopDistanceForAttacking);
			character.SetDestination (newTarget.transform.position);
		}

		void MouseOverWalkable (Vector3 destination) {
			if (Input.GetMouseButton (0) && isPlayerStillAlive && isPlayerFreeToMove && !isPlayerInRespawnProcess) {
				StopAllCoroutines ();
				StartCoroutine (MovingDestinationIndication (destination));
				character.SetStoppingDistance (TARGET_OFFSET); 				// So that the character move closer to the target center point
				character.SetDestination (destination);
			}
		}

		IEnumerator MovingDestinationIndication (Vector3 destination)
		{
			targetIndicator.SetActive (true);
			targetIndicator.transform.position = destination;
			yield return new WaitForSeconds (INDICATION_APPEAR_TIME);
			targetIndicator.SetActive (false);
		}

		void Update() {
			isPlayerStillAlive = GetComponent<HealthSystem> ().healthAsPercentage >= Mathf.Epsilon;
			TargettingEnemy ();
			ScanForBoss ();

			if (isPlayerStillAlive && isPlayerFreeToMove && !isPlayerInRespawnProcess) {
				ScanForAutoAttack ();
				ScanForAbilityKeyDown ();
			}
		}
			
		void TargettingEnemy() {
			if (Input.GetKeyDown (KeyCode.Tab)) {
				playerDetection.SelectingEnemyTarget ();
			}
		}

		void ScanForAutoAttack(){
			if (Input.GetKeyDown (KeyCode.Space)) {
				var selectedEnemy = playerDetection.GetEnemyTarget ();
				if (selectedEnemy != null) {
					if (IsTargetInRange (selectedEnemy)) {
						InterruptWalkingAction (selectedEnemy);
						weaponSystem.AttackTarget (selectedEnemy);
					} else if (!IsTargetInRange (selectedEnemy)) {
						StartCoroutine (MoveAndAttack (selectedEnemy));
					}
				}
			}
		}

		void ScanForAbilityKeyDown(){
			for (int abilityIndex = 1; abilityIndex < abilities.GetAbilitiesLength(); abilityIndex++) {
				if (Input.GetKeyDown(abilityIndex.ToString())){
					abilities.AttemptSpecialAbility (abilityIndex);
				}
			}
		}

		void ScanForBoss(){
			var boss = GameObject.FindGameObjectWithTag ("Boss");

			if (boss != null) {
				float bossDistance = Vector3.Distance (transform.position, boss.transform.position);
				if (bossDistance <= BOSS_ENCOUNTER_DIST && !inBossBattle) {
					gameManager.TriggerBossBattleDelegate ();
					inBossBattle = true;
				} 
			}

			if (boss == null && inBossBattle) {
				gameManager.TriggerBossEndDelegate ();
				inBossBattle = false;
			}
		}

		bool IsTargetInRange (GameObject target){
			float distanceDiff = Vector3.Distance (transform.position, target.transform.position);
			return distanceDiff <= weaponSystem.GetCurrentWeaponConfig().GetMaxAttackRange();
		}

		public void SetPlayerFreeToMove(bool permission){
			isPlayerFreeToMove = permission;
		}

		public void SetPlayerIsRespawning(bool permission){
			isPlayerInRespawnProcess = permission;
		}

		void OnPlayerRespawn(){
			var lastEquippedWeapon = checkpointManager.GetLastEquippedWeapon ();

			if (lastEquippedWeapon != null) {
				weaponSystem.ChangeWeaponInHand (lastEquippedWeapon);
			}

			if (inBossBattle) {
				inBossBattle = false;
			}

			character.PlayerRespawnSetup ();
		}

		public void SetPlayerDeathCount(){
			playerDeathCount += 1;
		}

		void GetPlayerDeathCount(){
			PlayerPrefManager.SetDeathNumber (playerDeathCount);
		}
	}
}