using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour {

	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float currentHealthPoints = 100f;

	public float healthAsPercentage
	{
		get
		{
			return currentHealthPoints / maxHealthPoints;	
		}
	}
}
