﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.ThirdPerson;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters{
	public class Player : MonoBehaviour, IDamageable{

		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] float playerMeleeDamage = 10f;
		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		float timeLastHit;
		float currentHealthPoints;
		CameraRaycaster cameraRayCaster;
		Animator animator;

		public float healthAsPercentage	{ get {	return currentHealthPoints / maxHealthPoints; }}

		void Start(){
			SetMaxHealth ();
			RegisterInDelegates ();
			PutWeaponInHand ();
			SetupRuntimeAnimator ();
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
			currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
		} 

		void MouseOverEnemy (Enemy enemy){
			if (Input.GetMouseButton (0) && IsTargetInRange (enemy.gameObject)) {
				AttackTarget (enemy);
			}
		}

		void AttackTarget (Enemy enemy){
			if (Time.time - timeLastHit > weaponInUse.GetTimeBetweenHits()) {
				enemy.TakeDamage (playerMeleeDamage);
				animator.SetTrigger ("isAttacking");
				timeLastHit = Time.time;
			}
		}

		bool IsTargetInRange (GameObject target){
			float distanceDiff = Vector3.Distance (transform.position, target.transform.position);
			return distanceDiff <= weaponInUse.GetMaxAttackRange();
		}
	}
}