﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour {
	[SerializeField] GameObject gamePanel = null;
	[SerializeField] GameObject quitPanel = null;

	Color panelColor;
	Image gamePanelBackground;
	bool isPanelFadingIn = false;
	bool isQuitPanelActive = false;
	const float FADE_TIME = 3.0f;

	// Use this for initialization
	void Start () {
		gamePanelBackground = gamePanel.GetComponent<Image> ();
		panelColor = gamePanelBackground.color;
		panelColor.a = 1.0f;
		quitPanel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (isPanelFadingIn) {
			StartCoroutine (FadingInPanel ());
		} 
	}

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

	public void OnPressPrompExitPanel(){
		if (!isQuitPanelActive) {
			quitPanel.SetActive (true);
			isQuitPanelActive = true;
		}
	}

	public void CancelExit () {
		quitPanel.SetActive (false);
		isQuitPanelActive = false;
	}
}
