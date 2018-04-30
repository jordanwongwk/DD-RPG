using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Weapons {
	public class Projectile : MonoBehaviour {
		
		[SerializeField] float projectileSpeed;
		[SerializeField] GameObject shooter;

		const float DESTROY_DELAY = 0.2f;
		float damageCaused;

		public float GetProjectileSpeed (){
			return projectileSpeed;
		}

		public void SetDamage(float damage){
			damageCaused = damage;
		}

		public void SetShooter (GameObject shooter){
			this.shooter = shooter;
		}

		void OnCollisionEnter (Collision collision){
			var collidedObjectLayer = collision.gameObject.layer;
			if (shooter && collidedObjectLayer != shooter.layer) {
				DamageIfDamageable (collision);
			}

			Destroy (gameObject, DESTROY_DELAY);
		}

		void DamageIfDamageable (Collision collision)
		{
			IDamageable damageable = collision.gameObject.GetComponent<IDamageable> ();
			if (damageable != null) {
				damageable.TakeDamage (damageCaused);
				Destroy (gameObject);
			}
		}
	}
}
