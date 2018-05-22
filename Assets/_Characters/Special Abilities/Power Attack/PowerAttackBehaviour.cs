﻿using System.Collections;
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
			float damageToDeal = (config as PowerAttackConfig).GetExtraDamage ();
			target.GetComponent<HealthSystem>().TakeDamage (damageToDeal);
		}
	}
}