using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class EnemyManager : MonoBehaviour {

		[SerializeField] bool respawn = false;

		List<GameObject> allEnemies = new List<GameObject>();


		// Use this for initialization
		void Start () {
			UpdateEnemyList ();
		}
		
		// Update is called once per frame
		void Update () {




			if (respawn) {
				respawn = false;
				foreach (GameObject enemy in allEnemies) {
					enemy.SetActive (true);
					enemy.GetComponent<Character> ().RespawningSetup ();
				}

			}
		}

		void UpdateEnemyList ()
		{
			for (int i = 0; i < transform.childCount; i++) {
				allEnemies.Add (transform.GetChild (i).GetComponent<EnemyAI> ().gameObject);
			}
		}

		void SettingUpEnemyOnBrinkOfDeath () {
			
		}
	}
}
