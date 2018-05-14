using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public struct AbilityUseParams {
		public IDamageable target;
		public float baseDamage;

		public AbilityUseParams (IDamageable aTarget, float aBaseDamage){
			this.target = aTarget;
			this.baseDamage = aBaseDamage;
		}
	}

	public abstract class AbilityConfig : ScriptableObject {

		[Header("Special Attack General")]
		[SerializeField] float energyCost = 10f;
		[SerializeField] GameObject particleSystem = null;
		[SerializeField] AudioClip audioClip = null;

		protected ISpecialAbility behaviour;

		abstract public void AttachComponentTo(GameObject gameObjectToAttachTo);

		public void Use(AbilityUseParams useParams) {
			behaviour.Use (useParams);
		}

		public float GetEnergyCost () {
			return energyCost;
		}

		public GameObject GetParticleSystem() {
			return particleSystem;
		}

		public AudioClip GetAudioClip() {
			return audioClip;
		}
	}

	public interface ISpecialAbility 
	{
		void Use(AbilityUseParams useParams);	
	}
}