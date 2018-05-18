using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters {
	public class SpecialAbilities : MonoBehaviour {

		[SerializeField] Image energyBar = null;		
		[SerializeField] AbilityConfig[] abilities = null;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float regenEnergyPerSecond = 1f;
		[SerializeField] AudioClip outOfEnergy;

		float currentEnergyPoints;
		float energyAsPercent { get { return currentEnergyPoints / maxEnergyPoints; } }

		AudioSource audioSource;

		void Start () {
			audioSource = GetComponent<AudioSource> ();

			currentEnergyPoints = maxEnergyPoints;
			UpdateEnergyBar ();
			SetupAbilitiesBehaviour ();
		}

		void Update() {
			if (currentEnergyPoints < maxEnergyPoints) {
				RegenEnergy ();
				UpdateEnergyBar ();
			}
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

			if (energyCost <= currentEnergyPoints) {		//TODO Read from SO
				ConsumeEnergy (energyCost);
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

		void UpdateEnergyBar ()
		{
			energyBar.fillAmount = energyAsPercent;
		}
	}
}
