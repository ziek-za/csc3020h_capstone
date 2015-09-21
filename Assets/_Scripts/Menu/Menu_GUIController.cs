using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Menu_GUIController : Photon.MonoBehaviour {

	public InputField playerNameInput;
	public InputField gameNameInput;

	public Button HostButton;
	public Button JoinButton;
	public Button LANButtonPrefab;
	public Transform ButtonPanel;
	public GameObject JoinLANScreen, loadingScreen;

	public Menu_NetworkController NetworkController;
	public List<LANgameInfo> listOfButtons;
	int buttonCount = 0, buttonYPos = 90;
	string serverToJoin = "", ipToJoin = "";

	// Use this for initialization
	void Start () {
		// Load string lookup object from MainController
		_MainController.ImportStringLookup ();
		listOfButtons = new List<LANgameInfo>();

	}

	bool hostButtonClicked = false, LANgameButtonClicked = false;

	// Update is called once per frame
	void Update () {
		//For Host
		if (PhotonNetwork.connected && hostButtonClicked){
			//Debug.LogError("Room hosted");
			hostButtonClicked = false;
			if (!playerNameInput.text.Equals(""))
				_MainController.playerName = playerNameInput.text;
			else
				_MainController.playerName = "default";

			if (!gameNameInput.text.Equals(""))
				_MainController.gameName = gameNameInput.text;
			else
				_MainController.gameName = "new game";
			
			_MainController.CreateServer();
		}

		//Creating list of joinable games
		if (listOfButtons.Count > buttonCount){
			//Debug.Log(listOfButtons[buttonCount].gameName);
			//Set up postion on screen and in heirarcy
			Vector3 buttonPos = new Vector3(0,buttonYPos,0);
			buttonYPos -= 35;
			Button tempButton = Instantiate(LANButtonPrefab,Vector3.zero, Quaternion.identity) as Button;
			tempButton.transform.SetParent(ButtonPanel);
			tempButton.transform.localPosition = buttonPos;
			tempButton.transform.localScale = new Vector3(1,1,1);
			tempButton.GetComponentInChildren<Text>().text = listOfButtons[buttonCount].ToString();
			//Add action listener
			int tempCount = buttonCount;
			tempButton.onClick.AddListener(() => OnLANGameClick(listOfButtons[tempCount].ip,listOfButtons[tempCount].serverName));

			buttonCount++;
		}

		//For joining a game
		if (PhotonNetwork.connected && LANgameButtonClicked){
			//Debug.Log(PhotonNetwork.connectionState + " " + PhotonNetwork.connectedAndReady);
			//Debug.LogError("Room Joined");
			_MainController.JoinServer(serverToJoin);
			LANgameButtonClicked = false;
		}
	}

	void OnConnectedToMaster () {
		Debug.Log ("master replies");
	}

	void OnLANGameClick(string ip, string serverName){
		serverToJoin = serverName;
		ipToJoin = ip;
		StopCoroutine(NetworkController.CheckLocalForPhoton());
		PhotonNetwork.Disconnect();
		//PhotonNetwork.networkingPeer.Disconnect();
		//Destroy(PhotonNetwork.networkingPeer);
		NetworkController.TryConnect(ipToJoin);
		NetworkController.buttonClicked = true;
		LANgameButtonClicked = true;
		loadingScreen.SetActive(true);
	}

	public void HostGameButtonClick(){
		NetworkController.TryConnect(Network.player.ipAddress);
		hostButtonClicked = true;
		JoinButton.interactable = false;
		HostButton.interactable = false;
		loadingScreen.SetActive(true);
	}

	public void JoinGameButtonClick(){
		if (!playerNameInput.text.Equals(""))
			_MainController.playerName = playerNameInput.text;
		else
			_MainController.playerName = "default";

		if (!gameNameInput.text.Equals(""))
			_MainController.gameName = gameNameInput.text;
		else
			_MainController.gameName = "new game";

		JoinButton.interactable = false;
		HostButton.interactable = false;
		StartCoroutine(NetworkController.CheckLocalForPhoton());
		JoinLANScreen.SetActive(true);
	}
}
