using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.CameraUI;

public class GameManager : MonoBehaviour {

	MySceneManager mySceneManager;

	[SerializeField] bool isPhase1Done = false;		
	[SerializeField] bool isPhase2Done = false;		
	[SerializeField] bool isPhase3Done = false;		
	[SerializeField] bool isSecret1Done = false;
	[SerializeField] bool isSecret2Done = false;
	[SerializeField] bool isSecret3Done = false;
	[SerializeField] bool isSecret4Done = false;
	int bossDefeated = 0;
	int secretsFound = 0;
	int weaponsFound = 0;
	bool isInBossBattle = false;

	const float TIME_END_GAME = 5.0f;

	public delegate void TriggerBossBattle();
	public event TriggerBossBattle triggerBossBattle;

	public delegate void TriggerBossEnd();
	public event TriggerBossEnd triggerBossEnd;

	public delegate void TriggerRestartBoss();
	public event TriggerRestartBoss triggerRestartBoss;

	public delegate void OnRespawning();
	public event OnRespawning onPlayerRespawn;

	public delegate void EndGameSetup();
	public event EndGameSetup endGameSetup;

	void Start() {
		mySceneManager = FindObjectOfType<MySceneManager> ();
	}

	// MAIN STORY
	// Phase 1 : Passed the package to Derrick, proceed to help investigate the castle to the west.
	public void SetPhase1Done () {
		isPhase1Done = true;
	}

	public bool GetPhase1Info(){
		return isPhase1Done;
	}
		
	// Phase 2 : Speaking to Merlin and learn about their leader being on the path you walked, UNLOCK SECRET 1.
	public void SetPhase2Done (){
		isPhase2Done = true;
	}

	public bool GetPhase2Info(){
		return isPhase2Done;
	}

	// Phase 3 : Defeated the Dark Knight (Main Boss)
	public void SetPhase3Done (){
		isPhase3Done = true;
	}

	public bool GetPhase3Info(){
		return isPhase3Done;
	}
	// MAIN STORY END

	// SECRET STORY
	// Secret 1 : Learn about Derrick talking to a Minion and saying a guarding Dark Knight in the castle. UNLOCK SECRET 2.
	public void SetSecret1Done (){
		isSecret1Done = true;
		secretsFound += 1;
	}

	public bool GetSecret1Info () {
		return isSecret1Done;
	}

	// Secret 2: Learn about Derrick from Dark Knight. Pre-boss?. REQUIRED SECRET 1
	public void SetSecret2Done (){
		isSecret2Done = true;
		secretsFound += 1;
	}

	public bool GetSecret2Info () {
		return isSecret2Done;
	}

	// Secret 3: Confronted Derrick and boss battle with Dark Knight (Optional). REQUIRED SECRET 2
	public void SetSecret3Done (){
		isSecret3Done = true;
		secretsFound += 1;
	}

	public bool GetSecret3Info () {
		return isSecret3Done;
	}

	// Secret 4: Learn about Axe origin and MC's truth (hint). REQUIRED SECRET 3 and AXE
	public void SetSecret4Done (){
		isSecret4Done = true;
		secretsFound += 1;
	}

	public bool GetSecret4Info () {
		return isSecret4Done;
	}
	// SECRET STORY END

	public void TriggerBossBattleDelegate(){
		triggerBossBattle ();
		isInBossBattle = true;
	}

	public void TriggerBossEndDelegate(){
		triggerBossEnd ();
		isInBossBattle = false;
		bossDefeated += 1;

		if (GetPhase3Info() == false && bossDefeated == 1) {
			SetPhase3Done ();
		} else if (GetSecret3Info() == false && bossDefeated == 2) {
			SetSecret3Done ();
		}
	}

	public void RestartBossBattleUponDeath() {
		if (isInBossBattle) {
			triggerRestartBoss ();
		}
	}

	public void StartRespawnDelegates(){
		onPlayerRespawn ();
	}

	public void SetWeaponFound (){
		weaponsFound += 1;
	}

	public void TriggerEndOfGame(){
		StartCoroutine (EndGame ());
	}

	IEnumerator EndGame(){
		endGameSetup ();
		PlayerPrefSettingUp ();
		yield return new WaitForSeconds (TIME_END_GAME);
		mySceneManager.RankingScene ();
	}

	void PlayerPrefSettingUp(){
		PlayerPrefManager.SetTime (Time.timeSinceLevelLoad);
		PlayerPrefManager.SetSecretDiscovered (secretsFound);
		PlayerPrefManager.SetWeaponDiscovered (weaponsFound);
		PlayerPrefManager.SetBossDefeated (bossDefeated);
	}
}
