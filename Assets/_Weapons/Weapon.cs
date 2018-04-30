using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons {
	[CreateAssetMenu(menuName = "RPG/Weapon")]
	public class Weapon : ScriptableObject {

		public GameObject gripTransform;

		[SerializeField] GameObject weaponPrefab;
		[SerializeField] AnimationClip weaponAnimation;

		public GameObject GetWeaponPrefab(){
			return weaponPrefab;
		}

		public AnimationClip GetAttAnimClip(){
			RemoveAnimationEvent (); 
			return weaponAnimation;
		}

		// Removing the asset pack's animation's event clip to prevent errors or bugs
		void RemoveAnimationEvent ()
		{
			weaponAnimation.events = new AnimationEvent[0];
		}
	}
}
