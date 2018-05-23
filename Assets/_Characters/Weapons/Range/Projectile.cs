using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class Projectile : MonoBehaviour {
		GameObject shooter;
		const float DESTROY_DELAY = 0.2f;
		float damageCaused;

		public void SetDamage(float damage){
			damageCaused = damage;
		}

		public void SetShooter (GameObject shooter){
			this.shooter = shooter;
		}

		void OnCollisionEnter (Collision collision){
			var collidedObjectLayer = collision.gameObject.layer;
			var healthSystem = collision.gameObject.GetComponent<HealthSystem> ();
			if (shooter && collidedObjectLayer != shooter.layer && healthSystem) {
				healthSystem.TakeDamage (damageCaused);
				Destroy (gameObject);
			}
			Destroy (gameObject, DESTROY_DELAY);
		}
	}
}
