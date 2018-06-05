using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[SerializeField] bool isPhase1Done = false;		// Giving item to Derrick
	[SerializeField] bool isPhase2Done = false;		// Talking to Merlin (triggering secret event 1)
	bool isPhase3Done = false;		// Defeated Boss & Phase 2 Complete

	bool isSecret1Done = false;

	public delegate void TriggerBossBattle();
	public event TriggerBossBattle triggerBossBattle;

	public delegate void TriggerBossEnd();
	public event TriggerBossEnd triggerBossEnd;

	// Phase 1
	public void SetPhase1Done () {
		isPhase1Done = true;
	}

	public bool GetPhase1Info(){
		return isPhase1Done;
	}
		
	// Phase 2
	public void SetPhase2Done (){
		isPhase2Done = true;
	}

	public bool GetPhase2Info(){
		return isPhase2Done;
	}

	// Phase 3
	public void SetPhase3Done (){
		isPhase3Done = true;
	}


	// Secret 1
	public void SetSecret1Done (){
		isSecret1Done = true;
	}

	public bool GetSecret1Info () {
		return isSecret1Done;
	}

	public void TriggerBossBattleDelegate(){
		triggerBossBattle ();
	}

	public void TriggerBossEndDelegate(){
		triggerBossEnd ();
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
