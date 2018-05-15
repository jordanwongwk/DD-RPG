using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehaviour : AbilityBehaviour {

		PowerAttackConfig config;
		AudioSource audioSource;

		public void SetConfig(PowerAttackConfig configToSet){
			this.config = configToSet;
		}

		void Start() {
			audioSource = GetComponent<AudioSource> ();
		}
			
		public override void Use(AbilityUseParams useParams) {
			DealingPowerDamage (useParams);
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

		void DealingPowerDamage (AbilityUseParams useParams)
		{
			float damageToDeal = useParams.baseDamage + config.GetExtraDamage ();
			useParams.target.TakeDamage (damageToDeal);
		}
	}
}