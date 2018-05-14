using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility {

		PowerAttackConfig config;

		public void SetConfig(PowerAttackConfig configToSet){
			this.config = configToSet;
		}

		// Use this for initialization
		void Start () {
			Debug.Log ("Power Attack Behaviour is attached to " + gameObject.name);
		}
		
		// Update is called once per frame
		void Update () {
			
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
			Debug.Log ("Ability is used by " + gameObject.name);
			float damageToDeal = useParams.baseDamage + config.GetExtraDamage ();
			useParams.target.TakeDamage (damageToDeal);
		}
	}
}