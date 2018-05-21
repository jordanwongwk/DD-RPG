using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
//	[ExecuteInEditMode]
	public class WeaponPickupPoint : MonoBehaviour {
		
		[SerializeField] WeaponConfig weaponConfig = null;
		[SerializeField] AudioClip pickUpSFX = null;

		AudioSource audioSource;

		// Use this for initialization
		void Start () {
			audioSource = GetComponent<AudioSource> ();
		}

		void DestroyChildren(){
			foreach (Transform child in transform) {
				DestroyImmediate (child.gameObject);
			}
		}

		// Update is called once per frame
		void Update () {
			if (!Application.isPlaying) {
				DestroyChildren ();
				InstantiateWeapon ();	
			}
		}

		void InstantiateWeapon(){
			var weapon = weaponConfig.GetWeaponPrefab ();
			weapon.transform.position = Vector3.zero;
			Instantiate (weapon, gameObject.transform);
		}

		void OnTriggerEnter(){
			Debug.Log ("Equipped with " + weaponConfig);
			FindObjectOfType<WeaponSystem> ().ChangeWeaponInHand (weaponConfig);
			audioSource.PlayOneShot (pickUpSFX);
		}
	}
}
