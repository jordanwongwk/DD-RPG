using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters{
	public class NPCConvoEvent : MonoBehaviour {

		[SerializeField] NPCName NPCIdentity;
		[SerializeField] float distanceToPlayerToTrigger = 5f;

		enum NPCName {DockGuy, Derrick, HutGuy, Merlin, TavernOwner, EscapedGuy };

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
			switch (NPCIdentity) 
			{
			case NPCName.DockGuy:
				NPCText.Add ("Man: \nI'm feeling so tired.");
				NPCText.Add ("You: \nTake a break man, its a long day.");
				break;

			case NPCName.Derrick:
				NPCText.Add ("Drunk Man: \nELLO BOI, WANT SOME ***GA?");
				NPCText.Add ("You: \nYou do know I can't add that to the game");
				break;

			default:
				NPCText.Add ("Invalid NPC");
				break;
			}
		}

		void OnDrawGizmos () {
			Gizmos.color = new Color(0,0,255f, 0.5f);
			Gizmos.DrawWireSphere (transform.position, distanceToPlayerToTrigger);
		}
	}
}
