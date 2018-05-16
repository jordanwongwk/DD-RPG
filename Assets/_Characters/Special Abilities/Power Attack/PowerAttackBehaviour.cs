using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehaviour : AbilityBehaviour {

		AudioSource audioSource;

		void Start() {
			audioSource = GetComponent<AudioSource> ();
		}
			
		public override void Use(AbilityUseParams useParams) {
			DealingPowerDamage (useParams);
			PlayParticleEffect ();
			audioSource.clip = config.GetAudioClip ();
			audioSource.Play ();
		}

		void DealingPowerDamage (AbilityUseParams useParams)
		{
			float damageToDeal = useParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage ();
			useParams.target.TakeDamage (damageToDeal);
		}
	}
}