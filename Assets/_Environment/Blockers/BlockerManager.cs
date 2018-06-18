using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerManager : MonoBehaviour {
	
	[SerializeField] GameObject bossBlocker = null;
	[SerializeField] GameObject optionalBossBlocker = null;
	[SerializeField] GameObject backVillageBlocker = null;
	[SerializeField] GameObject bossSecret2Blocker = null;
	[SerializeField] GameObject bossSecret3Blocker = null;
	[SerializeField] GameObject axeSecret3Reward = null;
	[SerializeField] GameObject endingBlocker = null;

	const string COLLISION_COLLIDER = "CollisionCollider";

	GameManager gameManager;
	EventManager eventManager;
	Vector3 backVillageEnableBlockPos;
	Vector3 bossSecret2EnableBlockPos;
	Vector3 bossSecret3EnableBlockPos;
	Vector3 axeSecret3RewardInitialPos;
	Vector3 endingBlockerInitialPos;

	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
		eventManager = FindObjectOfType<EventManager> ();
		gameManager.triggerBossBattle += blockBossArena;
		gameManager.triggerBossEnd += unblockBossArena;
		gameManager.triggerRestartBoss += unblockBossArena;

		SettingUpInitialBlockers ();
	}

	void SettingUpInitialBlockers () {
		bossBlocker.SetActive (false);
		optionalBossBlocker.SetActive (false);
		backVillageBlocker.SetActive (true);
		backVillageEnableBlockPos = backVillageBlocker.transform.position;
		bossSecret2EnableBlockPos = bossSecret2Blocker.transform.position;
		bossSecret3EnableBlockPos = bossSecret3Blocker.transform.position;
		axeSecret3RewardInitialPos = axeSecret3Reward.transform.position;
		endingBlockerInitialPos = endingBlocker.transform.position;
	}

	// Boss Blocker
	void blockBossArena(){
		bossBlocker.SetActive (true);
		optionalBossBlocker.SetActive (true);
	}

	void unblockBossArena(){
		bossBlocker.SetActive (false);
		optionalBossBlocker.SetActive (false);
	}

	void Update(){
		CheckForBackVillageBlocker ();
		CheckForBossSecret2Blocker ();
		CheckForBossSecret3Blocker ();
		CheckForAxeSecret3Reward ();
		CheckForEnding ();
	}

	void CheckForBackVillageBlocker (){
		// Disable on Phase1Done, Phase3Done
		if (gameManager.GetPhase1Info () == true ) {
			backVillageBlocker.transform.position = new Vector3 (backVillageEnableBlockPos.x, 15f, backVillageEnableBlockPos.z);
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
		if (gameManager.GetSecret2Info () == true && gameManager.GetSecret3Info () == false && eventManager.GetSecretEvent3Ended() == false) {
			bossSecret3Blocker.transform.position = bossSecret3EnableBlockPos;

			if (eventManager.GetSecretEvent3Initiation () == true) {
				bossSecret3Blocker.transform.Find (COLLISION_COLLIDER).gameObject.SetActive (false);
			}
		} else {
			bossSecret3Blocker.transform.position = new Vector3 (bossSecret3EnableBlockPos.x, 25f, bossSecret3EnableBlockPos.z);
		}
	}

	void CheckForAxeSecret3Reward() {
		if (gameManager.GetSecret3Info () == true) {
			axeSecret3Reward.transform.position = axeSecret3RewardInitialPos;
		} else {
			axeSecret3Reward.transform.position = new Vector3 (axeSecret3RewardInitialPos.x, 0f, axeSecret3RewardInitialPos.z);
		}
	}

	void CheckForEnding() {
		if (gameManager.GetPhase3Info () == true) {
			endingBlocker.transform.position = endingBlockerInitialPos;
		} else {
			endingBlocker.transform.position = new Vector3 (endingBlockerInitialPos.x, 10f, endingBlockerInitialPos.z);
		}
	}
}
