using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters {
	public class Energy : MonoBehaviour {

		[SerializeField] Image energyBar = null;
		[SerializeField] float maxEnergyPoints = 100f;

		float currentEnergyPoints;

		void Start () {
			currentEnergyPoints = maxEnergyPoints;
		}

		public bool IsEnergyAvailable (float amount) {
			return amount <= currentEnergyPoints;
		}

		public void ConsumeEnergy (float amount) {
			currentEnergyPoints = Mathf.Clamp (currentEnergyPoints - amount, 0f, maxEnergyPoints);
			energyBar.fillAmount = (currentEnergyPoints / maxEnergyPoints);
		}
	}
}
