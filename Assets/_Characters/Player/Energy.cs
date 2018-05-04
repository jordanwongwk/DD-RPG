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
			cameraRaycaster.onMouseOverEnemy += MouseOverEnemy;
		}

		void MouseOverEnemy (Enemy enemy){
			if (Input.GetMouseButtonDown (1)) {
				UpdateEnergyBar ();
			}
		}
			
		void UpdateEnergyBar ()
		{
			currentEnergyPoints = Mathf.Clamp (currentEnergyPoints - pointsPerHit, 0f, maxEnergyPoints);
			energyBar.fillAmount = (currentEnergyPoints / maxEnergyPoints);
		}
	}
}
