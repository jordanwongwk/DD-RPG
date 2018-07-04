using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehaviour : AbilityBehaviour {

		public override void Use(GameObject target) {
			DealingPowerDamage (target);
			PlayParticleEffect ();
			PlayAbilitySound ();
			PlayAbilityAnimation ();
		}

		void DealingPowerDamage (GameObject target)
		{
			transform.LookAt (target.transform.position);
			StartCoroutine (LookBackAtTarget (target));				// Forced character to lookback at target due to animation / navmesh offset
			float damageMultiplier = (config as PowerAttackConfig).GetDamageMultiplier ();
			float baseDamage = GetComponent<WeaponSystem> ().GetBaseDamage ();
			var weaponInHand = GetComponent<WeaponSystem> ().GetCurrentWeaponConfig ();
			float weaponDamage = weaponInHand.GetAdditionalDamage ();
			float damageToDeal = (weaponDamage * damageMultiplier) + baseDamage;

			target.GetComponent<HealthSystem>().TakeDamage (damageToDeal);
		}

		IEnumerator LookBackAtTarget (GameObject target) {			
			float animationClipTime = (config as PowerAttackConfig).GetAbilityAnimation ().length;
			yield return new WaitForSeconds (animationClipTime / 2);
			transform.LookAt (target.transform.position);
		}
	}
}