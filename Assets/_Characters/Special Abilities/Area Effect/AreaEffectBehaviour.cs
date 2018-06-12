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
			Vector3 circlePostion = new Vector3 (transform.position.x, (transform.position.y + 0.25f), transform.position.z);
			GameObject targetCircle = Instantiate (dangerCircle, circlePostion, Quaternion.identity);
			targetCircle.transform.parent = gameObject.transform;

			GameObject particleSystemPrefab = null;
			if (config.GetChannelParticle () != null) {
				particleSystemPrefab = Instantiate (config.GetChannelParticle (), transform.position, Quaternion.identity);
				particleSystemPrefab.transform.parent = transform;
				particleSystemPrefab.GetComponent<ParticleSystem> ().Play ();
			}

			yield return new WaitForSeconds (channelTime);
			Destroy (targetCircle);
			Destroy (particleSystemPrefab);
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
