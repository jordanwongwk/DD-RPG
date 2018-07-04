using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public abstract class AbilityBehaviour : MonoBehaviour {

		protected AbilityConfig config;

		const float PARTICLE_EFFECT_DELAY = 5f;
		const string DEFAULT_ATTACK = "DEFAULT ATTACK";
		const string ATTACK_TRIGGER = "isAttacking";

		public abstract void Use (GameObject target = null);

		public void SetConfig(AbilityConfig configToSet){
			config = configToSet;
		}

		protected void PlayParticleEffect () {
			GameObject particleSystemPrefab = Instantiate (config.GetParticleSystem(), transform.position, Quaternion.identity);
			particleSystemPrefab.transform.parent = transform;
			particleSystemPrefab.GetComponent<ParticleSystem> ().Play ();
			StartCoroutine (DestroyParticleEffectAfterPlaying (particleSystemPrefab));
		}

		IEnumerator DestroyParticleEffectAfterPlaying (GameObject particlePrefab){
			while (particlePrefab.GetComponent<ParticleSystem> ().isPlaying) {
				yield return new WaitForSeconds (PARTICLE_EFFECT_DELAY);
			}
			Destroy (particlePrefab);
			yield return new WaitForEndOfFrame ();
		}

		protected void PlayAbilitySound() {
			var abilitySound = config.GetRandomAudioClip ();
			var audioSource = GetComponent<AudioSource> ();
			audioSource.PlayOneShot (abilitySound);
		}

		protected void PlayAbilityAnimation(){
			if (config.GetAbilityAnimation () != null) {
				var animatorOverrideController = GetComponent<Character> ().GetOverrideController ();
				var animator = GetComponent<Animator> ();
				animator.runtimeAnimatorController = animatorOverrideController;
				animatorOverrideController [DEFAULT_ATTACK] = config.GetAbilityAnimation ();
				animator.SetTrigger (ATTACK_TRIGGER);
			}
		}
	}
}
