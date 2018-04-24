using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float projectileSpeed;

	float damageCaused;

	public void SetDamage(float damage){
		damageCaused = damage;
	}

	void OnCollisionEnter (Collision collision){
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable> ();
		if (damageable != null) {
			damageable.TakeDamage (damageCaused);
			Destroy (gameObject);
		}
		Destroy (gameObject, 0.2f);
	}
}
