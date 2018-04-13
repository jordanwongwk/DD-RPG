using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyHealthBarScript : MonoBehaviour {

	Player player;
	Image image;
	// Use this for initialization
	void Start () {
		player = FindObjectOfType<Player> ();
		image = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		image.fillAmount = player.healthAsPercentage;
	}
}
