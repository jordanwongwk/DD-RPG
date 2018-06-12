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
			float damageMultiplier = (config as PowerAttackConfig).GetDamageMultiplier ();
			float baseDamage = GetComponent<WeaponSystem> ().GetBaseDamage ();
			var weaponInHand = GetComponent<WeaponSystem> ().GetCurrentWeaponConfig ();
			float weaponDamage = weaponInHand.GetAdditionalDamage ();
			float damageToDeal = (weaponDamage * damageMultiplier) + baseDamage;

			target.GetComponent<HealthSystem>().TakeDamage (damageToDeal);
		}
	}
}