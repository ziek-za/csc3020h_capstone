using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Char_SelectChar : Photon.MonoBehaviour {

	public GameObject thiefPrefab, builderPrefab, soldierPrefab;
	public bool spawned=false;
	public static int classNo=9;

	public static Char_AttributeScript.Teams currentTeam = Char_AttributeScript.Teams.NONE;
	public static Vector3 spawnLocation = Vector3.zero;

	private Level_GUIController gc;
	private Level_NetworkController nc;
	// Use this for initialization
	void Start () {
		// initialize the GUI Controller object
		gc = GameObject.Find ("GUI Controller").GetComponent<Level_GUIController> ();
		// initialize the Network Controller object
		nc = GameObject.Find ("Network Controller").GetComponent<Level_NetworkController> ();
		classNo=9;
	}
	
	// Update is called once per frame
	void Update () {

		//End game - blue loses
		if (gc.bluePoints.value <= 0){
			EndGameRPC((int)Char_AttributeScript.Teams.RED);
		}

		//End game - red loses
		if (gc.redPoints.value <= 0){
			EndGameRPC((int)Char_AttributeScript.Teams.BLUE);
		}

		if(photonView.isMine){
			//if (Input.GetKeyDown(KeyCode.Escape)){
			//	Application.LoadLevel("Menu");
			//}
			if(!spawned){
				// Disable sniper scope regardless
				Camera.main.fieldOfView = 60;
				gc.sniperScope.enabled = false;
				// SOLDIER
				if(classNo==0){
					spawned=true;
					GameObject sol = PhotonNetwork.Instantiate(soldierPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					sol.GetComponent<Char_AttributeScript>().Respawner = this;
					sol.GetComponent<Char_AttributeScript>().team = currentTeam;
					// turn on the solider ability HUD
					gc.soldierHUD.gameObject.SetActive(true);
					gc.thiefHUD.gameObject.SetActive(false);
					gc.builderHUD.gameObject.SetActive(false);

					// Set the current class in the char attribute script
					sol.GetComponent<Char_AttributeScript>().current_class = Char_AttributeScript.Class.SOLDIER;
				}
				// THIEF
				else if(classNo==1){
					spawned=true;
					GameObject thief = PhotonNetwork.Instantiate(thiefPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					thief.GetComponent<Char_AttributeScript>().Respawner = this;
					thief.GetComponent<Char_AttributeScript>().team = currentTeam;
					// turn on the theif ability HUD
					gc.soldierHUD.gameObject.SetActive(false);
					gc.thiefHUD.gameObject.SetActive(true);
					gc.builderHUD.gameObject.SetActive(false);

					// Set the current class in the char attribute script
					thief.GetComponent<Char_AttributeScript>().current_class = Char_AttributeScript.Class.THIEF;
				}
				// BUILDER
				else if(classNo==2){
					spawned=true;
					GameObject bul = PhotonNetwork.Instantiate(builderPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					bul.GetComponent<Char_AttributeScript>().Respawner = this;
					bul.GetComponent<Char_AttributeScript>().team = currentTeam;
					// turn on the builder ability HUD
					gc.soldierHUD.gameObject.SetActive(false);
					gc.thiefHUD.gameObject.SetActive(false);
					gc.builderHUD.gameObject.SetActive(true);


					// Set the current class in the char attribute script
					bul.GetComponent<Char_AttributeScript>().current_class = Char_AttributeScript.Class.BUILDER;
				}
				if(spawned){
					Screen.lockCursor=true;
					//if (Camera.main.GetComponent<BlurEffect>())
						//Debug.Log("blur here");
					Camera.main.GetComponent<BlurEffect>().enabled=false;
					GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(0,0,0);

					//Turn off second camera
					gc.spawnPreviewCamera.gameObject.SetActive(false);

					// Turn the HUD on
					gc.HUDPivot.SetActive(true);
					gc.isDeadBool = false;
					// Set active weapon
					gc.SetWeaponIcon(0, 1);

					//Send RPC telling other clients to move us in their Lists of players
					MovePlayerTeamList(_MainController.playerName, (int) currentTeam);

				} else {
					// Turn the HUD off
					gc.isDeadBool = true;
					gc.ResetIconsCooldown();
					gc.HUDPivot.SetActive(false);
				}
			}
		}
	}

	[RPC] void EndGameRPC(int winningTeam){

		gc.gameOverPanel.SetActive(true);
		GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(0,0,0);
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < players.Length; i++){
			Destroy(players[i]);
		}
		gc.scoreboard.anchoredPosition = new Vector2(0.0f,0.0f);
		Screen.lockCursor = false;

		if ((Char_AttributeScript.Teams)winningTeam == Char_AttributeScript.Teams.BLUE){
			gc.redPoints.value = 0;
			gc.winnerText.GetComponent<Text>().text = "BLUE TEAM WINS";
			gc.winnerText.GetComponent<Text>().color = Color.blue;
		} else {
			gc.bluePoints.value = 0;
			gc.winnerText.GetComponent<Text>().text = "RED TEAM WINS";
			gc.winnerText.GetComponent<Text>().color = Color.red;
		}

		if (photonView.isMine)
			photonView.RPC("EndGameRPC", PhotonTargets.OthersBuffered, winningTeam);
	}

	[RPC] void MovePlayerTeamList(string pName, int newTeam){

		nc = GameObject.Find ("Network Controller").GetComponent<Level_NetworkController> ();
		gc = GameObject.Find ("GUI Controller").GetComponent<Level_GUIController> ();
		//If we join after other players
		if (nc.neutPlayers == null) {
			nc.neutPlayers = new List<PlayerInfo>();
			for (int i = 0; i < PhotonNetwork.playerList.Length - 1; i++){
				PlayerInfo newPlayer = new PlayerInfo("blank_name");
				nc.neutPlayers.Add(newPlayer);
			}
		}

		Debug.Log(pName + " " + newTeam.ToString());
		for (int i = 0; i < nc.neutPlayers.Count; i++){
			if (nc.neutPlayers[i].name == _MainController.playerName || nc.neutPlayers[i].name == "blank_name"){
				PlayerInfo pi = nc.neutPlayers[i];
				nc.neutPlayers.Remove(pi);
				pi.team = (Char_AttributeScript.Teams)newTeam;
				pi.name = pName;
				//pi.photonPlayerName = pName;
				if (pi.team == Char_AttributeScript.Teams.BLUE)
					nc.bluePlayers.Add(pi);
				else
					nc.redPlayers.Add(pi);
				break;
			}
		}

		gc.SetUpScoreboard();
		
		if (photonView.isMine)
			photonView.RPC("MovePlayerTeamList", PhotonTargets.OthersBuffered, pName, newTeam);
	}


}
