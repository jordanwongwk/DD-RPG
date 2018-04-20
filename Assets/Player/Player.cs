using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour, IDamageable{

	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float currentHealthPoints = 100f;

	public float healthAsPercentage	{ get {	return currentHealthPoints / maxHealthPoints; }}

	public void TakeDamage (float damage){
		currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
		if (currentHealthPoints <= 0f) {
			Destroy (gameObject);
		}
	} 
}
