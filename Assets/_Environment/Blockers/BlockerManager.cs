using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerManager : MonoBehaviour {
	
	[SerializeField] GameObject bossBlocker = null;
	[SerializeField] GameObject backVillageBlocker = null;
	[SerializeField] GameObject frontVillageBlocker = null;
	[SerializeField] GameObject bossEventBlocker = null;

	const string COLLISION_COLLIDER = "CollisionCollider";

	GameManager gameManager;
	EventManager eventManager;
	Vector3 backVillageEnableBlockPos;
	Vector3 bossEventEnableBlockPos;

	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
		eventManager = FindObjectOfType<EventManager> ();
		gameManager.triggerBossBattle += enteringBossBattle;
		gameManager.triggerBossEnd += endingBossBattle;

		SettingUpInitialBlockers ();
	}

	void SettingUpInitialBlockers () {
		bossBlocker.SetActive (false);
		backVillageBlocker.SetActive (true);
		backVillageEnableBlockPos = backVillageBlocker.transform.position;
		bossEventEnableBlockPos = bossEventBlocker.transform.position;
	}

	// Boss Blocker
	void enteringBossBattle(){
		bossBlocker.SetActive (true);
	}

	void endingBossBattle(){
		bossBlocker.SetActive (false);
	}

	void Update(){
		CheckForBackVillageBlocker ();
		CheckForBossEventBlocker ();
	}

	void CheckForBackVillageBlocker (){
		// Disable on Phase1Done, Phase3Done, Phase4Done
		if (gameManager.GetPhase1Info () == true) {
			backVillageBlocker.transform.position = new Vector3 (backVillageEnableBlockPos.x, 0f, backVillageEnableBlockPos.z);
		}

		// Enable on Phase2Done, BossFight
		if (gameManager.GetPhase2Info () == true) {
			backVillageBlocker.transform.position = backVillageEnableBlockPos;
		}
	}

	void CheckForBossEventBlocker () {
		if (gameManager.GetSecret1Info () == true && gameManager.GetSecret2Info() == false) {
			bossEventBlocker.transform.position = bossEventEnableBlockPos;

			if(eventManager.GetSecretEvent2Initiation () == true){
				bossEventBlocker.transform.Find (COLLISION_COLLIDER).gameObject.SetActive (false);
			}
		} else {
			float currentXPos = bossEventBlocker.transform.position.x;
			bossEventBlocker.transform.position = new Vector3 (currentXPos, 15f, bossEventEnableBlockPos.z);
		}
	}
}
