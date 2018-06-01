using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextManager : MonoBehaviour {
	[SerializeField] GameObject skillDescriptionTextBar = null;

	Text skillText;

	// Use this for initialization
	void Start () {
		skillDescriptionTextBar.SetActive (false);	
		skillText = skillDescriptionTextBar.GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
