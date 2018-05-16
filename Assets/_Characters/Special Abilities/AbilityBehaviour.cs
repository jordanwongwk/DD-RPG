using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public abstract class AbilityBehaviour : MonoBehaviour {

		protected AbilityConfig config;

		const float PARTICLE_EFFECT_DELAY = 10f;

		public abstract void Use (AbilityUseParams useParams);

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
	}
}
