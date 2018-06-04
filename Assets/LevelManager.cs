using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	bool isPhase1Done = false;		// Giving item to Derrick
	bool isPhase2Done = false;		// Talking to Merlin (triggering secret event 1)
	bool isPhase3Done = false;		// Defeated Boss & Phase 2 Complete

	public void SetPhase1Done () {
		isPhase1Done = true;
	}

	public bool GetPhase1Info(){
		return isPhase1Done;
	}

	public void SetPhase2Done (){
		isPhase2Done = true;
	}

	public void SetPhase3Done (){
		isPhase3Done = true;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
