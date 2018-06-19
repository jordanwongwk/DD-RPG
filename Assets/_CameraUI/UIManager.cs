using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;

namespace RPG.CameraUI {
	public class UIManager : MonoBehaviour {
		[SerializeField] GameObject skillDescriptionTextBar = null;
		[SerializeField] GameObject questTrackerTextBar = null;
		[SerializeField] GameObject interactionInstruction = null;
		[SerializeField] GameObject interactionTextBar = null;
		[SerializeField] GameObject healthRemainingGlobe = null;
		[SerializeField] GameObject energyRemainingGlobe = null;
		[SerializeField] GameObject gamePanel = null;
		[SerializeField] GameObject pausePanel = null;

		Text skillText;
		Text questText;
		Text instructionText;
		Text interText;
		Image gamePanelBackground;
		RawImage interPortrait;
		GameManager gameManager;
		MySceneManager mySceneManager;
		Color panelColor;

		bool isPausePanelActive = false;
		bool isPanelFadingIn = false;
		bool isPanelFadingOut = false;

		const float FADE_TIME = 3.0f;
		const float TIME_END_GAME = 5.0f;

		// Use this for initialization
		void Start () {
			skillText = skillDescriptionTextBar.GetComponentInChildren<Text> ();
			questText = questTrackerTextBar.GetComponentInChildren<Text> (); 
			instructionText = interactionInstruction.GetComponent<Text> ();
			interText = interactionTextBar.GetComponentInChildren<Text> ();
			interPortrait = interactionTextBar.GetComponentInChildren<RawImage> ();
			mySceneManager = FindObjectOfType<MySceneManager> ();

			gamePanelBackground = gamePanel.GetComponent<Image> ();
			panelColor = gamePanelBackground.color;
			panelColor.a = 1.0f;

			gameManager = FindObjectOfType<GameManager> ();
			gameManager.endGameSetup += PanelFadeOut;

			// Setting necessary window OFF
			skillDescriptionTextBar.SetActive (false);
			interactionInstruction.SetActive(false);
			interactionTextBar.SetActive (false);
			PanelFadeIn();
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
			QuestTrackerUpdate ();

			if (isPanelFadingOut) {
				StartCoroutine (FadingOutPanel ());
			} else if (isPanelFadingIn) {
				StartCoroutine (FadingInPanel ());
			}

			if (Input.GetKeyDown (KeyCode.Escape) && !isPausePanelActive) {
				PausingGame ();
			} else if (Input.GetKeyDown (KeyCode.Escape) && isPausePanelActive) {
				ResumingGame ();
			}
		}
			
		// Quest Tracker
		void QuestTrackerUpdate ()
		{
			if (gameManager.GetPhase1Info () == false) {
				QuestTrackerTextUpdate ("Head to the Village of Kalm and deliver the package to Derrick.");
			} else if (gameManager.GetPhase1Info () == true && gameManager.GetSecret2Info () == false) {
				QuestTrackerTextUpdate ("Head to the castle and find out what happened to the missing villagers.");
			} else if (gameManager.GetSecret2Info () == true && gameManager.GetSecret3Info () == false) {
				QuestTrackerTextUpdate ("Head back to town to warn the guards in Carconia about the impending attack!");
			} else if (gameManager.GetSecret3Info () == true) {
				QuestTrackerTextUpdate ("Chase after Derrick towards the direction of Cornelia's castle!");
			}
		}

		void QuestTrackerTextUpdate(string questObjective){
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
			skillText.text = "Heal [Key-2] ; Energy Cost = 30 \nRecover moderate amount of health.";
		}

		public void MouseOverSkillRM(){
			skillDescriptionTextBar.SetActive (true);
			skillText.text = "Power Attack [Right-click] ; Energy Cost = 20 \nEnchant energy into weapon and deal a powerful blow.";
		}

		public void MouseExit(){
			skillDescriptionTextBar.SetActive (false);
		}
		// END Skill Descriptions

		// Panel Fading
		public void PanelFadeIn(){
			gamePanel.SetActive (true);
			isPanelFadingIn = true;
		}

		IEnumerator FadingInPanel(){
			panelColor.a -= Time.deltaTime / FADE_TIME;
			gamePanelBackground.color = panelColor;
			yield return new WaitForSeconds (FADE_TIME);
			if (isPanelFadingIn) {					// Need to call this so the other called FadingInPanel coroutine won't keep disable the fade panel
				gamePanel.SetActive (false);
			}
			isPanelFadingIn = false;
		}

		public void PanelFadeOut(){
			gamePanel.SetActive (true);
			isPanelFadingOut = true;
		}

		IEnumerator FadingOutPanel(){
			panelColor.a += Time.deltaTime / FADE_TIME;
			gamePanelBackground.color = panelColor;
			yield return null;
		}
		// END Panel Fading

		// Pause Panel
		void PausingGame () {
			pausePanel.SetActive (true);
			isPausePanelActive = true;
			Time.timeScale = 0f;
		}

		public void ResumingGame() {
			pausePanel.SetActive (false);
			isPausePanelActive = false;
			Time.timeScale = 1f;
		}

		public void BackToMainMenu () {
			StartCoroutine (BackToMainMenuCoroutine ());
		}

		IEnumerator BackToMainMenuCoroutine() {
			ResumingGame ();
			PanelFadeOut ();
			yield return new WaitForSeconds (TIME_END_GAME);
			mySceneManager.MainMenuScene ();
		}
		// END Pause Panel

		// START Health and Energy Remaining Text
		public void MouseOverHPGlobe (){
			healthRemainingGlobe.SetActive (true);
		}

		public void MouseOverEnergyGlobe () {
			energyRemainingGlobe.SetActive (true);
		}

		public void MouseExitGlobe () {
			healthRemainingGlobe.SetActive (false);
			energyRemainingGlobe.SetActive (false);
		}
	}
}
