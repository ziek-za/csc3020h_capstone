using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu_PlayerPhotonView : Photon.MonoBehaviour {

	public Menu_GUIController GUIController;
	public Menu_NetworkController NetworkController;

	// Use this for initialization
	void Start () {
		GUIController = GameObject.Find("GUI Controller").GetComponent<Menu_GUIController>();
		NetworkController = GameObject.Find("Network Controller").GetComponent<Menu_NetworkController>();
		GUIController.LobbyRedrawTeams();
	}

	[RPC] public void ToggleReady(string playerName, bool v){

		for (int i = 0; i < NetworkController.blueTeam.Count; i++){
			if (NetworkController.blueTeam[i].playerName.Equals(playerName)){
				NetworkController.blueTeam[i].FlipReadyValue(v);
			}
		}
		for (int i = 0; i < NetworkController.redTeam.Count; i++){
			if (NetworkController.redTeam[i].playerName.Equals(playerName)){
				NetworkController.redTeam[i].FlipReadyValue(v);
			}
		}

		GUIController = GameObject.Find("GUI Controller").GetComponent<Menu_GUIController>();
		GUIController.LobbyRedrawTeams();

		if(photonView.isMine){
			photonView.RPC("ToggleReady", PhotonTargets.OthersBuffered, playerName, v);
		}
	}

	[RPC] public void StartGame(){
		GUIController.loadingScreen.SetActive(true);
		GUIController.lobbyScreen.SetActive(false);
		if(photonView.isMine){
			photonView.RPC("StartGame", PhotonTargets.OthersBuffered);
		}
		_MainController.LoadLevel(GUIController.loadingScreen);
	}


	[RPC] public void JoinRedLobby(string playerName){
		try {
			JoinTeam(playerName, Char_AttributeScript.Teams.RED);
		} catch (System.NullReferenceException e){
			NetworkController.redTeam = new List<LobbyPlayer>();
			NetworkController.blueTeam = new List<LobbyPlayer>();
			JoinTeam(playerName, Char_AttributeScript.Teams.RED);
		}

		if(photonView.isMine){
			photonView.RPC("JoinRedLobby", PhotonTargets.OthersBuffered, playerName);
		}
	}

	[RPC] public void JoinBlueLobby(string playerName){
		try {
			JoinTeam(playerName, Char_AttributeScript.Teams.BLUE);
		} catch (System.NullReferenceException e){
			NetworkController.redTeam = new List<LobbyPlayer>();
			NetworkController.blueTeam = new List<LobbyPlayer>();
			JoinTeam(playerName, Char_AttributeScript.Teams.BLUE);
		} 

		if(photonView.isMine){
			photonView.RPC("JoinBlueLobby", PhotonTargets.OthersBuffered, playerName);
		}
	}

	void JoinTeam(string playerName, Char_AttributeScript.Teams teamToJoin) {
		bool alreadyInTeam = false;
		NetworkController = GameObject.Find("Network Controller").GetComponent<Menu_NetworkController>();
		Debug.Log (teamToJoin);

		if (teamToJoin == Char_AttributeScript.Teams.RED){
			for (int i = 0; i < NetworkController.blueTeam.Count; i++){
				if (NetworkController.blueTeam[i].playerName.Equals(playerName)){
					NetworkController.blueTeam.RemoveAt(i);
					break;
				}
			}
			for (int i = 0; i < NetworkController.redTeam.Count; i++){
				if (NetworkController.redTeam[i].playerName.Equals(playerName)){
					alreadyInTeam = true;
					break;
				}
			}
			if (!alreadyInTeam)
				NetworkController.redTeam.Add(new LobbyPlayer(false,playerName,teamToJoin));
		} else {
			for (int i = 0; i < NetworkController.redTeam.Count; i++){
				if (NetworkController.redTeam[i].playerName.Equals(playerName)){
					NetworkController.redTeam.RemoveAt(i);
					break;
				}
			}
			for (int i = 0; i < NetworkController.blueTeam.Count; i++){
				if (NetworkController.blueTeam[i].playerName.Equals(playerName)){
					alreadyInTeam = true;
					break;
				}
			}
			if (!alreadyInTeam)
				NetworkController.blueTeam.Add(new LobbyPlayer(false,playerName,teamToJoin));
		}

		GUIController = GameObject.Find("GUI Controller").GetComponent<Menu_GUIController>();
		GUIController.LobbyRedrawTeams();
	}

	[RPC] public void LeaveGame(string playerName){

		for (int i = 0; i < NetworkController.blueTeam.Count; i++){
			if (NetworkController.blueTeam[i].playerName.Equals(playerName)){
				NetworkController.blueTeam.RemoveAt(i);
				break;
			}
		}
		for (int i = 0; i < NetworkController.redTeam.Count; i++){
			if (NetworkController.redTeam[i].playerName.Equals(playerName)){
				NetworkController.redTeam.RemoveAt(i);
				break;
			}
		}

		GUIController = GameObject.Find("GUI Controller").GetComponent<Menu_GUIController>();
		GUIController.LobbyRedrawTeams();

		if(photonView.isMine){
			photonView.RPC("LeaveGame", PhotonTargets.OthersBuffered, playerName);
		}
	}

	public void ResetTeams(){
		if (photonView.isMine){
			NetworkController = GameObject.Find("Network Controller").GetComponent<Menu_NetworkController>();

			try {
				Debug.Log(NetworkController.redTeam.Count);
			} catch (System.NullReferenceException e){
				NetworkController.redTeam = new List<LobbyPlayer>();
				NetworkController.blueTeam = new List<LobbyPlayer>();
			}
			//NetworkController.redTeam = new List<LobbyPlayer>();
			//NetworkController.blueTeam = new List<LobbyPlayer>();
		}
	}
}
