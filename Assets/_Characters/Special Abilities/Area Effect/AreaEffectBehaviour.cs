using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AreaEffectBehaviour : AbilityBehaviour {

		AreaEffectConfig config;
		AudioSource audioSource;

		public void SetConfig(AreaEffectConfig configToSet){
			this.config = configToSet;
		}

		void Start() {
			audioSource = GetComponent<AudioSource> ();
		}

		public override void Use(AbilityUseParams useParams) {
			DealingRadialDamage (useParams);
			PlayParticleEffect ();
			audioSource.clip = config.GetAudioClip ();
			audioSource.Play ();
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
					damageable.TakeDamage (damageToDeal);
				}
			}
		}
	}
}
