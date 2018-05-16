using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[CreateAssetMenu(menuName=("RPG/Special Ability/Power Attack"))]
	public class PowerAttackConfig : AbilityConfig {

		[Header("Power Attack Specifics")]
		[SerializeField] float extraDamage = 10f;

		public override AbilityBehaviour GetAbilityBehaviour(GameObject gameObjectToAttachTo){
			return gameObjectToAttachTo.AddComponent<PowerAttackBehaviour> ();
		}

		public float GetExtraDamage () {
			return extraDamage;
		}
	}
}
