using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility {

		PowerAttackConfig config;

		public void SetConfig(PowerAttackConfig configToSet){
			this.config = configToSet;
		}
			
		public void Use(AbilityUseParams useParams) {
			DealingPowerDamage (useParams);
			PlayParticleEffect ();
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
			useParams.target.AdjustHealth (damageToDeal);
		}
	}
}