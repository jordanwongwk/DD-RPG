using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float projectileSpeed;
	public float damageCaused;

	void OnTriggerEnter (Collider collider){
		IDamageable damageable = collider.GetComponent<IDamageable> ();
		if (damageable != null) {
			damageable.TakeDamage (damageCaused);
		}
	}
}
