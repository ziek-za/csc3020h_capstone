using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Level_GUIController : MonoBehaviour {

	public Text healthText, energyText;
	public enum classes {SOLDIER, THIEF, BUILDER, NONE};
	public classes GUIClass = classes.NONE;

	public GameObject shotIndicatorPivot, HUDPivot;
	public Image builderImage, thiefImage, soldierImage, shotIndicator,
		// SOLDIER
				soldierHUD, vortexIcon, explosionIcon,
		// THIEF
				thiefHUD,
		// BUILDER
				builderHUD;
	public HUD_AbilityIcon
		// SOLDIER
				vortexAndExplosionIcon,
		// THIEF
				teleportIcon, sprintIcon,
		// BUILDER
				linkIcon, turretIcon, boosterIcon;
	public Button linkButton;
	public Text playerNameLabel, playerKilledLabel;

	//Boolean for when the player is dead
	public bool isDeadBool = false;

	public GameObject mapCenter;

	// Use this for initialization
	void Start () {
		SetUpLinkButtons();
	}

	public void SetUpLinkButtons(){
		GameObject[] links = GameObject.FindGameObjectsWithTag("Link");
		//int yPos = 40;
		
		//Clear existing buttons
		foreach (Transform child in mapCenter.transform) {
			GameObject.Destroy(child.gameObject);
		}
		
		for (int i = 0; i < links.Length; i++){
			Vector3 linkPos = links[i].transform.position + new Vector3(Random.Range(2,5),2,Random.Range(2,5));
			Char_AttributeScript.Teams linkTeam = Char_AttributeScript.Teams.NONE;

			if (links[i].GetComponent<Map_LinkScript>())
				linkTeam = links[i].GetComponent<Map_LinkScript>().currentTeam;  //For proper links
			else if (links[i].GetComponent<Ability_BuilderLink>())
				linkTeam = links[i].GetComponent<Ability_BuilderLink>().currentTeam; //For builder links

			//Set up postion on screen and in heirarcy
			Vector3 buttonPos = new Vector3(links[i].transform.position.x*0.78125f,links[i].transform.position.z*0.78125f,0);
			Button tempButton = Instantiate(linkButton,Vector3.zero, Quaternion.identity) as Button;
			tempButton.transform.SetParent(mapCenter.transform);
			tempButton.transform.localPosition = buttonPos;
			tempButton.transform.localScale = new Vector3(1,1,1);
			//yPos -= 16;

			Debug.Log(tempButton.transform.lossyScale);

			//Set up text and colour
			//tempButton.GetComponentInChildren<Text>().text = "Link " + i;
			if (linkTeam == Char_AttributeScript.Teams.BLUE) {
				ColorBlock blueColors = new ColorBlock();
				blueColors.normalColor = Color.blue;
				blueColors.highlightedColor = Color.blue;
				blueColors.pressedColor = Color.cyan;
				blueColors.colorMultiplier = 1;
				tempButton.GetComponent<Button>().colors = blueColors;
				tempButton.interactable = true;
			} else if (linkTeam == Char_AttributeScript.Teams.RED){
				ColorBlock redColors = new ColorBlock();
				redColors.normalColor = Color.red;
				redColors.highlightedColor = Color.red;
				redColors.pressedColor = new Color(0.6f,0f,0f);
				redColors.colorMultiplier = 1;
				tempButton.GetComponent<Button>().colors = redColors;
				tempButton.interactable = true;
			} else {
				ColorBlock neutColors = new ColorBlock();
				neutColors.normalColor = Color.black;
				neutColors.disabledColor = Color.grey;
				neutColors.colorMultiplier = 1;
				tempButton.GetComponent<Button>().colors = neutColors;
				tempButton.interactable = false;
			}

			//Add action listener
			tempButton.onClick.AddListener(() => LinkSpawn(linkPos,linkTeam));
		}
	}

	// Update is called once per frame
	void Update () {
	}
	// Used to reset all cooldowns
	public void ResetIconsCooldown() {
		// SOLDIER
		vortexAndExplosionIcon.ResetCooldown ();
		// THIEF
		teleportIcon.ResetCooldown (); sprintIcon.ResetCooldown ();
		// BUILDER
		linkIcon.ResetCooldown (); turretIcon.ResetCooldown (); boosterIcon.ResetCooldown ();
	}
	
	public void UpdateHUDHealth(int health){
		healthText.text = health.ToString();
	}

	public void UpdateHUDEnergy(int energy){
		energyText.text = energy.ToString();
	}

	public void onSoldierSelectButtonPress(){
		//Char_SelectChar.classNo = 0;
		soldierImage.GetComponent<Image>().enabled = true;
		builderImage.GetComponent<Image>().enabled = false;
		thiefImage.GetComponent<Image>().enabled = false;
		GUIClass = classes.SOLDIER;
		Debug.Log ("Selecting Soldier");
	}

	public void onThiefSelectButtonPress(){
		//Char_SelectChar.classNo = 1;
		soldierImage.GetComponent<Image>().enabled = false;
		builderImage.GetComponent<Image>().enabled = false;
		thiefImage.GetComponent<Image>().enabled = true;
		GUIClass = classes.THIEF;
		Debug.Log ("Selecting Thief");
	}

	public void onBuilderSelectButtonPress(){
		//Char_SelectChar.classNo = 2;
		soldierImage.GetComponent<Image>().enabled = false;
		builderImage.GetComponent<Image>().enabled = true;
		thiefImage.GetComponent<Image>().enabled = false;
		GUIClass = classes.BUILDER;
		Debug.Log ("Selecting Builder");
	}

	void LinkSpawn(Vector3 spawnLoc, Char_AttributeScript.Teams team){

		Debug.Log("Button Clicked "+ team.ToString());

		Char_SelectChar.currentTeam = team;
		Char_SelectChar.spawnLocation = spawnLoc;

		switch (GUIClass){
			case classes.SOLDIER:
				Char_SelectChar.classNo = 0;
				break;
			case classes.THIEF:
				Char_SelectChar.classNo = 1;
				break;
			case classes.BUILDER:
				Char_SelectChar.classNo = 2;
				break;
			default:
				break;
		}
	}
}
