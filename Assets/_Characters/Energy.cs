using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters {
	public class Energy : MonoBehaviour {

		[SerializeField] Image energyBar = null;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float regenEnergyPerSecond = 1f;

		float currentEnergyPoints;

		void Start () {
			currentEnergyPoints = maxEnergyPoints;
			UpdateEnergyBar ();
		}

		void Update() {
			if (currentEnergyPoints < maxEnergyPoints) {
				RegenEnergy ();
				UpdateEnergyBar ();
			}
		}

		void RegenEnergy(){
			float pointsToAdd = regenEnergyPerSecond * Time.deltaTime;
			currentEnergyPoints = Mathf.Clamp (currentEnergyPoints + pointsToAdd, 0f, maxEnergyPoints);
		}

		public bool IsEnergyAvailable (float amount) {
			return amount <= currentEnergyPoints;
		}

		public void ConsumeEnergy (float amount) {
			currentEnergyPoints = Mathf.Clamp (currentEnergyPoints - amount, 0f, maxEnergyPoints);
			UpdateEnergyBar ();
		}

		void UpdateEnergyBar ()
		{
			energyBar.fillAmount = (currentEnergyPoints / maxEnergyPoints);
		}
	}
}
