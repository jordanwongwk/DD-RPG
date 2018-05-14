using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters { 
	public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility {

		SelfHealConfig config;
		Player player;
		AudioSource audioSource;

		public void SetConfig(SelfHealConfig configToSet){
			this.config = configToSet;
		}

		// Use this for initialization
		void Start () {
			player = GetComponent<Player> ();
			audioSource = GetComponent<AudioSource> ();
		}

		public void Use(AbilityUseParams useParams) {
			player.Heal (config.GetHealAmount ());
			PlayParticleEffect ();
			audioSource.clip = config.GetAudioClip ();
			audioSource.Play ();
		}

		void PlayParticleEffect () {
			GameObject particleSystemPrefab = Instantiate (config.GetParticleSystem(), transform.position, Quaternion.identity);
			particleSystemPrefab.transform.parent = transform;
			ParticleSystem myParticleSystem = particleSystemPrefab.GetComponent<ParticleSystem> ();
			myParticleSystem.Play ();
			Destroy (particleSystemPrefab, myParticleSystem.main.duration);
		}
	}
}