using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters{
	[CreateAssetMenu(menuName=("RPG/Special Ability/Area Effect"))]
	public class AreaEffectConfig : AbilityConfig {
		
		[Header("Area Effect Config")]
		[SerializeField] float radius = 5f;
		[SerializeField] float damageEachTarget = 15f;

		public override AbilityBehaviour GetAbilityBehaviour (GameObject gameObjectToAttachTo){
			return gameObjectToAttachTo.AddComponent<AreaEffectBehaviour> ();
		}

		public float GetRadius () {
			return radius;
		}

		public float GetDamageEachTarget () {
			return damageEachTarget;
		}
	}
}
