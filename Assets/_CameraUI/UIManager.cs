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
		[SerializeField] GameObject tutorialPanel = null;
		[SerializeField] GameObject returnToMainMenuWindow = null;
		[SerializeField] GameObject endGameConfirmationWindow = null;
		[SerializeField] GameObject notificationTextBox = null;

		Text skillText;
		Text questText;
		Text instructionText;
		Text interText;
		Text notificationText;
		Image gamePanelBackground;
		RawImage interPortrait;
		GameManager gameManager;
		MySceneManager mySceneManager;
		Color panelColor;

		bool isPausePanelActive = false;
		bool isTutorialPanelActive = false;
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
			notificationText = notificationTextBox.GetComponent<Text> ();
			mySceneManager = FindObjectOfType<MySceneManager> ();

			gamePanelBackground = gamePanel.GetComponent<Image> ();
			panelColor = gamePanelBackground.color;
			panelColor.a = 1.0f;

			gameManager = FindObjectOfType<GameManager> ();
			gameManager.endGameSetup += PanelFadeOut;

			// Find Tutorial Panel Active
			if (tutorialPanel.activeInHierarchy == true){
				isTutorialPanelActive = true;
			}

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

			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (!isPausePanelActive && !isTutorialPanelActive) {
					PausingGame ();
				} else if (isPausePanelActive && !isTutorialPanelActive) {
					ResumingGame ();
				} else if (isTutorialPanelActive) {
					OnStartCloseTutorialWindow ();
				}
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

		public void SetMainMenuConfirmationWindowActive () {
			if (returnToMainMenuWindow.activeInHierarchy == false) {
				returnToMainMenuWindow.SetActive (true);
			}
		}

		public void CancelBackToMainMenu() {
			returnToMainMenuWindow.SetActive (false);
		}

		public void BackToMainMenu () {
			StartCoroutine (BackToMainMenuCoroutine ());
		}

		IEnumerator BackToMainMenuCoroutine() {
			returnToMainMenuWindow.SetActive (false);
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
		// END Health and Energy Remaining Text

		// START End Game Confirmation Window
		public void SetEndGameConfirmationActive(){
			if (endGameConfirmationWindow.activeInHierarchy == false) {
				endGameConfirmationWindow.SetActive (true);
			}
		}

		public void EndGameYes () {
			endGameConfirmationWindow.SetActive (false);
			gameManager.TriggerEndOfGame ();
		}

		public void EndGameNo (){
			endGameConfirmationWindow.SetActive (false);
			gameManager.SetIsGameEnding (false);
		}
		// END End Game Confirmation Window

		// Tutorial Panel
		public void OnStartCloseTutorialWindow() {
			tutorialPanel.SetActive (false);
			isTutorialPanelActive = false;
		}

		public void OnClickOpenTutorialWindow () {
			tutorialPanel.GetComponentInChildren<TutorialText> ().ResetAllCounts ();
			tutorialPanel.SetActive (true);
			isTutorialPanelActive = true;
		}
		// END Tutorial Panel

		// Notification text
		public void SecretFoundText (int numberOfSecretsFound){
			notificationText.text = "Secret found! ( " + numberOfSecretsFound + " / 4 )";
			StartCoroutine (DisplayNotificationText ());
		}

		public void WeaponFoundText (int numberOfWeaponsFound) {
			notificationText.text = "New weapon discovered! ( " + numberOfWeaponsFound + " / 5 )";
			StartCoroutine (DisplayNotificationText ());
		}

		public void CheckpointFoundText () {
			notificationText.text = "Checkpoint registered!";
			StartCoroutine (DisplayNotificationText ());
		}

		IEnumerator DisplayNotificationText () {
			notificationTextBox.SetActive (true);
			yield return new WaitForSecondsRealtime (5f);
			notificationTextBox.SetActive (false);
		}
		// END Notification text
	}
}
