using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters{
	public class NPCBehaviour : MonoBehaviour {

		[SerializeField] float healingRadius = 5f;
		[SerializeField] float energyRecoveryRate = 10f;
		[Tooltip("Character setup for healer : Capsule Collider Size = healingRadius; isKinematic = true")]
		[SerializeField] bool isHealer = true;
		[SerializeField] GameObject healingParticles;

		GameObject player;
		GameObject healParticles;

		void Start () {
			player = FindObjectOfType<PlayerControl> ().gameObject;
		}

		void OnTriggerEnter (Collider collider){
			if (isHealer && collider.gameObject == player) {			// TODO Tag reference?
				collider.GetComponent<SpecialAbilities>().SetEnergyRecoveryRate(energyRecoveryRate);
				healParticles = Instantiate (healingParticles, player.transform.position, Quaternion.identity);
				healParticles.transform.parent = player.transform;
			}
		}

		void OnTriggerExit (Collider collider){
			if (isHealer && collider.gameObject == player) {
				collider.GetComponent<SpecialAbilities> ().SetDefaultEnergyRecoveryRate ();
				Destroy (healParticles);
			}
		}

		void OnDrawGizmos(){
			Gizmos.color = new Color (255f, 255f, 0, 0.5f);
			Gizmos.DrawWireSphere (transform.position, healingRadius);
		}
	}
}
