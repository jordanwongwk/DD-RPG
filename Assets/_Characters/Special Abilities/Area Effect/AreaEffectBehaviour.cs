using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AreaEffectBehaviour : AbilityBehaviour {

		public override void Use(GameObject target) {
			DealingRadialDamage ();
			PlayParticleEffect ();
			PlayAbilitySound ();
		}

		void DealingRadialDamage ()
		{
			RaycastHit[] hits = Physics.SphereCastAll (
				transform.position, 
				(config as AreaEffectConfig).GetRadius (), 
				Vector3.up, 
				(config as AreaEffectConfig).GetRadius ()
			);
			foreach (RaycastHit hit in hits) {
				var damageable = hit.collider.gameObject.GetComponent<HealthSystem> ();
				if (damageable != null && hit.collider.tag != gameObject.tag) {
					float damageToDeal = (config as AreaEffectConfig).GetDamageEachTarget ();
					damageable.TakeDamage (damageToDeal);
				}
			}
		}
	}
}
