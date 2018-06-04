using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.CameraUI {
	public class UITextManager : MonoBehaviour {
		[SerializeField] GameObject skillDescriptionTextBar = null;
		[SerializeField] GameObject questTrackerTextBar = null;
		[SerializeField] GameObject NPCConvoInstruction = null;
		[SerializeField] GameObject convoTextBar = null;

		Text skillText;
		Text questText;
		Text instructionText;
		Text convoText;

		// Use this for initialization
		void Start () {
			skillDescriptionTextBar.SetActive (false);	
			skillText = skillDescriptionTextBar.GetComponentInChildren<Text> ();
			questText = questTrackerTextBar.GetComponentInChildren<Text> (); 
			instructionText = NPCConvoInstruction.GetComponent<Text> ();
			convoText = convoTextBar.GetComponentInChildren<Text> ();

			Quest01 ();
		}

		// Conversation Tracker
		public void ShowInstruction(){
			NPCConvoInstruction.SetActive (true);
			instructionText.text = "Press F to Talk";
		}

		public void ShowNPCTextBox(){
			convoTextBar.SetActive (true);
		}

		public void SetNPCConvoText(string NPCText){
			convoText.text = NPCText;
		}

		public void DisableInstructionAndNPCTextBox(){
			NPCConvoInstruction.SetActive (false);
			convoTextBar.SetActive (false);
		}
			


		// Quest Tracker
		public void Quest01(){
			questText.text = "- Head to the village on top of the hill to the west.\n- Deliver mysterious parcel to Derrick.";
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
