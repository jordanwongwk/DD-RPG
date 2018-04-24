using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable {

	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float attackRadius = 5f;
	[SerializeField] float moveRadius = 6f;
	[SerializeField] float projectileDamage = 9f;
	[SerializeField] float projectileShotDelay = 0.5f;
	[SerializeField] GameObject projectileToUse;
	[SerializeField] GameObject projectileSpawnPoint;
	[SerializeField] Vector3 aimOffset = new Vector3(0,1f,0);

	bool isAttacking = false;
	float currentHealthPoints;
	AICharacterControl aiCharacterControl = null;
	GameObject player;

	public float healthAsPercentage { get {	return currentHealthPoints / maxHealthPoints; }	}

	void Start(){
		currentHealthPoints = maxHealthPoints;
		aiCharacterControl = GetComponent<AICharacterControl> ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Update(){
		float distanceDiff = Vector3.Distance (player.transform.position, transform.position);

		// For attack radius
		if (distanceDiff <= attackRadius) {
			if (!isAttacking) {
				isAttacking = true;
				InvokeRepeating ("SpawnProjectiles", 0f, projectileShotDelay);
			}
			// Make enemy keep looking at player
			transform.LookAt (player.transform.position);
		} 

		if (distanceDiff > attackRadius) {
			isAttacking = false;
			CancelInvoke ();
		}	

		//TODO Move and attack
		// For move radius
		if (distanceDiff <= moveRadius) {
			aiCharacterControl.SetTarget (player.transform);
		} else {
			aiCharacterControl.SetTarget (transform);
		}
	}

	void SpawnProjectiles ()
	{
		GameObject projectile = Instantiate (projectileToUse, projectileSpawnPoint.transform.position, Quaternion.identity);
		Projectile projComponent = projectile.GetComponent<Projectile> ();
		projComponent.SetDamage(projectileDamage);

		Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSpawnPoint.transform.position).normalized;
		float projectileSpeed = projComponent.projectileSpeed;
		projectile.GetComponent<Rigidbody> ().velocity = unitVectorToPlayer * projectileSpeed;
	}

	public void TakeDamage (float damage){
		currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);

		if (currentHealthPoints <= 0f) {
			Destroy (gameObject);
		}
	}

	void OnDrawGizmos(){
		// Drawing Attack Radius
		Gizmos.color = new Color(255,0,0,0.5f);
		Gizmos.DrawWireSphere (transform.position, attackRadius);

		// Drawing Move Radius
		Gizmos.color = new Color(0,255,0,0.5f);
		Gizmos.DrawWireSphere (transform.position, moveRadius);
	}
}
