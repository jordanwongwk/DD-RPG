using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerManager : MonoBehaviour {
	
	[SerializeField] GameObject bossBlocker = null;
	[SerializeField] GameObject backVillageBlocker = null;
	[SerializeField] GameObject frontVillageBlocker = null;

	GameManager gameManager;
	Vector3 backVillageEnableBlockPos;

	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
		gameManager.triggerBossBattle += enteringBossBattle;
		gameManager.triggerBossEnd += endingBossBattle;

		SettingUpInitialBlockers ();
	}

	void SettingUpInitialBlockers () {
		bossBlocker.SetActive (false);
		backVillageBlocker.SetActive (true);
		backVillageEnableBlockPos = backVillageBlocker.transform.position;
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
}
