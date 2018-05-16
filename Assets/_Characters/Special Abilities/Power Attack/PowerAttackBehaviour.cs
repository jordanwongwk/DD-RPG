using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehaviour : AbilityBehaviour {

		public override void Use(AbilityUseParams useParams) {
			DealingPowerDamage (useParams);
			PlayParticleEffect ();
			PlayAbilitySound ();
		}

		void DealingPowerDamage (AbilityUseParams useParams)
		{
			float damageToDeal = useParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage ();
			useParams.target.TakeDamage (damageToDeal);
		}
	}
}