﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters {
	public class SpecialAbilities : MonoBehaviour {

		[SerializeField] Image energyBar = null;
		[SerializeField] Text energyText = null;
		[SerializeField] AbilityConfig[] abilities = null;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float regenEnergyPerSecond = 1f;
		[SerializeField] AudioClip outOfEnergy = null;

		float currentEnergyPoints;
		float defaultEnergyRecoverRate;
		float energyAsPercent { get { return currentEnergyPoints / maxEnergyPoints; } }

		AudioSource audioSource;
		WeaponSystem weaponSystem;
		GameManager gameManager;
		GameObject encounteredNPC;

		void Start () {
			audioSource = GetComponent<AudioSource> ();
			weaponSystem = GetComponent<WeaponSystem> ();
			gameManager = FindObjectOfType<GameManager> ();

			gameManager.onPlayerRespawn += SetRespawnFullEnergy;
			currentEnergyPoints = maxEnergyPoints;
			defaultEnergyRecoverRate = regenEnergyPerSecond;
			UpdateEnergyBar ();
			UpdateEnergyText ();
			SetupAbilitiesBehaviour ();
		}

		void Update() {
			if (currentEnergyPoints <= maxEnergyPoints) {
				RegenEnergy ();
				UpdateEnergyBar ();
				UpdateEnergyText ();
			}
		}

		void UpdateEnergyBar ()
		{
			energyBar.fillAmount = energyAsPercent;
		}

		void UpdateEnergyText () {
			energyText.text = Mathf.RoundToInt(currentEnergyPoints) + " / " + maxEnergyPoints;
		}

		void SetRespawnFullEnergy(){
			currentEnergyPoints = maxEnergyPoints;
		}

		public void SetEnergyRecoveryRate (float newEnergyRecRate){
			regenEnergyPerSecond = newEnergyRecRate;
		}

		public void SetDefaultEnergyRecoveryRate (){
			regenEnergyPerSecond = defaultEnergyRecoverRate;
		}

		public int GetAbilitiesLength () {
			return abilities.Length;
		}

		void SetupAbilitiesBehaviour () {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities[abilityIndex].AttachAbilityTo (gameObject);
			}
		}

		public void AttemptSpecialAbility (int abilityNumber, GameObject target = null)
		{
			float energyCost = abilities [abilityNumber].GetEnergyCost ();

			if (energyCost <= currentEnergyPoints) {		
				ConsumeEnergy (energyCost);
				weaponSystem.StopAttacking ();
				abilities [abilityNumber].Use (target);
			} else {
				audioSource.PlayOneShot (outOfEnergy);
			}
		}

		void RegenEnergy(){
			float pointsToAdd = regenEnergyPerSecond * Time.deltaTime;
			currentEnergyPoints = Mathf.Clamp (currentEnergyPoints + pointsToAdd, 0f, maxEnergyPoints);
		}

		public void ConsumeEnergy (float amount) {
			currentEnergyPoints = Mathf.Clamp (currentEnergyPoints - amount, 0f, maxEnergyPoints);
			UpdateEnergyBar ();
		}
	}
}
