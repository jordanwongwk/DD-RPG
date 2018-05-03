using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters {
	public class Energy : MonoBehaviour {

		[SerializeField] Image energyBar = null;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float pointsPerHit = 10f;

		[SerializeField] float currentEnergyPoints;
		CameraRaycaster cameraRaycaster;

		void Start () {
			currentEnergyPoints = maxEnergyPoints;
			RegisterInDelegates ();
		}

		void RegisterInDelegates (){
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.notifyRightClickObservers += ConsumeEnergy;
		}

		void ConsumeEnergy(RaycastHit raycastHit, int layerHit){
			currentEnergyPoints = Mathf.Clamp (currentEnergyPoints - pointsPerHit, 0f, maxEnergyPoints);
			energyBar.fillAmount = (currentEnergyPoints / maxEnergyPoints);
		}
	}
}
