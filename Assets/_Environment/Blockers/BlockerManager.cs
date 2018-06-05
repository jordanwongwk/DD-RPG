using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerManager : MonoBehaviour {
	
	[SerializeField] GameObject bossBlocker = null;
	[SerializeField] GameObject backVillageBlocker = null;
	[SerializeField] GameObject frontVillageBlocker = null;

	GameManager gameManager;

	void Start () {
		gameManager = FindObjectOfType<GameManager> ();
		gameManager.triggerBossBattle += enteringBossBattle;
		gameManager.triggerBossEnd += endingBossBattle;
	}
	
	void enteringBossBattle(){
		bossBlocker.gameObject.SetActive (true);
	}

	void endingBossBattle(){
		bossBlocker.gameObject.SetActive (false);
	}

	void Update(){
		
	}
}
