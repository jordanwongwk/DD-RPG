using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[CreateAssetMenu(menuName = "RPG/Weapon")]
	public class WeaponConfig : ScriptableObject {

		public GameObject gripTransform;

		[SerializeField] GameObject weaponPrefab = null;
		[SerializeField] AnimationClip weaponAnimation = null;
		[SerializeField] AudioClip weaponSFX = null;
		[SerializeField] float timeBetweenHits = 0.5f;
		[SerializeField] float maxAttackRange = 1f;
		[SerializeField] float additionalDamage = 10f;
		[SerializeField] float damageDelay = 1f;

		[Header("Projectile Settings")]
		[SerializeField] GameObject weaponProjectile = null;
		[SerializeField] AudioClip impactSFX = null;
		[SerializeField] float projectileDamage = 10f;
		[SerializeField] float projectileSpeed = 1.0f;
		[SerializeField] float firingDelay = 0.5f;

		public float GetFiringDelay(){
			return firingDelay;
		}

		public float GetProjectileDamage(){
			return projectileDamage;
		}

		public float GetProjectileSpeed(){
			return projectileSpeed;
		}

		public AudioClip GetImpactSFX(){
			return impactSFX;
		}

		public GameObject GetWeaponProjectile(){
			return weaponProjectile;
		}

		public float GetTimeBetweenHits (){
			return timeBetweenHits;
		}

		public float GetMaxAttackRange (){
			return maxAttackRange;
		}

		public AudioClip GetWeaponSFX(){
			return weaponSFX;
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
