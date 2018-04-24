using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour, IDamageable{

	[SerializeField] float enemyLayer = 9;
	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float playerMeleeDamage = 10f;
	[SerializeField] float timeBetweenHits = 0.5f;
	[SerializeField] float attackRange = 1f;


	float timeLastHit;
	[SerializeField] float currentHealthPoints;
	GameObject currentTarget;
	CameraRaycaster cameraRayCaster;

	public float healthAsPercentage	{ get {	return currentHealthPoints / maxHealthPoints; }}

	void Start(){
		currentHealthPoints = maxHealthPoints;
		cameraRayCaster = FindObjectOfType<CameraRaycaster> ();
		cameraRayCaster.notifyMouseClickObservers += OnMouseClick;
	}

	void OnMouseClick (RaycastHit raycastHit, int layerHit){
		if (layerHit == enemyLayer) {
			var enemy = raycastHit.collider.gameObject;
			currentTarget = enemy;
			Enemy enemyComponent = enemy.GetComponent<Enemy> ();

			float distanceDiff = Vector3.Distance (transform.position, enemy.transform.position);
			if (Time.time - timeLastHit > timeBetweenHits && distanceDiff <= attackRange) {
				enemyComponent.TakeDamage (playerMeleeDamage);
				timeLastHit = Time.time;
			}
		}
	}

	public void TakeDamage (float damage){
		currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
	} 


}
