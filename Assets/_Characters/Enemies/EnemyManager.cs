using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class EnemyManager : MonoBehaviour {
		List<GameObject> allEnemies = new List<GameObject>();

		// Use this for initialization
		void Start () {
			GameManager gameManager = FindObjectOfType<GameManager> ();
			gameManager.onPlayerRespawn += RespawnEnemiesOnBrinkOfDeath;
			gameManager.triggerBossEnd += UpdateEnemyList;
			gameManager.endGameSetup += CalculateEnemiesLeft;

			UpdateEnemyList ();
		}

		void RespawnEnemiesOnBrinkOfDeath () {
			foreach (GameObject enemy in allEnemies) {
				enemy.SetActive (true);
				enemy.GetComponent<Character> ().RespawningSetup ();
			}
		}

		void UpdateEnemyList ()
		{
			allEnemies.Clear ();
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild (i) != null) {
					allEnemies.Add (transform.GetChild (i).GetComponent<EnemyAI> ().gameObject);
				}
			}
		}

		public void DestroyEnemyOnBrinkOfDeath () {
			for (int i = 0; i < transform.childCount; i++)  {
				if (allEnemies[i].activeInHierarchy == false) {
					allEnemies [i].transform.parent = null;				// Unparent the child
					Destroy (allEnemies[i]);							// Destroy
				}
			}
			UpdateEnemyList ();
		}

		void CalculateEnemiesLeft () {
			DestroyEnemyOnBrinkOfDeath ();
			PlayerPrefManager.SetEnemiesLeft (allEnemies.Count);
		}
	}
}
