using UnityEngine;

namespace RPG.Characters{
	public class AudioTrigger : MonoBehaviour {

		[SerializeField] AudioClip clip = null;
		[SerializeField] float distanceToPlayerToTrigger = 5f;
		[Range(0, 1.0f)][SerializeField] float audioSpatialBlend = 0f;
		[Range(0, 1.0f)][SerializeField] float audioVolume = 0.8f;
		[SerializeField] bool isOneTimeOnly = false;
		[SerializeField] bool isLooping = false;
		[SerializeField] bool simple3DVolume = false;
			
		bool hasPlayed = false;
		AudioSource audioSource;
		GameObject player;

		void Start () {
			audioSource = gameObject.AddComponent<AudioSource> ();
			audioSource.playOnAwake = false;
			audioSource.spatialBlend = audioSpatialBlend;
			audioSource.volume = audioVolume;
			audioSource.loop = isLooping;
			audioSource.clip = clip;
			player = FindObjectOfType<PlayerControl> ().gameObject;
		}
		
		void Update(){
			float distanceDifferenceToPlayer = Vector3.Distance (transform.position, player.transform.position);
			if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger) {
				RequestPlayAudioClip ();
				if (simple3DVolume) {
					float realtimeVolume = audioVolume - ((distanceDifferenceToPlayer / distanceToPlayerToTrigger) * audioVolume);
					audioSource.volume = realtimeVolume;
				}
			} else {
				audioSource.Stop ();
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
