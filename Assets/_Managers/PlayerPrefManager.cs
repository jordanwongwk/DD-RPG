using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefManager : MonoBehaviour {

	const string TIME = "time";
	const string SECRET_DISCOVERED = "secret_discovered";
	const string WEAPON_DISCOVERED = "weapon_discovered";
	const string BOSS_DEFEATED = "boss_defeated";
	const string ENEMIES_LEFT = "enemies_left";
	const string NUMBER_OF_DEATH = "number_of_death";

	public static void SetTime (float time){
		PlayerPrefs.SetFloat(TIME, time);
	}

	public static float GetTime (){
		return PlayerPrefs.GetFloat (TIME);
	}

	public static void SetSecretDiscovered (int secretFound){
		PlayerPrefs.SetInt (SECRET_DISCOVERED, secretFound);
	}

	public static int GetSecretDiscovered () {
		return PlayerPrefs.GetInt (SECRET_DISCOVERED);
	}

	public static void SetWeaponDiscovered (int weaponFound){
		PlayerPrefs.SetInt (WEAPON_DISCOVERED, weaponFound);
	}

	public static int GetWeaponDiscovered () {
		return PlayerPrefs.GetInt (WEAPON_DISCOVERED);
	}

	public static void SetBossDefeated (int bossDefeated) {
		PlayerPrefs.SetInt (BOSS_DEFEATED, bossDefeated);
	}

	public static int GetBossDefeated () {
		return PlayerPrefs.GetInt (BOSS_DEFEATED);
	}

	public static void SetEnemiesLeft (int enemiesLeft){
		PlayerPrefs.SetInt (ENEMIES_LEFT, enemiesLeft);
	}

	public static int GetEnemiesLeft (){
		return PlayerPrefs.GetInt(ENEMIES_LEFT);
	}

	public static void SetDeathNumber (int numberOfDeaths){
		PlayerPrefs.SetInt (NUMBER_OF_DEATH, numberOfDeaths);
	}

	public static int GetDeathNumber () {
		return PlayerPrefs.GetInt (NUMBER_OF_DEATH);
	}
}
