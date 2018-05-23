﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[CreateAssetMenu(menuName = "RPG/Weapon")]
	public class WeaponConfig : ScriptableObject {

		public GameObject gripTransform;

		[SerializeField] GameObject weaponPrefab = null;
		[SerializeField] AnimationClip weaponAnimation = null;
		[SerializeField] float timeBetweenHits = 0.5f;
		[SerializeField] float maxAttackRange = 1f;
		[SerializeField] float additionalDamage = 10f;
		[SerializeField] float damageDelay = 1f;
		[SerializeField] DominantHand dominantHand;

		public enum DominantHand
		{
			LeftHand,
			RightHand
		}

		public DominantHand GetDominantHand(){
			return dominantHand;
		}

		public float GetTimeBetweenHits (){
			return timeBetweenHits;
		}

		public float GetMaxAttackRange (){
			return maxAttackRange;
		}

		public GameObject GetWeaponPrefab(){
			return weaponPrefab;
		}

		public AnimationClip GetAttAnimClip(){
			RemoveAnimationEvent (); 
			return weaponAnimation;
		}

		public float GetAdditionalDamage(){
			return additionalDamage;
		}

		public float GetDamageDelay(){
			return damageDelay;
		}

		// Removing the asset pack's animation's event clip to prevent errors or bugs
		void RemoveAnimationEvent ()
		{
			weaponAnimation.events = new AnimationEvent[0];
		}
	}
}
