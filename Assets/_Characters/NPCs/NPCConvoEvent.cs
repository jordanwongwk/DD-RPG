using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters{
	public class NPCConvoEvent : MonoBehaviour {

		[SerializeField] NPCName NPCIdentity;
		[SerializeField] float distanceToPlayerToTrigger = 5f;

		public enum NPCName {DockGuy, Derrick, HutGuy, Merlin, TavernOwner, EscapedGuy };

		GameObject player;
		UITextManager textManager;
		LevelManager levelManager;
		List<string> NPCText = new List<string>();
		int convoSequence;
		bool startConversation = false;

		void Start () {
			player = FindObjectOfType<PlayerControl> ().gameObject;
			textManager = FindObjectOfType<UITextManager> ();
			levelManager = FindObjectOfType<LevelManager> ();
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
			NPCText.Clear ();
			textManager.DisableInstructionAndNPCTextBox ();
			player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (true);
		}

		void StartConvoWithNPC(){
			startConversation = true;
			convoSequence = 0;
			SettingUpConvoText ();
			textManager.ShowNPCTextBox ();
			textManager.SetNPCConvoText (NPCText [convoSequence]);
			player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (false);
		}

		void SettingUpConvoText(){
			switch (NPCIdentity) 
			{
			case NPCName.DockGuy:
				NPCText.Add ("Man: \nI'm feeling so tired.");
				NPCText.Add ("You: \nTake a break man, its a long day.");
				break;

			case NPCName.Derrick:
				if (levelManager.GetPhase1Info() == false) {
					NPCText.Add ("You: \nAre you Derrick? I'm here for a delivery.");
					NPCText.Add ("Derrick: \nIndeed I am. Oh, I've been waiting for this. Thank you. You did not peek into it right?");
					NPCText.Add ("You: \nI didn't, sir. Its very rude to do so.");
					NPCText.Add ("Derrick: \nHaha just joking, its not something big. You must be the boy Sieghart always mentioned.");
					NPCText.Add ("You: \nI'm honoured, sir. My guild master always treat me as his son.");
					NPCText.Add ("Derrick: \nSounds like good, old Sieghart to me. Hahaha.");
					NPCText.Add ("You: \nSir, if you don't mind me asking, what happened to the town? It seems awfully quiet.");
					NPCText.Add ("Derrick: \nThose hooded guys invaded the town during daytime. I am the only survivor left.");
					NPCText.Add ("You: \nThat's awful. I'm glad you are safe, sir. But where are the people?");
					NPCText.Add ("Derrick: \nI've eavesdropped the guards during the raid, they seemed to be bringing them to the dungeons in a castle south of here.");
					NPCText.Add ("Derrick: \nI wanted to check it out but I got my hands full in guarding the town.");
					NPCText.Add ("You: \nLet me help. I'll try to investigate for you.");
					NPCText.Add ("Derrick: \nI'm grateful but its already this late and the road there is crawling with bandits and the hoodie rats.");
					NPCText.Add ("Derrick: \nSieg would kill me if you did not make it back safely.");
					NPCText.Add ("You: \nTrust me! I'm stronger than I looked.");
					NPCText.Add ("Derrick: \nIf you insists, I've already asked a few town guards to help me with the investigation.");
					NPCText.Add ("Derrick: \nYou should be able to meet them there at the castle.");
					NPCText.Add ("You: \nGot it!");
					NPCText.Add ("Derrick: \nBe careful, I don't want my head being cut off by Sieg if something happened to you.");

					levelManager.SetPhase1Done ();
				} else {
					NPCText.Add ("Derrick: \nBe extra careful boy");
				}
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
