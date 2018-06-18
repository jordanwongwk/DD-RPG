using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {

	[Header ("Numbers")]
	[SerializeField] Text gameplayTimeText = null;
	[SerializeField] Text completionText = null;
	[SerializeField] Text secretDiscoveredText = null;
	[SerializeField] Text weaponDiscoveredText = null;
	[SerializeField] Text bossDefeatedText = null;
	[SerializeField] Text enemiesAliveText = null;
	[SerializeField] Text deathNumberText = null;

	[Header ("Total")]
	[SerializeField] Text completionTotalText = null;
	[SerializeField] Text secretsTotalText = null;
	[SerializeField] Text weaponsTotalText = null;
	[SerializeField] Text bossDefeatedTotalText = null;
	[SerializeField] Text enemiesAliveTotalText = null;
	[SerializeField] Text deathNumberTotalText = null;
	[SerializeField] Text finalTotalText = null;

	float gameplayTime;
	int gameplayMinutes;
	int gameplaySeconds;
	int gameCompleted;
	int secretDiscovered;
	int weaponDiscovered;
	int bossDefeated;
	int enemiesAlive;
	int deathNumber;

	int completeTotal;
	int secretTotal;
	int weaponTotal;
	int bossDefeatedTotal;
	int enemiesTotal;
	int deathTotal;
	int finalTotalScore;

	void Start () {
		GetInformationFromPlayerPrefs ();
		CalculateTotal ();
		DisplayValueAndTotal ();
	}

	void GetInformationFromPlayerPrefs ()
	{
		gameplayTime = PlayerPrefManager.GetTime ();
		gameCompleted = PlayerPrefManager.GetGameCompleted ();
		secretDiscovered = PlayerPrefManager.GetSecretDiscovered ();
		weaponDiscovered = PlayerPrefManager.GetWeaponDiscovered ();
		bossDefeated = PlayerPrefManager.GetBossDefeated ();
		enemiesAlive = PlayerPrefManager.GetEnemiesLeft ();
		deathNumber = PlayerPrefManager.GetDeathNumber ();
	}

	void CalculateTotal ()
	{
		int gameplayTimeRoundOff = Mathf.RoundToInt (gameplayTime);
		gameplayMinutes = gameplayTimeRoundOff / 60;
		gameplaySeconds = gameplayTimeRoundOff % 60;

		completeTotal = gameCompleted * 1000;
		secretTotal = secretDiscovered * 2000;
		weaponTotal = weaponDiscovered * 500;
		bossDefeatedTotal = bossDefeated * 1000;
		enemiesTotal = enemiesAlive * 100;
		deathTotal = deathNumber * 500;
		finalTotalScore = completeTotal + secretTotal + weaponTotal - enemiesTotal - deathTotal;
	}

	void DisplayValueAndTotal() {
		gameplayTimeText.text = gameplayMinutes.ToString() + " Min " + gameplaySeconds.ToString() + " Sec";
		completionText.text = gameCompleted.ToString ();
		secretDiscoveredText.text = secretDiscovered.ToString();
		weaponDiscoveredText.text = weaponDiscovered.ToString ();
		bossDefeatedText.text = bossDefeated.ToString ();
		enemiesAliveText.text = enemiesAlive.ToString ();
		deathNumberText.text = deathNumber.ToString ();

		completionTotalText.text = "= " + completeTotal.ToString ();
		secretsTotalText.text = "= " + secretTotal.ToString ();
		weaponsTotalText.text = "= " + weaponTotal.ToString ();
		bossDefeatedTotalText.text = "= " + bossDefeatedTotal.ToString ();
		enemiesAliveTotalText.text = "= -" + enemiesTotal.ToString ();
		deathNumberTotalText.text = "= - " + deathTotal.ToString ();
		finalTotalText.text = finalTotalScore.ToString ();
	}
}
