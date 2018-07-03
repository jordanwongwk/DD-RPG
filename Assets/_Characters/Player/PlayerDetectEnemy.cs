using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PlayerDetectEnemy : MonoBehaviour {
		
		[SerializeField] float detectionRange = 3.0f;
		[SerializeField] GameObject enemyTargetIndicator = null;

		int selectedEnemyNumber = 100; 				// Make absurd high number
		GameObject selectedEnemy;
		List<GameObject> enemyInSight = new List<GameObject>();
			
		void Update()
		{
			GetEnemyInRange ();

			if (selectedEnemy != null && !enemyInSight.Contains(selectedEnemy)) {		// Go false if enemy out of sight
				ResettingSelectedEnemyAndIndicator ();
			} else if (selectedEnemy != null){
				IndicateTargettedEnemy ();
			} 

			for (int i = enemyInSight.Count-1; i > -1; i--) {
				if (enemyInSight [i] == null) {			// Boss destroys immediately
					enemyInSight.RemoveAt (i);
					return;
				}

				float distanceToPlayer = Vector3.Distance (transform.position, enemyInSight [i].transform.position);
				if (!enemyInSight [i].activeInHierarchy || distanceToPlayer > detectionRange) {
					enemyInSight.RemoveAt (i);
				}
			}
		}

		void GetEnemyInRange(){
			Collider[] hitColliders = Physics.OverlapSphere (transform.position, detectionRange);

			foreach (Collider col in hitColliders) {
				if (col.gameObject.GetComponent<EnemyAI> () && !enemyInSight.Contains(col.gameObject)) {
					enemyInSight.Add (col.gameObject);
				}
			}
		}

		void IndicateTargettedEnemy(){
			enemyTargetIndicator.transform.position = selectedEnemy.transform.position;
		}

		public void SelectingEnemyTarget(){
			if (enemyInSight.Count > 0) {			// If there are enemies in range
				selectedEnemyNumber++;
				if (selectedEnemyNumber >= enemyInSight.Count) {
					selectedEnemyNumber = 0;
				} 

				if (enemyInSight [selectedEnemyNumber] != null) {
					selectedEnemy = enemyInSight [selectedEnemyNumber];
					IndicateTargettedEnemy ();
					enemyTargetIndicator.SetActive (true);
				} 
			} 
		}
			
		public GameObject GetEnemyTarget (){
			return selectedEnemy;
		}

		public void SetEnemyTarget (GameObject target) {
			selectedEnemy = target;
			IndicateTargettedEnemy ();
			enemyTargetIndicator.SetActive (true);
		}

		public void SetIndicatorOffStillTargetted(GameObject target){
			if (target == selectedEnemy) {
				enemyTargetIndicator.SetActive (false);
				selectedEnemy = null;
			}
		}

		public void ResettingSelectedEnemyAndIndicator(){
			enemyTargetIndicator.SetActive (false);
			selectedEnemy = null;
		}
	}
}