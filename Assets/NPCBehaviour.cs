using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters{
	public class NPCBehaviour : MonoBehaviour {

		[SerializeField] float healingRadius = 5f;
		[SerializeField] float energyRecoveryRate = 10f;
		[SerializeField] bool isHealer = true;

		GameObject player;

		void Start () {
			player = FindObjectOfType<PlayerControl> ().gameObject;
		}

		void OnTriggerEnter (Collider collider){
			if (isHealer && collider.gameObject == player) {			// TODO Tag reference?
				collider.GetComponent<SpecialAbilities>().SetEnergyRecoveryRate(energyRecoveryRate);
			}
		}

		void OnTriggerExit (Collider collider){
			if (isHealer && collider.gameObject == player) {
				collider.GetComponent<SpecialAbilities> ().SetDefaultEnergyRecoveryRate ();
			}
		}

		void OnDrawGizmos(){
			Gizmos.color = new Color (255f, 255f, 0, 0.5f);
			Gizmos.DrawWireSphere (transform.position, healingRadius);
		}
	}
}
