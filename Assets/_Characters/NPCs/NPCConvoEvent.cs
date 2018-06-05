using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters{
	public class NPCConvoEvent : MonoBehaviour {

		[SerializeField] NPCName NPCIdentity;
		[SerializeField] float distanceToPlayerToTrigger = 2f;

		public enum NPCName {DockGuy, Derrick, HutGuy, Merlin, TavernOwner, EscapedGuy };

		const float OFFSET_TO_CLEAR_CONVO = 0.5f;

		GameObject player;
		UITextManager textManager;
		GameManager gameManager;
		List<string> NPCText = new List<string>();
		int convoSequence;
		bool startConversation = false;
		bool storyPhase1Done = false;
		bool storyPhase2Done = false;

		// Normal NPC Conversation Change
		bool dockGuyInitialChat = false;

		void Start () {
			player = FindObjectOfType<PlayerControl> ().gameObject;
			textManager = FindObjectOfType<UITextManager> ();
			gameManager = FindObjectOfType<GameManager> ();
		}

		void Update(){
			float distanceDifferenceToPlayer = Vector3.Distance (transform.position, player.transform.position);
			if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger) {
				textManager.ShowInstruction ();
			} else if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger + OFFSET_TO_CLEAR_CONVO && distanceDifferenceToPlayer > distanceToPlayerToTrigger) {
				EndConvoWithNPC ();
			} 
			// The "else if" statement is to clear the conversation state when player pass by the circle.
			// Also functions to avoid clearing of conversation box caused by other NPC's scripts.

			if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger && Input.GetKeyDown (KeyCode.F)) {
				if (!startConversation) {
					StartConvoWithNPC ();
				} else if (startConversation) {
					convoSequence += 1;
					CheckIfConvoEnd ();
				}
			}
		}

		void CheckIfConvoEnd ()
		{
			// Triggers only when conversation successfully ended in a natural way
			if (convoSequence >= NPCText.Count) {
				EndConvoWithNPC ();
				CheckStoryProgress ();
			} else {
				textManager.SetNPCConvoText (NPCText [convoSequence]);
			}
		}

		void StartConvoWithNPC(){
			startConversation = true;
			convoSequence = 0;
			SettingUpConvoText ();

			textManager.ShowNPCTextBox ();
			textManager.SetNPCConvoText (NPCText [convoSequence]);
			player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (false);
		}

		void EndConvoWithNPC ()
		{
			startConversation = false;
			NPCText.Clear ();

			textManager.DisableInstructionAndNPCTextBox ();
			player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (true);
		}

		void CheckStoryProgress ()
		{
			if (storyPhase1Done) {
				gameManager.SetPhase1Done ();
			} else if (storyPhase2Done) {
				gameManager.SetPhase2Done ();
			}
		}


		void SettingUpConvoText(){
			switch (NPCIdentity) 
			{
			case NPCName.DockGuy:
				if (dockGuyInitialChat == false) {
					NPCText.Add ("Man: \nHeard a lot of commotion in town. Is everything alright?");
					NPCText.Add ("You: \nSome weird people in hood and the townsman start attacking me for no reason. I'm not even sure why.");
					NPCText.Add ("Man: \nSounds like hypnotize spell to me. You should be careful and head back to the city if you can.");
					NPCText.Add ("You: \nThanks but I'm heading to Kalm, which way should I go?");
					NPCText.Add ("Man: \nOnly way is up the hill. Take the rod over there, it should be lighter compared to your hoe. Should be handy.");
					NPCText.Add ("You: \nThanks a lot!");
					NPCText.Add ("Man: \nDo be careful, the day grows dark and a lot of weird stuff been going on around. Keep your eyes wide open and watch your back.");

					dockGuyInitialChat = true;
				} else {
					NPCText.Add ("Man: \nKalm is up the hill to the west. Be careful and stay safe.");
				}
				break;

			case NPCName.Derrick:
				if (gameManager.GetPhase1Info() == false) {
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

					storyPhase1Done = true;	
				} else {
					NPCText.Add ("Derrick: \nBe extra careful boy");
				}
				break;

			case NPCName.HutGuy:
				NPCText.Add ("Man: \nI've heard of a news saying that a thief stole a legendary dagger from the palace and escape to the plains here few months ago.");
				NPCText.Add ("Man: \nThe knights and archers pursued him and with a lot of fatal wounds, he died floating on the river nearby.");
				NPCText.Add ("Man: \nThe knights recovered the body but no one is able to find the dagger. Not even in the river and the rest is history.");
				NPCText.Add ("Man: \nA lot of people are offering high price for the dagger but heck even I can't find when I'm here all the time.");
				break;

			case NPCName.Merlin:
				if (gameManager.GetPhase2Info () == false) {
					NPCText.Add ("Merlin: \nWhat in the blaze is a young boy doing here and this time of the day?");
					NPCText.Add ("You: \nI'm looking for missing villagers from Kalm, do you have any idea where they went? I'm trying to rescue them.");
					NPCText.Add ("Merlin: \nAre you sane, boy? What you are facing is not ordinary threat, not some slime in a local forest.");
					NPCText.Add ("You: \nBeen fighting some of them shady people and bandits on my way here so I'm pretty confident.");
					NPCText.Add ("Merlin: \nWell guess I could give you what I see here.");
					NPCText.Add ("Merlin: \nI've seen a lot of captured men and women being escorted to the west. Down the forest trail to the village of Cornelia.");
					NPCText.Add ("You: \nIs there a castle near there?");
					NPCText.Add ("Merlin: \nCastle? If you mean the abandoned, haunted castle across the river of the village then yes, there is one.");
					NPCText.Add ("You: \nThat is exactly what I need to know! Thanks, kind sir.");
					NPCText.Add ("Merlin: \nWhile you are heading here, apart from the blasted hoody people and bandits, you did not see their leader?");
					NPCText.Add ("You: \nLeader? I'm not sure I've seen him. All the hostiles I have encountered so far are just normal hoodies and bandits.");
					NPCText.Add ("Merlin: \nI saw a young man guiding a hoard of his troops up to the hill to Kalm and captured the villagers as I've told you.");
					NPCText.Add ("Merlin: \nI don't like this one bit but I got a really bad feeling about this. Off you go, boy, I need to do some investigation.");
					NPCText.Add ("You: \n*I'm too having a bad feeling about this, I guess Derrick should be fine. I need to help him investigate the missing villagers first.*");
				
					storyPhase2Done = true;
				} else {
					NPCText.Add ("Merlin: \nAnything else, boy? I'm rather busy here.");
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
