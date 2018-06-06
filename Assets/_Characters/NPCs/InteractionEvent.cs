using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters{
	public class InteractionEvent : MonoBehaviour {

		[SerializeField] ObjectName objectIdentity;
		[SerializeField] float distanceToPlayerToTrigger = 2f;
		[SerializeField] float offsetToClearConvo = 0.5f;
		[Tooltip("Add 'Player' Portrait as elem 0; targetted 'NPC' as elem 1; others as elem 2 onwards")]
		[SerializeField] List<Texture> characterPortrait = new List<Texture>();

		public enum ObjectName { DockGuy, Derrick, HutGuy, Merlin, TavernOwner, EscapedGuy, BackVillage, FrontVillage, BossEvent, BossOptionalEvent, AxePickupPoint };

		const int MC_PORTRAIT = 0;
		const int NPC_PORTRAIT = 1;

		EventManager eventManager;
		GameObject player;
		UIManager uIManager;
		GameManager gameManager;
		List<Texture> interactPortrait = new List<Texture> ();
		List<string> interactText = new List<string>();
		int convoSequence;
		bool startConversation = false;
		bool inEvent = false;

		// Checking Story / Secret Progression
		bool storyPhase1Done = false;
		bool storyPhase2Done = false;
		bool secret1Done = false;
		bool secret2Done = false;


		// Normal NPC Conversation Change
		bool dockGuyInitialChat = false;
		bool tavernInitialChat = false;

		void Start () {
			player = FindObjectOfType<PlayerControl> ().gameObject;
			uIManager = FindObjectOfType<UIManager> ();
			gameManager = FindObjectOfType<GameManager> ();
			eventManager = FindObjectOfType<EventManager> ();
		}

		void Update(){
			float distanceDifferenceToPlayer = Vector3.Distance (transform.position, player.transform.position);
			if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger) {
				uIManager.ShowInstruction ();
			} else if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger + offsetToClearConvo && distanceDifferenceToPlayer > distanceToPlayerToTrigger) {
				EndInteraction ();
			} 
			// The "else if" statement is to clear the conversation state when player pass by the circle.
			// Also functions to avoid clearing of conversation box caused by other NPC's scripts.

			if (distanceDifferenceToPlayer <= distanceToPlayerToTrigger && Input.GetKeyDown (KeyCode.F)) {
				if (!startConversation) {
					CheckForEvent ();
					StartInteraction ();
				} else if (startConversation) {
					convoSequence += 1;
					CheckIfInteractionEnds ();
				}
			}
		}

		void CheckIfInteractionEnds ()
		{
			// Triggers only when conversation successfully ended in a natural way
			if (convoSequence >= interactText.Count) {
				EndInteraction ();
				CheckStoryProgress ();
			} else {
				uIManager.SetInteractionText (interactText [convoSequence]);
				uIManager.SetInteractionPortrait (interactPortrait [convoSequence]);
			}
		}

		void CheckForEvent(){
			if (objectIdentity == ObjectName.BackVillage && gameManager.GetPhase2Info () == true && gameManager.GetSecret1Info () == false && !inEvent) {
				inEvent = true;
				eventManager.StartSecretEvent1 ();
			}

			if (objectIdentity == ObjectName.BossEvent) {
				inEvent = true;
				eventManager.StartSecretEvent2 ();
			}

			if (objectIdentity == ObjectName.BossOptionalEvent) {
				inEvent = true;
				eventManager.StartSecretEvent3 ();
			}
		}

		void StartInteraction(){
			startConversation = true;
			convoSequence = 0;
			SettingUpConvoTextAndPortraits ();

			uIManager.ShowInteractionTextBox ();
			uIManager.SetInteractionText (interactText [convoSequence]);
			uIManager.SetInteractionPortrait (interactPortrait [convoSequence]);
			player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (false);
		}

		void EndInteraction ()
		{
			startConversation = false;
			interactText.Clear ();
			interactPortrait.Clear ();

			uIManager.DisableInstructionAndInterTextBox ();
			player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (true);

			if (inEvent) {
				eventManager.EndEvents ();
				inEvent = false;
			}
		}

		void CheckStoryProgress ()
		{
			if (storyPhase1Done) {
				gameManager.SetPhase1Done ();
			} else if (storyPhase2Done) {
				gameManager.SetPhase2Done ();
			} else if (secret1Done) {
				gameManager.SetSecret1Done ();
			} else if (secret2Done) {
				gameManager.SetSecret2Done ();
			} 

			// Secret 3 is done on optional boss death, managed by GameManager
		}


		void SettingUpConvoTextAndPortraits(){
			switch (objectIdentity) 
			{
			// NPC
			case ObjectName.DockGuy:
				if (dockGuyInitialChat == false) {
					interactText.Add ("Man: \nHeard a lot of commotion in town. Is everything alright?");
					interactText.Add ("You: \nSome weird people in hood and the townsman start attacking me for no reason. I'm not even sure why.");
					interactText.Add ("Man: \nSounds like hypnotize spell to me. You should be careful and head back to the city if you can.");
					interactText.Add ("You: \nThanks but I'm heading to Kalm, which way should I go?");
					interactText.Add ("Man: \nOnly way is up the hill. Take the rod over there, it should be lighter compared to your hoe. Should be handy.");
					interactText.Add ("You: \nThanks a lot!");
					interactText.Add ("Man: \nDo be careful, the day grows dark and a lot of weird stuff been going on around. Keep your eyes wide open and watch your back.");

					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);

					dockGuyInitialChat = true;
				} else {
					interactText.Add ("Man: \nKalm is up the hill to the west. Be careful and stay safe.");
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				}
				break;

			case ObjectName.Derrick:
				if (gameManager.GetPhase1Info() == false) {
					interactText.Add ("You: \nAre you Derrick? I'm here for a delivery.");
					interactText.Add ("Derrick: \nIndeed I am. Oh, I've been waiting for this. Thank you. You did not peek into it right?");
					interactText.Add ("You: \nI didn't, sir. Its very rude to do so.");
					interactText.Add ("Derrick: \nHaha just joking, its not something big. You must be the boy Sieghart always mentioned.");
					interactText.Add ("You: \nI'm honoured, sir. The guild master always treat me as his real son even he adopted me.");
					interactText.Add ("Derrick: \nSounds like good, old Sieghart to me. Hahaha.");
					interactText.Add ("You: \nSir, if you don't mind me asking, what happened to the town? It seems awfully quiet.");
					interactText.Add ("Derrick: \nThose hooded guys invaded the town during daytime. I am the only survivor left.");
					interactText.Add ("You: \nThat's awful. I'm glad you are safe, sir. But where are the people?");
					interactText.Add ("Derrick: \nI've eavesdropped the guards during the raid, they seemed to be bringing them to the dungeons in a castle south of here.");
					interactText.Add ("Derrick: \nI wanted to check it out but I got my hands full in guarding the town.");
					interactText.Add ("You: \nLet me help. I'll try to investigate for you.");
					interactText.Add ("Derrick: \nI'm grateful but its already this late and the road there is crawling with bandits and the hoodie rats.");
					interactText.Add ("Derrick: \nSieg would kill me if you did not make it back safely.");
					interactText.Add ("You: \nTrust me! I'm stronger than I looked.");
					interactText.Add ("Derrick: \nIf you insists, I've already asked a few town guards to help me with the investigation.");
					interactText.Add ("Derrick: \nYou should be able to meet them there at the castle.");
					interactText.Add ("You: \nGot it!");
					interactText.Add ("Derrick: \nBe careful, I don't want my head being cut off by Sieg if something happened to you.");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);

					storyPhase1Done = true;	
				} else {
					interactText.Add ("Derrick: \nBe extra careful boy");
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				}
				break;

			case ObjectName.HutGuy:
				interactText.Add ("Man: \nI've heard of a news saying that a thief stole a legendary dagger from the palace and escape to the plains here few months ago.");
				interactText.Add ("Man: \nThe knights and archers pursued him and with a lot of fatal wounds, he died floating on the river nearby.");
				interactText.Add ("Man: \nThe knights recovered the body but no one is able to find the dagger. Not even in the river and the rest is history.");
				interactText.Add ("Man: \nA lot of people are offering high price for the dagger but heck even I can't find when I'm here all the time.");

				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				break;

			case ObjectName.Merlin:
				if (gameManager.GetPhase2Info () == false) {
					interactText.Add ("Merlin: \nWhat in the blaze is a young boy doing here and this time of the day?");
					interactText.Add ("You: \nI'm looking for missing villagers from Kalm, do you have any idea where they went? I'm trying to rescue them.");
					interactText.Add ("Merlin: \nAre you sane, boy? What you are facing is not ordinary threat, not some slime in a local forest.");
					interactText.Add ("You: \nI've been fighting some of them shady people and bandits on my way here so I'm pretty confident.");
					interactText.Add ("Merlin: \nWell guess I could give you what I see here.");
					interactText.Add ("Merlin: \nI've seen a lot of captured men and women being escorted to the west. Down the forest trail to the village of Cornelia.");
					interactText.Add ("You: \nIs there a castle near there?");
					interactText.Add ("Merlin: \nCastle? If you mean the abandoned, haunted castle across the river of the village then yes, there is one.");
					interactText.Add ("You: \nThat is exactly what I need to know! Thanks, kind sir.");
					interactText.Add ("Merlin: \nWhile you are heading here, apart from the blasted hoody people and bandits, did you saw their leader with them?");
					interactText.Add ("You: \nLeader? I'm not sure I've seen him. All the hostiles I have encountered so far are just normal hoodies and bandits.");
					interactText.Add ("Merlin: \nI saw a young man guiding a hoard of his troops up to the hill to Kalm and captured the villagers as I've told you.");
					interactText.Add ("Merlin: \nI don't like this one bit but I got a really bad feeling about this. Off you go, boy, I need to do some investigation.");
					interactText.Add ("You: \n*I'm too having a bad feeling about this, I guess Derrick should be fine. I need to help him investigate the missing villagers first.*");
				
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);

					storyPhase2Done = true;
				} else {
					interactText.Add ("Merlin: \nAnything else, boy? I'm rather busy here.");
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				}
				break;

			case ObjectName.TavernOwner:
				if (tavernInitialChat == false && gameManager.GetPhase2Info() == true && gameManager.GetSecret4Info () == false) {
					interactText.Add ("Tavern Owner: \nOh, if it isn't Sieghart's adopted son. How are you doing?");
					interactText.Add ("You: \nHow do you know me? I believed we never met before.");
					interactText.Add ("Tavern Owner: \nThis is a tavern, boy. A place filled with information. Even the deepest secret could possibly be revealed here.");
					interactText.Add ("You: \nDo you know anything about these shady groups of people and the bandits collaboration?");
					interactText.Add ("Tavern Owner: \nWell, not that deep. But I do know Derrick has been leading them here from south of here a few days ago.");
					interactText.Add ("You: \nDerrick? You mean the guy who stay in Kalm?");
					interactText.Add ("Tavern Owner: \nI don't think he came from Kalm but if you saw him in Kalm a moment ago then that's the guy. He led a raid this afternoon.");
					interactText.Add ("You: \nThanks for the info, sir.");
					interactText.Add ("Tavern Owner: \nWatch your back out there boy. Its dangerous at this hour.");

					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);

					if (gameManager.GetPhase2Info () == true && gameManager.GetSecret1Info () == false) {
						interactText.Add ("You: \n*So that's the leader that the old man mentioned to me earlier? I guess I should go take a look in Kalm for a while.*");
						interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					} else if (gameManager.GetSecret1Info () == true) {
						interactText.Add ("You: \n*Guess I can confirm that Derrick is the main culprit for the raid. I should try reason with this 'Dark Knight' of his.*");
						interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					}

					tavernInitialChat = true;
				} else {
					interactText.Add ("Tavern Owner: \nWatch your back out there boy. Its dangerous at this hour.");
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				}
				break;

			case ObjectName.EscapedGuy:
				interactText.Add ("Man: \nThey brought the people to the dungeon of the castle up ahead. The entrance is to the left pass the main gate into the woods.");
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				break;


			// Non-Living Objects
			case ObjectName.BackVillage:
				if (gameManager.GetPhase1Info () == false) {
					interactText.Add ("You: \nI'm here to pass the package to Derrick. I think the guy at the center of town is him.");
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				} else if (gameManager.GetPhase2Info () == true && gameManager.GetSecret1Info () == false) {
					interactText.Add ("You: \nWait someone is talking to Derrick.");
					interactText.Add ("Derrick: \nHave we completely spread our 'religion' to all the villagers down at the barn and the gate village?");
					interactText.Add ("You: \nHe must have meant the village where the villagers attacked me for no reason.");
					interactText.Add ("Minion: \nYes, sir. Those who accepted Magnados are being equipped with proper gear. But those who don't are transported to the hideout.");
					interactText.Add ("You: \nMagnados?");
					interactText.Add ("Minion: \nAlso a boy passed by earlier and incapacitated some of our men.");
					interactText.Add ("Derrick: \nYes, I've met the boy. I led him to the hideout and the dark knight will deal with the rest. Along with those captured villagers.");
					interactText.Add ("Minion: \nThe dark knight as in the executioner?");
					interactText.Add ("Derrick: \nMore of a loyal servant to me and Magnados. The dark knight's 'charming' aura will certainly put those who denied us to join us.");
					interactText.Add ("Minion: \nIf they... don't?");
					interactText.Add ("Derrick: \nThey die, simple as that. You ask too many questions, return to the search party at once.");
					interactText.Add ("Minion: \nNoted, sir.");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);

					secret1Done = true;
				} else if (gameManager.GetPhase2Info () == true && gameManager.GetSecret1Info () == true) {		// Phase2Done and Secret1Done
					interactText.Add ("You: \nSo it seems that Derrick and this Magnados guy are kidnapping people around here.");
					interactText.Add ("You: \nI should try heading to the castle. Maybe the dark knight could give me some answers.");
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				} else {
					interactText.Add ("Error 404: Text not found.");
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				}
				break;

			case ObjectName.BossEvent:
				interactText.Add ("You: \nSo you must be the dark knight Derrick mentioned.");
				interactText.Add ("Dark Knight: \nIt seems like you know me and our leader's identity. This makes things easier.");
				interactText.Add ("You: \nTell me, what have you done to the villagers? What are your cult is doing?");
				interactText.Add ("Dark Knight: \nJust following orders, bring them to the dungeons, tortured them to join us and embraced themselves to Magnados.");
				interactText.Add ("Dark Knight: \nThe stubborn ones, however, get executed by me. Orders from Derrick too, he's an impatient man.");
				interactText.Add ("Dark Knight: \nAs for what we are doing, to fulfill Magnados' prophecy of course. Starting with raiding the city.");
				interactText.Add ("You: \nWhat?! He's planning to raid the city?!");
				interactText.Add ("Dark Knight: \nOh, you didn't know? Well, guess you are gonna die here anyway so let me just tell you the whole story.");
				interactText.Add ("Dark Knight: \nDerrick raided towns after towns and forces people to serve him and Magnados. Those who obey are spared and join us.");
				interactText.Add ("Dark Knight: \nRaiding a whole city is not as easy as raiding a town like this. That's why he's gathering explosives now up north.");
				interactText.Add ("You: \nSo the item that I delievered to him was...");
				interactText.Add ("Dark Knight: \nProbably a detonator.");
				interactText.Add ("You: \n! I have to warn the people!");
				interactText.Add ("Dark Knight: \nNot on my watch! En garde!");

				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);

				secret2Done = true;
				break;

			case ObjectName.BossOptionalEvent:
				interactText.Add ("You: \nDerrick!!!");
				interactText.Add ("Derrick: \nMy my, you certain come at a poor timing.");

				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);

				break;

			default:
				interactText.Add ("Invalid Interaction, did you forget to give this thing text?");
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				break;
			}
		}

		void OnDrawGizmos () {
			Gizmos.color = new Color(0,0,255f, 0.5f);
			Gizmos.DrawWireSphere (transform.position, distanceToPlayerToTrigger);
		}
	}
}
