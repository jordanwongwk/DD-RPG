using UnityEngine;

namespace RPG.Characters{
	public class AudioTrigger : MonoBehaviour {

		[SerializeField] AudioClip clip = null;
		[SerializeField] float distanceToPlayerToTrigger = 5f;
		[SerializeField] bool isOneTimeOnly = false;

		[SerializeField] bool hasPlayed = false;
		AudioSource audioSource;
		GameObject player;

		void Start () {
			audioSource = gameObject.AddComponent<AudioSource> ();
			audioSource.playOnAwake = false;
			audioSource.clip = clip;
			player = FindObjectOfType<PlayerControl> ().gameObject;
		}
		
		void Update(){
			float distanceDifferenceToPlayer = Vector3.Distance (transform.position, player.transform.position);
			if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger) {
				RequestPlayAudioClip ();
			}
		}

		void RequestPlayAudioClip(){
			if (isOneTimeOnly && hasPlayed) {
				return;
			} else if (audioSource.isPlaying == false) {
				audioSource.Play ();
				hasPlayed = true;
			}
		}

		void OnDrawGizmos () {
			Gizmos.color = new Color(0,0,255f, 0.5f);
			Gizmos.DrawWireSphere (transform.position, distanceToPlayerToTrigger);
		}
	}
}
