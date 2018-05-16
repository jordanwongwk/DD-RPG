using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AreaEffectBehaviour : AbilityBehaviour {

		public override void Use(AbilityUseParams useParams) {
			DealingRadialDamage (useParams);
			PlayParticleEffect ();
			PlayAbilitySound ();
		}

		void DealingRadialDamage (AbilityUseParams useParams)
		{
			RaycastHit[] hits = Physics.SphereCastAll (
				transform.position, 
				(config as AreaEffectConfig).GetRadius (), 
				Vector3.up, 
				(config as AreaEffectConfig).GetRadius ()
			);
			foreach (RaycastHit hit in hits) {
				var damageable = hit.collider.gameObject.GetComponent<IDamageable> ();
				if (damageable != null && hit.collider.tag != gameObject.tag) {
					float damageToDeal = useParams.baseDamage + (config as AreaEffectConfig).GetDamageEachTarget ();
					damageable.TakeDamage (damageToDeal);
				}
			}
		}
	}
}
