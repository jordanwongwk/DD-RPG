using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;

namespace RPG.Characters{
	public class PlayerMovement : MonoBehaviour{
		[Range (0.1f, 1.0f)][SerializeField] float criticalHitChance = 0.1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;
		[SerializeField] ParticleSystem criticalHitParticle = null;

		CameraRaycaster cameraRayCaster;
		WeaponSystem weaponSystem;
		Enemy enemy;
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

		void MouseOverEnemy (Enemy enemyToSet){
			enemy = enemyToSet;
			if (Input.GetMouseButton (0) && IsTargetInRange (enemy.gameObject)) {
				weaponSystem.AttackTarget (enemy.gameObject);
			} else if (Input.GetMouseButtonDown (1) && IsTargetInRange (enemy.gameObject)) {
				abilities.AttemptSpecialAbility (0, enemy.gameObject);
			}
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