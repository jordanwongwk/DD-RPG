﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[CreateAssetMenu(menuName=("RPG/Special Ability/Self Heal"))]
	public class SelfHealConfig : SpecialAbility {

		[Header("Self Heal Specifics")]
		[SerializeField] float healAmount = 50f;

		public override void AttachComponentTo(GameObject gameObjectToAttachTo){
			var behaviourComponent = gameObjectToAttachTo.AddComponent<SelfHealBehaviour> ();
			behaviourComponent.SetConfig (this);
			behaviour = behaviourComponent;
		}

		public float GetHealAmount () {
			return healAmount;
		}
	}
}
