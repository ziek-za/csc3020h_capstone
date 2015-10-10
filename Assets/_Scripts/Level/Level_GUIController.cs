using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Level_GUIController : MonoBehaviour {

	public Level_NetworkController NetworkController;
	public Char_AttributeScript.Teams localPlayerTeam = Char_AttributeScript.Teams.NONE;

	public Text healthText, energyText, respawnTimerText;
	public enum classes {SOLDIER, THIEF, BUILDER, NONE};
	public classes GUIClass = classes.NONE;
	public RectTransform scoreboard;
	public Slider bluePoints;
	public Slider redPoints;
	private float decTimer = 0f;

	public Camera spawnPreviewCamera;
	public Transform spawnPreviewSoldierCameraPos, spawnPreviewSniperCameraPos, spawnPreviewBuilderCameraPos;

	public Text redCountText;
	public Text blueCountText;

	public GameObject shotIndicatorPivot, HUDPivot,
		// SCOREBOARD
			playersRed, playersBlue,
			scoreBoardPlayerItem;

		//GAME OVER MESSAGE
	public GameObject gameOverPanel, winnerText;

	public Image builderImage, thiefImage, soldierImage, shotIndicator,
		// SOLDIER
				soldierHUD, vortexIcon, explosionIcon,
		// THIEF
				thiefHUD,
		// BUILDER
				builderHUD,
		// WEAPONS
				shotgun, pistol, sniper, rocketLauncher, glove;
	public HUD_AbilityIcon
		// SOLDIER
				vortexAndExplosionIcon,
		// THIEF
				teleportIcon, sprintIcon,
		// BUILDER
				linkIcon, turretIcon, boosterIcon;
	public Button linkButton;
	public Text playerNameLabel, playerKilledLabel, gameStatsText;
	bool internetGame = false;

	//Boolean for when the player is dead
	public bool isDeadBool = false;

	public GameObject mapCenter;

	// Use this for initialization
	void Start () {
		SetUpLinkButtons();
		if (_MainController.gameStats.Equals("ping")){
			internetGame = true;
		} else {
			internetGame = false;
			gameStatsText.text = "IP: " + _MainController.gameStats;
		}
	}

	public void SetUpLinkButtons(){
		redCountText.text = "0";
		blueCountText.text = "0";
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

			//Set up text and colour
			//tempButton.GetComponentInChildren<Text>().text = "Link " + i;
			if (linkTeam == Char_AttributeScript.Teams.BLUE) {
				ColorBlock blueColors = new ColorBlock();
				blueColors.normalColor = Color.blue;
				blueColors.highlightedColor = Color.blue;
				blueColors.pressedColor = Color.cyan;
				blueColors.disabledColor = new Color(0f,0f,0.5f);
				blueColors.colorMultiplier = 1;
				tempButton.GetComponent<Button>().colors = blueColors;

				if (localPlayerTeam == Char_AttributeScript.Teams.BLUE)
					tempButton.interactable = true;
				else
					tempButton.interactable = false;

				if (!links[i].GetComponent<Ability_BuilderLink>())
					blueCountText.text = (int.Parse(blueCountText.text) + 1).ToString();

			} else if (linkTeam == Char_AttributeScript.Teams.RED){
				ColorBlock redColors = new ColorBlock();
				redColors.normalColor = Color.red;
				redColors.highlightedColor = Color.red;
				redColors.pressedColor = new Color(0.6f,0f,0f);
				redColors.disabledColor = new Color(0.5f,0f,0f);
				redColors.colorMultiplier = 1;
				tempButton.GetComponent<Button>().colors = redColors;

				if (localPlayerTeam == Char_AttributeScript.Teams.RED )
					tempButton.interactable = true;
				else
					tempButton.interactable = false;

				if (!links[i].GetComponent<Ability_BuilderLink>())
					redCountText.text = (int.Parse(redCountText.text) + 1).ToString();

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

	public void SetUpScoreboard(){
		//Clear existing text
		foreach (Transform child in playersBlue.transform) {
			GameObject.Destroy(child.gameObject);
		} 

		foreach (Transform child in playersRed.transform) {
			GameObject.Destroy(child.gameObject);
		}

		int yShift = 0;

		NetworkController.redPlayers.Sort((a,b) => -1*a.kills.CompareTo(b.kills));
		for (int i = 0; i < NetworkController.redPlayers.Count; i++){
			GameObject tempLabel = Instantiate(scoreBoardPlayerItem,Vector3.zero, Quaternion.identity) as GameObject;
			tempLabel.transform.SetParent(playersRed.transform);
			Vector3 labelPos = new Vector3(0,yShift,0);
			yShift -= 34;
			tempLabel.transform.localPosition = labelPos;
			tempLabel.transform.localScale = new Vector3(1,1,1);
			tempLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(0,32);
			tempLabel.transform.FindChild("Name").GetComponent<Text>().text = NetworkController.redPlayers[i].name;
			tempLabel.transform.FindChild("K").GetComponent<Text>().text = NetworkController.redPlayers[i].kills.ToString();
			tempLabel.transform.FindChild("D").GetComponent<Text>().text = NetworkController.redPlayers[i].deaths.ToString();
			tempLabel.transform.FindChild("Ping").GetComponent<Text>().text = "";
		}

		yShift = 0;

		NetworkController.bluePlayers.Sort((a,b) => -1*a.kills.CompareTo(b.kills));
		for (int i = 0; i < NetworkController.bluePlayers.Count; i++){
			GameObject tempLabel = Instantiate(scoreBoardPlayerItem,Vector3.zero, Quaternion.identity) as GameObject;
			tempLabel.transform.SetParent(playersBlue.transform);
			Vector3 labelPos = new Vector3(0,yShift,0);
			yShift -= 34;
			tempLabel.transform.localPosition = labelPos;
			tempLabel.transform.localScale = new Vector3(1,1,1);
			tempLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(0,32);
			tempLabel.transform.FindChild("Name").GetComponent<Text>().text = NetworkController.bluePlayers[i].name;
			tempLabel.transform.FindChild("K").GetComponent<Text>().text = NetworkController.bluePlayers[i].kills.ToString();
			tempLabel.transform.FindChild("D").GetComponent<Text>().text = NetworkController.bluePlayers[i].deaths.ToString();
			tempLabel.transform.FindChild("Ping").GetComponent<Text>().text = "";
		}

	}

	// Update is called once per frame
	void Update () {

		if (internetGame){
			gameStatsText.text = "Ping: " + PhotonNetwork.GetPing().ToString();
		}

		//Lose 1 point per second per the difference between the teams # of links
		if (int.Parse(redCountText.text) > int.Parse(blueCountText.text)){
			decTimer += Time.deltaTime;
			if (decTimer > 1){
				bluePoints.value = bluePoints.value - (int.Parse(redCountText.text) - int.Parse(blueCountText.text));
				decTimer -= 1;
			}
		} else if (int.Parse(redCountText.text) < int.Parse(blueCountText.text)){
			decTimer += Time.deltaTime;
			if (decTimer > 1){
				redPoints.value = redPoints.value - (int.Parse(blueCountText.text) - int.Parse(redCountText.text));
				decTimer -= 1;
			}
		}
		// Show scoreboard
		if (Input.GetKey(KeyCode.Tab)) {
			scoreboard.anchoredPosition = new Vector2(0.0f,
			                                  0.0f);
		} else {
			scoreboard.anchoredPosition = new Vector2(-245.0f,
			                                          0.0f);
		}
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

	// Used to set the weapon icons
	public void SetWeaponIcon(Char_AttributeScript.Class player_class, int weapon_slot) {
		// Set all weapons to false
		pistol.gameObject.SetActive (false);
		shotgun.gameObject.SetActive (false);
		sniper.gameObject.SetActive (false);
		rocketLauncher.gameObject.SetActive (false);
		glove.gameObject.SetActive (false);
		// Activate the desired weapon
		switch (weapon_slot) {
			// PRIMARY
			case 1:
				pistol.gameObject.SetActive(true);
				break;
			// SECONDARY
			case 2:
				if (player_class == Char_AttributeScript.Class.SOLDIER) {
					rocketLauncher.gameObject.SetActive(true);
				} else if (player_class == Char_AttributeScript.Class.THIEF) {
					sniper.gameObject.SetActive(true);
				} else if (player_class == Char_AttributeScript.Class.BUILDER) {
					shotgun.gameObject.SetActive(true);
				}
				break;
			// TERTIARY
			case 3:
				if (player_class == Char_AttributeScript.Class.BUILDER) {
					glove.gameObject.SetActive(true);
				}
				break;
		}
	}

	public void onSoldierSelectButtonPress(){
		//Char_SelectChar.classNo = 0;
		soldierImage.GetComponent<Image>().enabled = true;
		builderImage.GetComponent<Image>().enabled = false;
		thiefImage.GetComponent<Image>().enabled = false;
		GUIClass = classes.SOLDIER;
		spawnPreviewCamera.gameObject.SetActive(true);
		spawnPreviewCamera.transform.localPosition = spawnPreviewSoldierCameraPos.localPosition;
		Debug.Log ("Selecting Soldier");
	}

	public void onThiefSelectButtonPress(){
		//Char_SelectChar.classNo = 1;
		soldierImage.GetComponent<Image>().enabled = false;
		builderImage.GetComponent<Image>().enabled = false;
		thiefImage.GetComponent<Image>().enabled = true;
		GUIClass = classes.THIEF;
		spawnPreviewCamera.gameObject.SetActive(true);
		spawnPreviewCamera.transform.localPosition = spawnPreviewSniperCameraPos.localPosition;
		Debug.Log ("Selecting Thief");
	}

	public void onBuilderSelectButtonPress(){
		//Char_SelectChar.classNo = 2;
		soldierImage.GetComponent<Image>().enabled = false;
		builderImage.GetComponent<Image>().enabled = true;
		thiefImage.GetComponent<Image>().enabled = false;
		GUIClass = classes.BUILDER;
		spawnPreviewCamera.gameObject.SetActive(true);
		spawnPreviewCamera.transform.localPosition = spawnPreviewBuilderCameraPos.localPosition;
		Debug.Log ("Selecting Builder");
	}

	public void onRedTeamButtonPress(){
		localPlayerTeam = Char_AttributeScript.Teams.RED;
		spawnPreviewCamera.gameObject.SetActive(true);
		GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(10,5,5);
		GameObject.Find("PickATeam").gameObject.SetActive(false);
		SetUpLinkButtons();
	}

	public void onBlueTeamButtonPress(){
		localPlayerTeam = Char_AttributeScript.Teams.BLUE;
		GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(10,5,5);
		spawnPreviewCamera.gameObject.SetActive(true);
		GameObject.Find("PickATeam").gameObject.SetActive(false);
		SetUpLinkButtons();
	}

	public void onReturnToMenuButtonPress(){
		Application.LoadLevel("Menu");
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
