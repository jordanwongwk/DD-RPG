using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters{
	public class NPCConvoEvent : MonoBehaviour {

		[SerializeField] int NPCIdentityID = 0;
		[SerializeField] float distanceToPlayerToTrigger = 5f;

		GameObject player;
		UITextManager textManager;
		List<string> NPCText = new List<string>();
		int convoSequence;
		bool startConversation = false;


		void Start () {
			player = FindObjectOfType<PlayerControl> ().gameObject;
			textManager = FindObjectOfType<UITextManager> ();

			SettingUpConvoText ();
		}

		void Update(){
			float distanceDifferenceToPlayer = Vector3.Distance (transform.position, player.transform.position);
			if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger) {
				textManager.ShowInstruction ();
			} else if (distanceDifferenceToPlayer > distanceToPlayerToTrigger) {
				textManager.DisableInstructionAndNPCTextBox ();
			}

			if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger && Input.GetKeyDown (KeyCode.F)) {
				if (!startConversation) {
					StartConvoWithNPC ();
					Debug.Log (NPCText.Count);
				} else if (startConversation) {
					convoSequence += 1;
					ManageConversation ();
				}
			}
		}

		void ManageConversation ()
		{
			if (convoSequence >= NPCText.Count) {
				EndConversation ();
			} else {
				textManager.SetNPCConvoText (NPCText [convoSequence]);
			}
		}

		void EndConversation ()
		{
			startConversation = false;
			textManager.DisableInstructionAndNPCTextBox ();
			// Can Move
		}

		void StartConvoWithNPC(){
			startConversation = true;
			convoSequence = 0;
			textManager.ShowNPCTextBox ();
			textManager.SetNPCConvoText (NPCText [convoSequence]);
			// Stop Moving Player
		}

		void SettingUpConvoText(){
			if (NPCIdentityID == 0) {
				NPCText.Add("Man:\n I'm feeling so tired.");
				NPCText.Add("You:\n Take a break man, its a long day.");
			}

			if (NPCIdentityID == 1) {
				Debug.Log ("HELP");
			}
		}

		void OnDrawGizmos () {
			Gizmos.color = new Color(0,0,255f, 0.5f);
			Gizmos.DrawWireSphere (transform.position, distanceToPlayerToTrigger);
		}
	}
}
