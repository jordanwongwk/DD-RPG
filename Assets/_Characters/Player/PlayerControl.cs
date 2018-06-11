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
		HealthSystem healthSystem;
		bool isPlayerStillAlive = true;	
		bool isPlayerFreeToMove = true;
		bool inBossBattle = false;
		float playerStopDistance;

		const float TARGET_OFFSET = 0.25f;
		const float BOSS_ENCOUNTER_DIST = 15f;

		void Start(){
			abilities = GetComponent<SpecialAbilities> ();
			character = GetComponent<Character> ();
			weaponSystem = GetComponent<WeaponSystem> ();
			gameManager = FindObjectOfType<GameManager> ();
			playerStopDistance = character.GetStoppingDistance ();
			RegisterOnMouseEvents ();
		}
			
		void RegisterOnMouseEvents ()
		{
			cameraRayCaster = FindObjectOfType<CameraRaycaster> ();
			cameraRayCaster.onMouseOverEnemy += MouseOverEnemy;
			cameraRayCaster.onMouseOverWalkable += MouseOverWalkable;

			gameManager.onPlayerRespawn += OnPlayerRespawn;
		}

		void MouseOverEnemy (EnemyAI enemy){
			if (isPlayerStillAlive && isPlayerFreeToMove) {
				if (Input.GetMouseButtonDown (0) && IsTargetInRange (enemy.gameObject)) {
					weaponSystem.AttackTarget (enemy.gameObject);
				} else if (Input.GetMouseButtonDown (0) && !IsTargetInRange (enemy.gameObject)) {
					StartCoroutine (MoveAndAttack (enemy.gameObject));
				} else if (Input.GetMouseButtonDown (1) && IsTargetInRange (enemy.gameObject)) {
					abilities.AttemptSpecialAbility (0, enemy.gameObject);
				} else if (Input.GetMouseButtonDown (1) && !IsTargetInRange (enemy.gameObject)) {
					StartCoroutine (MoveAndSpecialAttack (enemy.gameObject));
				}
			}
		}

		IEnumerator MoveToTarget (GameObject target){
			while (!IsTargetInRange (target)) {
				MovingTargetIndication (target);
				character.SetDestination (target.transform.position);
				yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator MoveAndAttack(GameObject enemy){
			yield return StartCoroutine (MoveToTarget (enemy));
			character.SetStoppingDistance (playerStopDistance);				// Leave some distance for combat
			MovingTargetIndication (enemy);
			weaponSystem.AttackTarget (enemy);
		}

		IEnumerator MoveAndSpecialAttack(GameObject enemy){
			yield return StartCoroutine (MoveToTarget (enemy));
			character.SetStoppingDistance (playerStopDistance);
			MovingTargetIndication (enemy);
			abilities.AttemptSpecialAbility (0, enemy);
		}

		void MovingTargetIndication (GameObject target){
			targetIndicator.transform.position = target.transform.position;
		}

		void MouseOverWalkable (Vector3 destination) {
			if (Input.GetMouseButton (0) && isPlayerStillAlive && isPlayerFreeToMove) {
				StopAllCoroutines ();
				targetIndicator.transform.position = destination;
				character.SetStoppingDistance (TARGET_OFFSET); 				// So that the character move closer to the target center point
				character.SetDestination (destination);
			}
		}

		void Update() {
			isPlayerStillAlive = GetComponent<HealthSystem> ().healthAsPercentage >= Mathf.Epsilon;
			ScanForAbilityKeyDown ();
			ScanForBoss ();
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

		void OnPlayerRespawn(){
			if (inBossBattle) {
				inBossBattle = false;
			}
			character.PlayerRespawnSetup ();
		}
	}
}