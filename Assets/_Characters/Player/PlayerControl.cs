using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;

namespace RPG.Characters{
	public class PlayerControl : MonoBehaviour{
		CameraRaycaster cameraRayCaster;
		WeaponSystem weaponSystem;
		SpecialAbilities abilities;
		Character character;

		void Start(){
			abilities = GetComponent<SpecialAbilities> ();
			character = GetComponent<Character> ();
			weaponSystem = GetComponent<WeaponSystem> ();
			RegisterOnMouseEvents ();
		}
			
		void RegisterOnMouseEvents ()
		{
			cameraRayCaster = FindObjectOfType<CameraRaycaster> ();
			cameraRayCaster.onMouseOverEnemy += MouseOverEnemy;
			cameraRayCaster.onMouseOverWalkable += MouseOverWalkable;
		}

		void MouseOverEnemy (EnemyAI enemy){
			if (Input.GetMouseButton (0) && IsTargetInRange (enemy.gameObject)) {
				weaponSystem.AttackTarget (enemy.gameObject);
			} else if (Input.GetMouseButton (0) && !IsTargetInRange (enemy.gameObject)) {
				StartCoroutine (MoveAndAttack (enemy.gameObject));
			} else if (Input.GetMouseButtonDown (1) && IsTargetInRange (enemy.gameObject)) {
				abilities.AttemptSpecialAbility (0, enemy.gameObject);
			} else if (Input.GetMouseButtonDown (1) && !IsTargetInRange (enemy.gameObject)) {
				StartCoroutine (MoveAndSpecialAttack (enemy.gameObject));
			}
		}

		IEnumerator MoveToTarget (GameObject target){

			while (!IsTargetInRange (target)) {
				character.SetDestination (target.transform.position);
				yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator MoveAndAttack(GameObject enemy){
			yield return StartCoroutine (MoveToTarget (enemy));
			weaponSystem.AttackTarget (enemy);
		}

		IEnumerator MoveAndSpecialAttack(GameObject enemy){
			yield return StartCoroutine (MoveToTarget (enemy));
			abilities.AttemptSpecialAbility (0, enemy);
		}

		void MouseOverWalkable (Vector3 destination) {
			if (Input.GetMouseButton (0)) {
				character.SetDestination (destination);
			}
		}

		void Update() {
			ScanForAbilityKeyDown ();
		}

		void ScanForAbilityKeyDown(){
			for (int abilityIndex = 1; abilityIndex < abilities.GetAbilitiesLength(); abilityIndex++) {
				if (Input.GetKeyDown(abilityIndex.ToString())){
					abilities.AttemptSpecialAbility (abilityIndex);
				}
			}
		}

		bool IsTargetInRange (GameObject target){
			float distanceDiff = Vector3.Distance (transform.position, target.transform.position);
			return distanceDiff <= weaponSystem.GetCurrentWeaponConfig().GetMaxAttackRange();
		}
	}
}