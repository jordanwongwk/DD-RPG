using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AreaEffectBehaviour : MonoBehaviour, ISpecialAbility {

		AreaEffectConfig config;

		public void SetConfig(AreaEffectConfig configToSet){
			this.config = configToSet;
		}

		public void Use(AbilityUseParams useParams) {
			DealingRadialDamage (useParams);
			PlayParticleEffect ();
		}

		void PlayParticleEffect () {
			GameObject particleSystemPrefab = Instantiate (config.GetParticleSystem(), transform.position, Quaternion.identity);
			ParticleSystem myParticleSystem = particleSystemPrefab.GetComponent<ParticleSystem> ();
			myParticleSystem.Play ();
			Destroy (particleSystemPrefab, myParticleSystem.main.duration);
		}

		void DealingRadialDamage (AbilityUseParams useParams)
		{
			RaycastHit[] hits = Physics.SphereCastAll (transform.position, config.GetRadius (), Vector3.up, config.GetRadius ());
			foreach (RaycastHit hit in hits) {
				var damageable = hit.collider.gameObject.GetComponent<IDamageable> ();
				if (damageable != null && hit.collider.tag != gameObject.tag) {
					float damageToDeal = useParams.baseDamage + config.GetDamageEachTarget ();
					damageable.AdjustHealth (damageToDeal);
				}
			}
		}
	}
}
