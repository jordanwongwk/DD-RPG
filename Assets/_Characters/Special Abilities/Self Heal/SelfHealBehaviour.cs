using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters { 
	public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility {

		SelfHealConfig config;
		Player player;

		public void SetConfig(SelfHealConfig configToSet){
			this.config = configToSet;
		}

		// Use this for initialization
		void Start () {
			player = GetComponent<Player> ();
		}

		public void Use(AbilityUseParams useParams) {
			player.AdjustHealth (-config.GetHealAmount ());
		}
	}
}