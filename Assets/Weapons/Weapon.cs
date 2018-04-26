using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Weapon")]
public class Weapon : ScriptableObject {

	public GameObject gripTransform;

	[SerializeField] GameObject weaponPrefab;
	[SerializeField] AnimationClip weaponAnimation;

	public GameObject GetWeaponPrefab(){
		return weaponPrefab;
	}
}
