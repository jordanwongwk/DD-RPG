using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters { 
	public class SelfHealBehaviour : AbilityBehaviour {

		PlayerControl player;

		void Start () {
			player = GetComponent<PlayerControl> ();
		}

		public override void Use(GameObject target) {
			var playerHealth = player.GetComponent<HealthSystem> ();
			playerHealth.Heal ((config as SelfHealConfig).GetHealAmount ());
			PlayParticleEffect ();
			PlayAbilitySound ();
			PlayAbilityAnimation ();
		}
	}
}