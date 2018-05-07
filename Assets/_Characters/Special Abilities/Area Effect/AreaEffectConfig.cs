using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters{
	[CreateAssetMenu(menuName=("RPG/Special Ability/Area Effect"))]
	public class AreaEffectConfig : SpecialAbility {
		
		[Header("Area Effect Config")]
		[SerializeField] float radius = 5f;
		[SerializeField] float damageEachTarget = 15f;

		public override void AttachComponentTo (GameObject gameObjectToAttachTo){
			var behaviourComponent = gameObjectToAttachTo.AddComponent<AreaEffectBehaviour> ();
			behaviourComponent.SetConfig (this);
			behaviour = behaviourComponent;
		}

		public float GetRadius () {
			return radius;
		}

		public float GetDamageEachTarget () {
			return damageEachTarget;
		}
	}
}
