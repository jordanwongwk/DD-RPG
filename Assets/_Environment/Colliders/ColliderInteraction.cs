using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.CameraUI;
using RPG.Characters;

public class ColliderInteraction : MonoBehaviour {

	[SerializeField] ColliderPosition colliderID;

	enum ColliderPosition { FrontVillage, BackVillage };

	UITextManager textManager;
	GameManager gameManager;
	GameObject player;
	List<string> interactionText = new List<string>();
	bool isInteracting = false;
	int interactionSequence;

	void Start(){
		textManager = FindObjectOfType<UITextManager> ();
		gameManager = FindObjectOfType<GameManager> ();
		player = FindObjectOfType<PlayerControl> ().gameObject;
	}

//	void OnTriggerEnter(Collider collider){
//		if (collider.gameObject == player) {
//			textManager.ShowInstruction ();
//		}
//	}

	void OnTriggerStay(Collider collider){

		if (collider.gameObject == player) {
			textManager.ShowInstruction ();
		}

		if (collider.gameObject == player && Input.GetKeyDown (KeyCode.F)) {
			if (!isInteracting) {
				StartInteraction ();
			} else if (isInteracting) {
				interactionSequence += 1;
				CheckIfInteractionEnds ();
			}
		}
	}

	void StartInteraction(){
		interactionSequence = 0;
		isInteracting = true;
		SettingUpText ();

		textManager.ShowNPCTextBox ();
		textManager.SetNPCConvoText (interactionText [interactionSequence]);
		player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (false);
	}

	void CheckIfInteractionEnds(){
		if (interactionSequence >= interactionText.Count) {
			EndInteraction ();
		} else {
			textManager.SetNPCConvoText (interactionText [interactionSequence]);
		}
	}

	void EndInteraction(){
		isInteracting = false;
		interactionText.Clear();

		textManager.DisableInstructionAndNPCTextBox ();
		player.GetComponent<PlayerControl> ().SetPlayerFreeToMove (true);
	}

	void SettingUpText(){
		switch (colliderID)
		{
		case ColliderPosition.BackVillage:
			if (gameManager.GetPhase1Info() == false) {
				interactionText.Add ("You: \nI'm here to pass the package to Derrick. I think the guy at the center of town is him.");
			} else if (gameManager.GetPhase2Info() == true) {
				interactionText.Add ("You: \nDerrick said that a dark knight is stationed in the castle, I should go check it out for now.");
			} else {
				interactionText.Add ("Error 404: Text not found.");
			}
			break;
		}
	}
}
