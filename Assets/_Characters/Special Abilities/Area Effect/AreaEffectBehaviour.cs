using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AreaEffectBehaviour : AbilityBehaviour {

		float channelTime;
		float xChange = 0;
		float zChange = 0;
		bool channelTimeIndication = false;
		GameObject targetCircleTop;

		public override void Use(GameObject target) {
			if ((config as AreaEffectConfig).GetRequiresChanneling () == true) {
				StartCoroutine (AbilityChanneling ());
			} else {
				ExecuteAbility ();
			}
		}

		IEnumerator AbilityChanneling(){
			channelTime = (config as AreaEffectConfig).GetChannelTime ();
			var dangerCircle = (config as AreaEffectConfig).GetDangerCircle ();
			var dangerCircleTop = (config as AreaEffectConfig).GetDangerCircleTop ();

			Vector3 circlePostion = new Vector3 (transform.position.x, (transform.position.y + 0.25f), transform.position.z);
			GameObject targetCircle = Instantiate (dangerCircle, circlePostion, Quaternion.identity);
			targetCircle.transform.parent = gameObject.transform;

			targetCircleTop = Instantiate (dangerCircleTop, circlePostion, Quaternion.identity);
			targetCircleTop.transform.parent = gameObject.transform;
			InitializeTopDangerCircle ();
			channelTimeIndication = true;

			GameObject particleSystemPrefab = null;
			if (config.GetChannelParticle () != null) {
				particleSystemPrefab = Instantiate (config.GetChannelParticle (), transform.position, Quaternion.identity);
				particleSystemPrefab.transform.parent = transform;
				particleSystemPrefab.GetComponent<ParticleSystem> ().Play ();
			}
				
			yield return new WaitForSeconds (channelTime);
			channelTimeIndication = false;
			Destroy (targetCircle);
			Destroy (targetCircleTop);
			Destroy (particleSystemPrefab);
			ExecuteAbility ();
		}

		void InitializeTopDangerCircle ()
		{
			Vector3 targetCircleTopInitialScale = new Vector3 (0f, 0.1f, 0f);
			targetCircleTop.transform.localScale = targetCircleTopInitialScale;
			xChange = 0;
			zChange = 0;
		}

		void ExecuteAbility ()
		{
			DealingRadialDamage ();
			PlayParticleEffect ();
			PlayAbilitySound ();
			PlayAbilityAnimation ();
		}

		void DealingRadialDamage ()
		{
			RaycastHit[] hits = Physics.SphereCastAll (
				transform.position, 
				(config as AreaEffectConfig).GetRadius (), 
				Vector3.up, 
				(config as AreaEffectConfig).GetRadius ()
			);
			foreach (RaycastHit hit in hits) {
				var damageable = hit.collider.gameObject.GetComponent<HealthSystem> ();
				if (damageable != null && hit.collider.tag != gameObject.tag) {
					float damageToDeal = (config as AreaEffectConfig).GetDamageEachTarget ();
					damageable.TakeDamage (damageToDeal);
				}
			}
		}

		void Update() {
			if (channelTimeIndication) {
				xChange += Time.deltaTime / channelTime;
				zChange += Time.deltaTime / channelTime;
				targetCircleTop.transform.localScale = new Vector3 (xChange, 0.1f, zChange);
			}
		}
	}
}
