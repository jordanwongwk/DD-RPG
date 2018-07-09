using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.CameraUI {
	public class TutorialText : MonoBehaviour {

		[SerializeField] GameObject previousButton = null;
		[SerializeField] GameObject nextButton = null;
		[SerializeField] Text pageNumberText = null;
		[SerializeField] AudioClip pageFlipSound = null;

		int tutorialDisplayCount = 0;
		UIManager uiManager;
		AudioSource audioSource;

		void Start () {
			uiManager = FindObjectOfType<UIManager> ();
			audioSource = GetComponent<AudioSource> ();
			ManagingTutorialTextWindow ();
		}
		
		public void OnClickNext () {
			tutorialDisplayCount++;
			ManagingTutorialTextWindow ();
			audioSource.PlayOneShot (pageFlipSound);
		}

		public void OnClickPrevious () {
			tutorialDisplayCount--;
			ManagingTutorialTextWindow ();
			audioSource.PlayOneShot (pageFlipSound);
		}

		public void OnClickCloseWindow(){
			uiManager.OnStartCloseTutorialWindow ();
		}

		void ManagingTutorialTextWindow() {

			// Configure Next Button
			if (tutorialDisplayCount < transform.childCount - 1) {
				nextButton.GetComponentInChildren<Text> ().text = "Next >>";
			} else if (tutorialDisplayCount == transform.childCount - 1) {
				nextButton.GetComponentInChildren<Text> ().text = "Close";
			} else if (tutorialDisplayCount >= transform.childCount) {
				uiManager.OnStartCloseTutorialWindow ();
				return;
			}

			// Configure Previous Button
			if (tutorialDisplayCount <= 0) {
				previousButton.GetComponent<Button> ().interactable = false;
			} else {
				previousButton.GetComponent<Button> ().interactable = true;
			}

			// Configure Tutorial Text Window
			if (tutorialDisplayCount < transform.childCount) {
				for (int i = 0; i < transform.childCount; i++) {
					transform.GetChild (i).gameObject.SetActive (false);
				}
				transform.GetChild (tutorialDisplayCount).gameObject.SetActive (true);
				pageNumberText.text = (tutorialDisplayCount + 1) + " / " + transform.childCount;
			}
		}

		public void ResetAllCounts() {
			tutorialDisplayCount = 0;
			pageNumberText.text = (tutorialDisplayCount + 1) + " / " + transform.childCount;
			ManagingTutorialTextWindow ();
		}
	}
}
