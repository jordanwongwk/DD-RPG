using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField] float damageCaused = 10f;

	void OnTriggerEnter (Collider collider){
		IDamageable damageable = collider.GetComponent<IDamageable> ();
		damageable.TakeDamage (damageCaused);
	}
}
