using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[CreateAssetMenu(menuName=("RPG/Special Ability/Self Heal"))]
	public class SelfHealConfig : AbilityConfig {

		[Header("Self Heal Specifics")]
		[SerializeField] float healAmount = 50f;

		public override AbilityBehaviour GetAbilityBehaviour(GameObject gameObjectToAttachTo){
			return gameObjectToAttachTo.AddComponent<SelfHealBehaviour> ();
		}

		public float GetHealAmount () {
			return healAmount;
		}
	}
}
