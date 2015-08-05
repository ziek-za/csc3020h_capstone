using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Level_GUIController : MonoBehaviour {

	public Text healthText;
	public enum classes {SOLDIER, THIEF, BUILDER, NONE};
	public classes GUIClass = classes.NONE;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateHUD(int health){
		healthText.text = health.ToString();
	}

	public void onSoldierSelectButtonPress(){
		Char_SelectChar.classNo = 0;
		GUIClass = classes.SOLDIER;
		Debug.Log ("Selecting Soldier");
		}

	public void onThiefSelectButtonPress(){
		Char_SelectChar.classNo = 1;
		GUIClass = classes.THIEF;
		Debug.Log ("Selecting Thief");
	}

	public void onBuilderSelectButtonPress(){
		Char_SelectChar.classNo = 2;
		GUIClass = classes.BUILDER;
		Debug.Log ("Selecting Builder");
	}


}
