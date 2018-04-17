using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour {

	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float currentHealthPoints = 100f;
	[SerializeField] float triggerRadius = 5f;

	AICharacterControl aiCharacterControl = null;
	GameObject player;

	public float healthAsPercentage
	{
		get
		{
			return currentHealthPoints / maxHealthPoints;	
		}
	}

	void Start(){
		aiCharacterControl = GetComponent<AICharacterControl> ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Update(){
		float distanceDiff = Vector3.Distance (player.transform.position, transform.position);

		if (distanceDiff <= triggerRadius) {
			aiCharacterControl.SetTarget (player.transform);
		} else if (distanceDiff > triggerRadius){
			aiCharacterControl.SetTarget (transform);
		}
	}
}
