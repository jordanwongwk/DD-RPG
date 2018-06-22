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

		public enum ObjectName { DockGuy, Derrick, HutGuy, Merlin, TavernOwner, EscapedGuy, 
								 BackVillage, FrontVillage, BossEvent, BossOptionalEvent, AxePickupPoint, EndingPoint};

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
		bool secret4Done = false;

		// One-Time Only
		bool dockGuyInitialChat = false;
		bool tavernInitialChat = false;
		bool pickedUpAxe = false;
		bool pickedUpAxeStatus = false;
		bool triggerEndGame = false;

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
			PlayingGreetingsClip ();
			SettingUpConvoTextAndPortraits ();

			uIManager.ShowInteractionTextBox ();
			uIManager.SetInteractionText (interactText [convoSequence]);
			uIManager.SetInteractionPortrait (interactPortrait [convoSequence]);
			player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (false);
		}

		void PlayingGreetingsClip(){
			if (GetComponent<Character> () == true) {
				AudioClip greetings = GetComponent<Character> ().GetGreetingsAudioClip ();
				AudioSource audioSource = GetComponent<AudioSource> ();
				audioSource.PlayOneShot (greetings);
			}
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
			if (gameManager.GetPhase1Info() == false && storyPhase1Done) {
				gameManager.SetPhase1Done ();
			} else if (gameManager.GetPhase2Info() == false && storyPhase2Done) {
				gameManager.SetPhase2Done ();
			} else if (gameManager.GetSecret1Info() == false && secret1Done) {
				gameManager.SetSecret1Done ();
			} else if (gameManager.GetSecret2Info() == false && secret2Done) {
				gameManager.SetSecret2Done ();
			} else if (gameManager.GetSecret4Info() == false && secret4Done) {
				gameManager.SetSecret4Done ();
			} else if (gameManager.GetIsGameEnding() == true) {
				uIManager.SetEndGameConfirmationActive ();
			}
			// Secret 3 is done on optional boss death, managed by GameManager

			if (pickedUpAxe) {
				pickedUpAxeStatus = true;
			}
		}


		void SettingUpConvoTextAndPortraits(){
			switch (objectIdentity) 
			{
			// NPC
			case ObjectName.DockGuy:
				if (dockGuyInitialChat == false) {
					interactText.Add ("Man: \nHeard a lot of commotion in town. Is everything alright?");
					interactText.Add ("You: \nSome weird people in hood and the townsman start attacking me for no reason. I'm not even sure why.");
					interactText.Add ("Man: \nSounds like hypnotize spell to me or defectors. You should be careful and head back to the city if you can.");
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
					interactText.Add ("You: \nI'm honoured, sir. The guild master always treat me as his real son even though he adopted me.");
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
					interactText.Add ("You: \nGot it! But wait, how do I get there?");
					interactText.Add ("Derrick: \nThe enemies have blocked the main road outside of town. There's a secret route that can lead you to the main road.");
					interactText.Add ("Derrick: \nThe secret route is behind the Shop, which is the building to the right of me. Go around it and you are back outside.");
					interactText.Add ("Derrick: \nThe castle is to the west down the hill. Cross the river and the road is pretty much straight forward.");
					interactText.Add ("You: \nThanks for the directions!");
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
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);

					storyPhase1Done = true;	
				} else {
					interactText.Add ("Derrick: \nSecret route is behind the building to the right of me. Get around it to exit to the main road.");
					interactText.Add ("Derrick: \nThe castle is to the west down the hill. Cross the river and the road is pretty much straight forward.");
					interactText.Add ("Derrick: \nBe prepared for the worst, boy. It is a dark night full of terrors.");
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
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
					interactText.Add ("Merlin: \nAnything else, boy? I'm rather busy here. You want to go Cornelia, its west of here. Turn right on the roundabout.");
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				}
				break;

			case ObjectName.TavernOwner:
				if (tavernInitialChat == false && gameManager.GetPhase2Info () == true && gameManager.GetSecret3Info () == false) {
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
						interactText.Add ("You: \n*So that's the leader that the wise man mentioned to me earlier? I guess I should go take a look in Kalm for a while.*");
						interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					} else if (gameManager.GetSecret1Info () == true) {
						interactText.Add ("You: \n*Guess I can confirm that Derrick is the main culprit for the raid. I should try reason with this 'Dark Knight' of his.*");
						interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					}

					tavernInitialChat = true;
				} else if (gameManager.GetSecret3Info () == true && gameManager.GetSecret4Info() == false && gameManager.GetSecret4ReadyInfo() == false) {
					interactText.Add ("Tavern Owner: \nYou need something boy? Got a feeling you have a question to me. Ask away.");
					interactText.Add ("You: \n*Maybe I can ask him if I can show him the item.*");

					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);

				} else if (gameManager.GetSecret3Info () == true && gameManager.GetSecret4Info() == false && gameManager.GetSecret4ReadyInfo() == true) {
					interactText.Add ("You: \nOld man, I heard you know a lot right? Tell me about this weapon.");
					interactText.Add ("Tavern Owner: \nHmm, this is no ordinary axe. Was this wielded by Derrick's dark knight?");
					interactText.Add ("You: \nYeah. The moment I hold it I felt strange, its very light and easy to hold but when I hit the floor, it felt like it weights a ton.");
					interactText.Add ("Tavern Owner: \n... I believed you know the answer yourself... If I'm not mistaken.");
					interactText.Add ("Tavern Owner: \nThis is one of the many Corrupted Axes. I guess in Derrick's party, only the Dark Knights are wielding it.");
					interactText.Add ("Tavern Owner: \nThe corrupted weapons will corrupt the person overtime. The longer the wield it, the more they are being consumed.");
					interactText.Add ("You: \n...");
					interactText.Add ("Tavern Owner: \nHmm... You are not Sieghart's son, aren't you. Who are you?");
					interactText.Add ("You(?): \n...");
					interactText.Add ("Tavern Owner: \nNo matter, you might already know this but I'm pretty sure the player of this game did not know this.");
					interactText.Add ("Tavern Owner: \nThat axe you are holding is a corrupted axe. No ordinary man can hold this axe. Like a hammer belonging to a thunder god.");
					interactText.Add ("Tavern Owner: \nThere is a catch, however, the strength itself is unchanged but the weight of the axe will become lighter depending on the wielder.");
					interactText.Add ("Tavern Owner: \nDepending on the wielder's sin.");
					interactText.Add ("You(?): \n...");
					interactText.Add ("Tavern Owner: \nEven though the weapon you are holding is not the true corrupted weapon and just an ordinary out of them.");
					interactText.Add ("Tavern Owner: \nBut the weapon itself is a lot times stronger than an ordinary weapon.");
					interactText.Add ("Tavern Owner: \nI'm confident that Sieghart's son won't be able to wield that axe entirely. Even impossible to swing it with ease.");
					interactText.Add ("Tavern Owner: \nIt seems you are holding quite a lot of sin there huh, 'boy'?");
					interactText.Add ("You(?): \n...");
					interactText.Add ("Tavern Owner: \nYou wanted to know where Derrick went, am I wrong? That's why you are here. You won't ask something you already know.");
					interactText.Add ("Tavern Owner: \nOh something interesting to tell you, Derrick has a lot of such replica corrupted weapons up in his arsenal.");
					interactText.Add ("You(?): \nHe's making fake weapons out of such pure power! The nerve of that human.");
					interactText.Add ("Tavern Owner: \nHmm... Fake? That's an interesting reply. Well, I can dig that information out of my own source.");
					interactText.Add ("Tavern Owner: \nAnyway, if you are asking for Derrick, I saw him running off into Cornelia. I'm placing my bet he went back to their hideout.");
					interactText.Add ("Tavern Owner: \nI have my eyes on you, boy. Don't get yourself killed. I'm sure Derrick is more dangerous now with those corrupted weapons.");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);

					secret4Done = true;

				} else if (gameManager.GetSecret4Info() == true){
					interactText.Add ("Tavern Owner: \nYou are nearly to the end. Good job on finding all the secrets in this prototype.");
					interactText.Add ("You(?): \nWhat?");
					interactText.Add ("Tavern Owner: \nI'm not talking to you. I'm talking to the person who's controlling you... And his/her friends standing at the side looking.");

					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
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
					interactText.Add ("You: \nI'm here to pass the package to Derrick. I think he is the guy in the center of town by the well.");
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				} else if (gameManager.GetPhase2Info () == true && gameManager.GetSecret1Info () == false) {
					interactText.Add ("You: \nWait someone is talking to Derrick.");
					interactText.Add ("Derrick: \nHave you completely search the barn and the village by the gate? It is a very vital location for our plan.");
					interactText.Add ("You: \nHe must have meant the village where the villagers attacked me for no reason. And 'plan'?");
					interactText.Add ("Minion: \nYes, sir. Those who obeyed and believed in us are being directed as planned. But those who don't are transported to the hideout.");
					interactText.Add ("Minion: \nAlso a boy passed by earlier and incapacitated some of our men while we are searching.");
					interactText.Add ("Derrick: \nYes, I've met the boy. He is a rather troublesome. Our plan is going to be at the peak and we still have someone to deal with.");
					interactText.Add ("Derrick: \nSieghart's timing can never be better. No matter, I led the boy to our hideout. The dark knight should know what to do.");
					interactText.Add ("Minion: \nThe dark knight as in the executioner?");
					interactText.Add ("Derrick: \nNow now, the dark knight is actually a loyal friend and general to me. You know he hates anyone who call him an executioner.");
					interactText.Add ("Minion: \nMy apologies, sir. Please don't let him know I said that, I don't want to see my head being chopped off by that axe of his.");
					interactText.Add ("Derrick: \nExactly, you don't want him to use that axe of his. Now, go continue the search, we are late on schedule.");
					interactText.Add ("Minion: \nYes, sir.");
					interactText.Add ("You: \nSo this dark knight who's waiting for me might know of this 'plan'. And I can't abandoned those who are captured now.");
					interactText.Add ("You: \nI'll stick to the current plan then. To the castle.");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [2]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);

					secret1Done = true;
				} else if (gameManager.GetPhase2Info () == true && gameManager.GetSecret1Info () == true) {		// Phase2Done and Secret1Done
					interactText.Add ("You: \nSo it seems that Derrick is kidnapping people around here.");
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
				interactText.Add ("You: \nTell me, what have you done to the villagers? What are you and your lackeys are doing?");
				interactText.Add ("Dark Knight: \nJust following orders, bring them to the dungeons, tortured them to join us and enjoy life with the empire.");
				interactText.Add ("Dark Knight: \nThe stubborn ones, however, get executed by me. Though I did not like that job much.");
				interactText.Add ("Dark Knight: \nAs for what we are doing, we're just going to raid the city and destroy the resistance.");
				interactText.Add ("You: \nWhat?! He's planning to raid the city?! And you guys are from the empire?!");
				interactText.Add ("Dark Knight: \nOh, you didn't know? Well, guess you are gonna die here anyway so let me just tell you the whole story.");
				interactText.Add ("Dark Knight: \nDerrick is the commander of the empire's army. The growing of resistance cause uneasiness to the emperor.");
				interactText.Add ("Dark Knight: \nWe were ordered to use any measures to destroy those pesky resistance in the city and surrounding towns.");
				interactText.Add ("Dark Knight: \nDerrick is a man of strategy. He decided to raid from a smaller scale, gain some allies then invade the city grounds.");
				interactText.Add ("Dark Knight: \nWe raided towns after towns and forced people to serve us and forget about any plans to rebel against the empire.");
				interactText.Add ("Dark Knight: \nRaiding a whole city is not as easy as raiding a town like this. That's why he's gathering explosives now up north.");
				interactText.Add ("You: \nSo the item that I delievered to him was...");
				interactText.Add ("Dark Knight: \nProbably a detonator.");
				interactText.Add ("You: \n! I have to warn the guards now!");
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
				interactText.Add ("Derrick: \nMy my, you certain come at a poor timing, boy. I'm a little busy here at the moment.");
				interactText.Add ("You: \nWhy are you doing this? Why are you in the empire? Aren't you dad's best friend?!");
				interactText.Add ("Derrick: \nBoy, I've been working with the empire for years now. You could ask your dad on that. I'm not a defector.");
				interactText.Add ("You: \nYou killed, murdered and even trying to bomb towns and cities just so cause your emperor can't sleep at night?!");
				interactText.Add ("Derrick: \n*Sigh* It seems like your twin brother is at it again, running his mouth.");
				interactText.Add ("Dark Knight: \nHe is not the type to keep secrets in the first place anyway.");
				interactText.Add ("Derrick: \nWell, what's done is done. And yes, I'm raiding the city to kill all those pesky resistance.");
				interactText.Add ("You: \nWhy won't you use a more peaceful measures in reducing such rebel? Killing the innocents seems...");
				interactText.Add ("Derrick: \nYou are too young to understand, boy. Fear is the best weapon to cleanse these corrupted humans.");
				interactText.Add ("Derrick: \nAnd the only way to successfully instill fear in a fighter is by death and destruction. Come boy, join us.");
				interactText.Add ("Derrick: \nThe emperor will pay you handsomely if the job is done. You could probably get your own villa in the city.");
				interactText.Add ("You: \nNo way.");
				interactText.Add ("Derrick: \nIt seems like I'm wasting my time and breath here then.");
				interactText.Add ("Derrick: \nYou see, I'm a rather busy man. I need to walk back to the hideout now to get my last preperation done.");
				interactText.Add ("Derrick: \nAnd you are definitely not getting in my way, boy. My dear knight, you know what to do.");
				interactText.Add ("Dark Knight: \nYes, indeed. I'm gonna enjoy this.");
				interactText.Add ("You: \nNo, wait! Get back here!");

				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [2]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [NPC_PORTRAIT]);
				interactPortrait.Add (characterPortrait [2]);
				interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				break;

			case ObjectName.AxePickupPoint:
				if (pickedUpAxeStatus == false) {
					interactText.Add ("You: \nThis weapon is dropped by the dark knight earlier. It seems rather odd-looking. Let me try take it.");
					interactText.Add ("You: \nOuch! There's a sudden spark. Let me give it a shot again.");
					interactText.Add ("You: \n...");
					interactText.Add ("You: \nNope, its too heavy. Guess I should give up...");
					interactText.Add ("You: \n...");
					interactText.Add ("You: \nOne more time... ...");
					interactText.Add ("You: \nGot it. Its not as heavy as first. I wondered why.");
					interactText.Add ("You: \nAnd this purple gem. Wonder what is it. I wonder if anyone knows about this weapon.");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);

					pickedUpAxe = true;
				} else {
					interactText.Add ("You: \nI wonder if anyone knows about this weapon. Its getting lighter now.");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				}
				break;

			case ObjectName.EndingPoint:
				if (gameManager.GetSecret4Info () == true) {
					interactText.Add ("You(?): \nThat Derrick guy must have gone through here.");
					interactText.Add ("You(?): \nI'm coming for you and your fake replicas. I'm gonna burn them all!");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				} else if (gameManager.GetSecret4Info () == false && gameManager.GetSecret3Info () == true) {
					interactText.Add ("You: \nI'm guessing Derrick went through here.");
					interactText.Add ("You: \nHere goes nothing.");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				} else if (gameManager.GetSecret3Info () == false) {
					interactText.Add ("You: \nSo, this is where the villagers are being taken to.");
					interactText.Add ("You: \nHang in there everyone. I'm coming to help!");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				} else {
					interactText.Add ("You: \nHere goes.");

					interactPortrait.Add (characterPortrait [MC_PORTRAIT]);
				}

				gameManager.SetIsGameEnding (true);
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
