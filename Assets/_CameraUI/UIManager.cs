using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.CameraUI {
	public class UIManager : MonoBehaviour {
		[SerializeField] GameObject skillDescriptionTextBar = null;
		[SerializeField] GameObject questTrackerTextBar = null;
		[SerializeField] GameObject interactionInstruction = null;
		[SerializeField] GameObject interactionTextBar = null;

		Text skillText;
		Text questText;
		Text instructionText;
		Text interText;
		RawImage interPortrait;
		GameManager gameManager;

		// Use this for initialization
		void Start () {
			skillText = skillDescriptionTextBar.GetComponentInChildren<Text> ();
			questText = questTrackerTextBar.GetComponentInChildren<Text> (); 
			instructionText = interactionInstruction.GetComponent<Text> ();
			interText = interactionTextBar.GetComponentInChildren<Text> ();
			interPortrait = interactionTextBar.GetComponentInChildren<RawImage> ();
			gameManager = FindObjectOfType<GameManager> ();

			// Setting necessary window OFF
			skillDescriptionTextBar.SetActive (false);
			interactionInstruction.SetActive(false);
			interactionTextBar.SetActive (false);

			QuestTrackerUpdate ("Head to the Village of Kalm and deliver the package to Derrick.");
		}

		// Conversation Tracker
		public void ShowInstruction(){
			interactionInstruction.SetActive (true);
			instructionText.text = "Press F to Interact";
		}

		public void ShowInteractionTextBox(){
			interactionTextBar.SetActive (true);
		}

		public void SetInteractionText(string NPCText){
			interText.text = NPCText;
		}

		public void SetInteractionPortrait (Texture speaker){
			interPortrait.texture = speaker;
		}

		public void DisableInstructionAndInterTextBox(){
			interactionInstruction.SetActive (false);
			interactionTextBar.SetActive (false);
		}
		// END Conversation Tracker

		void Update(){
			if (gameManager.GetPhase1Info() == true) {
				QuestTrackerUpdate ("Head to the castle to find out what happened to the missing villagers.");
			}
		}

		// Quest Tracker
		public void QuestTrackerUpdate(string questObjective){
			questText.text = questObjective;
		}
		// END Quest Tracker

		// Skill Descriptions
		public void MouseOverSkill1(){
			skillDescriptionTextBar.SetActive (true);
			skillText.text = "Explosion [Key-1] ; Energy Cost = 50 \nDeal explosive damage to nearby enemies.";
		}

		public void MouseOverSkill2(){
			skillDescriptionTextBar.SetActive (true);
			skillText.text = "Heal [Key-2] ; Energy Cost = 25 \nRecover moderate amount of health.";
		}

		public void MouseOverSkillRM(){
			skillDescriptionTextBar.SetActive (true);
			skillText.text = "Power Attack [Key-3] ; Energy Cost = 25 \nDeliver a much powerful blow by using energy.";
		}

		public void MouseExit(){
			skillDescriptionTextBar.SetActive (false);
		}
	}
}
