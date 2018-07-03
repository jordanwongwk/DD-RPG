using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters{
	public class WispGuideBehaviour : MonoBehaviour {
		[SerializeField] float setDistanceToWaitForPlayer = 10.0f;
		[SerializeField] GameObject phase1ObjectivePoint = null;
		[SerializeField] GameObject mainBossObjectivePoint = null;
		[SerializeField] GameObject mainGoalObjectivePoint = null;
		[SerializeField] GameObject optionalBossObjectivePoint = null; 	

		NavMeshAgent agent;
		GameObject player;
		GameManager gameManager;

		void Start () {
			agent = GetComponent<NavMeshAgent> ();
			player = FindObjectOfType<PlayerControl> ().gameObject;
			gameManager = FindObjectOfType<GameManager> ();
		}

		void Update () {
			// Setting Objective Point
			if (gameManager.GetPhase1Info () == false) {
				agent.destination = phase1ObjectivePoint.transform.position;
			} else if (gameManager.GetPhase1Info () == true && gameManager.GetPhase3Info () == false) {
				agent.destination = mainBossObjectivePoint.transform.position;
			} else if (gameManager.GetPhase3Info () == true && gameManager.GetSecret2Info () == false) {
				agent.destination = mainGoalObjectivePoint.transform.position;
			} else if (gameManager.GetPhase3Info () == true && gameManager.GetSecret2Info () == true && gameManager.GetSecret3Info () == false) {
				agent.destination = optionalBossObjectivePoint.transform.position;
			} else if (gameManager.GetSecret3Info () == true) {
				agent.destination = mainGoalObjectivePoint.transform.position;
			}

			float distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);
			if (distanceToPlayer <= setDistanceToWaitForPlayer) {
				agent.isStopped = false;
			} else {
				agent.isStopped = true;
			}
		}
	}
}
