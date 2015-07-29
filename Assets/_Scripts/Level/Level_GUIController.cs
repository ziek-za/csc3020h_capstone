using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Level_GUIController : MonoBehaviour {

	public Text healthText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateHUD(int health){
		healthText.text = health.ToString();
	}
}
