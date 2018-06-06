using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerManager : MonoBehaviour {
	
	[SerializeField] GameObject bossBlocker = null;
	[SerializeField] GameObject backVillageBlocker = null;
	[SerializeField] GameObject frontVillageBlocker = null;
	[SerializeField] GameObject bossSecret2Blocker = null;
	[SerializeField] GameObject bossSecret3Blocker = null;

	const string COLLISION_COLLIDER = "CollisionCollider";

	GameManager gameManager;
	EventManager eventManager;
	Vector3 backVillageEnableBlockPos;
	Vector3 bossSecret2EnableBlockPos;
	Vector3 bossSecret3EnableBlockPos;

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
		bossSecret2EnableBlockPos = bossSecret2Blocker.transform.position;
		bossSecret3EnableBlockPos = bossSecret3Blocker.transform.position;
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
		CheckForBossSecret2Blocker ();
		CheckForBossSecret3Blocker ();
	}

	void CheckForBackVillageBlocker (){
		// Disable on Phase1Done, Phase3Done
		if (gameManager.GetPhase1Info () == true ) {
			backVillageBlocker.transform.position = new Vector3 (backVillageEnableBlockPos.x, 0f, backVillageEnableBlockPos.z);
		}

		// Enable on Phase2Done
		if (gameManager.GetPhase2Info () == true && gameManager.GetSecret2Info() == false) {
			backVillageBlocker.transform.position = backVillageEnableBlockPos;
		}
	}

	void CheckForBossSecret2Blocker () {
		if (gameManager.GetSecret1Info () == true && gameManager.GetSecret2Info() == false) {
			bossSecret2Blocker.transform.position = bossSecret2EnableBlockPos;

			if(eventManager.GetSecretEvent2Initiation () == true){
				bossSecret2Blocker.transform.Find (COLLISION_COLLIDER).gameObject.SetActive (false);
			}
		} else {
			float currentXPos = bossSecret2Blocker.transform.position.x;
			bossSecret2Blocker.transform.position = new Vector3 (currentXPos, 15f, bossSecret2EnableBlockPos.z);
		}
	}

	void CheckForBossSecret3Blocker () {
		if (gameManager.GetSecret2Info () == true && gameManager.GetSecret3Info () == false) {
			bossSecret3Blocker.transform.position = bossSecret3EnableBlockPos;

			if (eventManager.GetSecretEvent3Initiation () == true) {
				bossSecret3Blocker.transform.Find (COLLISION_COLLIDER).gameObject.SetActive (false);
			}
		} else {
			float currentXPos = bossSecret3Blocker.transform.position.x;
			bossSecret3Blocker.transform.position = new Vector3 (currentXPos, 25f, bossSecret3EnableBlockPos.z);
		}
	}
}
