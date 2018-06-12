using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AreaEffectBehaviour : AbilityBehaviour {
		
		public override void Use(GameObject target) {
			if ((config as AreaEffectConfig).GetRequiresChanneling () == true) {
				StartCoroutine (AbilityChanneling ());
			} else {
				ExecuteAbility ();
			}
		}

		IEnumerator AbilityChanneling(){
			float channelTime = (config as AreaEffectConfig).GetChannelTime ();
			var dangerCircle = (config as AreaEffectConfig).GetDangerCircle ();
			GameObject targetCircle = Instantiate (dangerCircle, transform.position, Quaternion.identity);
			targetCircle.transform.parent = gameObject.transform;

			yield return new WaitForSeconds (channelTime);
			Destroy (targetCircle);
			ExecuteAbility ();
		}

		void ExecuteAbility ()
		{
			DealingRadialDamage ();
			PlayParticleEffect ();
			PlayAbilitySound ();
			PlayAbilityAnimation ();
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
