﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyEnemyHealthBar : MonoBehaviour {

	Enemy enemy;
	Image image;
	// Use this for initialization
	void Start () {
		enemy = GetComponentInParent<Enemy> ();
		image = GetComponent<Image> ();
	}

	// Update is called once per frame
	void Update () {
		image.fillAmount = enemy.healthAsPercentage;
	}
}
