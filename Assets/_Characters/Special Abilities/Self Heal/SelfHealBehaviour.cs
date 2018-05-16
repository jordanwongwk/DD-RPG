using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters { 
	public class SelfHealBehaviour : AbilityBehaviour {

		Player player;
		AudioSource audioSource;

		void Start () {
			player = GetComponent<Player> ();
			audioSource = GetComponent<AudioSource> ();
		}

		public override void Use(AbilityUseParams useParams) {
			player.Heal ((config as SelfHealConfig).GetHealAmount ());
			PlayParticleEffect ();
			audioSource.clip = config.GetAudioClip ();
			audioSource.Play ();
		}
	}
}