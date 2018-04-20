using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float projectileSpeed;

	float damageCaused;

	public void SetDamage(float damage){
		damageCaused = damage;
	}

	void OnTriggerEnter (Collider collider){
		IDamageable damageable = collider.GetComponent<IDamageable> ();
		if (damageable != null) {
			damageable.TakeDamage (damageCaused);
		}
	}
}
