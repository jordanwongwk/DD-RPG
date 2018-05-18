using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters { 
	public class SelfHealBehaviour : AbilityBehaviour {

		Player player;

		void Start () {
			player = GetComponent<Player> ();
		}

		public override void Use(GameObject target) {
			var playerHealth = player.GetComponent<HealthSystem> ();
			playerHealth.Heal ((config as SelfHealConfig).GetHealAmount ());
			PlayParticleEffect ();
			PlayAbilitySound ();
		}
	}
}