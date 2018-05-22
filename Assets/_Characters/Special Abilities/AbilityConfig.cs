using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public abstract class AbilityConfig : ScriptableObject {

		[Header("Special Attack General")]
		[SerializeField] float energyCost = 10f;
		[SerializeField] GameObject particleSystem = null;
		[SerializeField] AudioClip[] audioClips = null;
		[SerializeField] AnimationClip abilityAnimation = null;

		protected AbilityBehaviour behaviour;

		public abstract AbilityBehaviour GetAbilityBehaviour (GameObject gameObjectToAttachTo);

		public void AttachAbilityTo (GameObject gameObjectToAttachTo){
			var behaviourComponent = GetAbilityBehaviour (gameObjectToAttachTo);
			behaviourComponent.SetConfig (this);
			behaviour = behaviourComponent;
		}

		public void Use(GameObject target) {
			behaviour.Use (target);
		}
			
		public float GetEnergyCost () {
			return energyCost;
		}

		public GameObject GetParticleSystem() {
			return particleSystem;
		}

		public AudioClip GetRandomAudioClip() {
			return audioClips[Random.Range(0, audioClips.Length)];
		}

		public AnimationClip GetAbilityAnimation(){
			RemoveAnimationEvent ();
			return abilityAnimation;
		}

		// Removing the asset pack's animation's event clip to prevent errors or bugs
		void RemoveAnimationEvent ()
		{
			abilityAnimation.events = new AnimationEvent[0];
		}
	}
}