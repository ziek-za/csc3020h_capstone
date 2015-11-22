using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Menu_GUIController : Photon.MonoBehaviour {

	public InputField playerNameInput, gameNameInput, hostIpInput;
	public Button InternetButton, LANButton, ExitButton;
	public Text GameListHeadings;
	public Transform JoinGamePanel;

	public Button LANButtonPrefab;
	public Transform ButtonPanel;
	public GameObject NetworkScreen, loadingScreen;

	public Menu_NetworkController NetworkController;
	public List<LANgameInfo> listOfLANButtons;
	public List<OnlineGameInfo> listOfOnlineButtons;
	int buttonCount = 0, buttonYPos = 95;
	public string serverToJoin = "";
	public AudioClip button_click;

	public bool internetGame = false; //Used when deciding whether to show the LAN/Online UI

	const string alphaNum = "abcdefghijklmnopqrstuvwxyz0123456789";

	//Used for cloud games
	bool joinedLobby = false;

	//Lobby screen
	public GameObject lobbyScreen, redPlayerLabelPrefab, bluePlayerLabelPrefab, redTeamPanel, blueTeamPanel, localPlayerPrefab;
	public Transform hostPanel;
	public Text lobbyGameName, lobbyGameDetails;
	bool inLobbyScreen = false;
	public Menu_PlayerPhotonView localPlayer;

	// Use this for initialization
	void Start () {
		// Load string lookup object from MainController
		_MainController.ImportStringLookup ();
		if (_MainController.playerName != "")
			playerNameInput.text = _MainController.playerName;
		listOfLANButtons = new List<LANgameInfo>();
		listOfOnlineButtons = new List<OnlineGameInfo>();
		inLobbyScreen = false;
	}

	bool HostButtonClicked = false, JoinGameButtonClicked = false, DirectJoinButtonClicked = false;

	void OnJoinedLobby(){
		joinedLobby = true;
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log(PhotonNetwork.connectionStateDetailed.ToString());}

		//For Host
		if (PhotonNetwork.connected && HostButtonClicked && joinedLobby){
			HostButtonClicked = false;	
			_MainController.CreateServer();
			NetworkScreen.GetComponent<Animator>().Play("MoveMenuUp");
			NetworkScreen.transform.localPosition = new Vector3(0,500,0);
			Invoke("SetUpLobbyScreen",2f);
		}

		//Creating list of joinable games
		if ((!internetGame && listOfLANButtons.Count > buttonCount) || (internetGame && listOfOnlineButtons.Count > buttonCount)){

			//Set up postion on screen and in heirarcy
			Vector3 buttonPos = new Vector3(0,buttonYPos,0);
			buttonYPos -= 35;
			Button tempButton = Instantiate(LANButtonPrefab,Vector3.zero, Quaternion.identity) as Button;
			tempButton.transform.SetParent(ButtonPanel);
			tempButton.transform.localPosition = buttonPos;
			tempButton.transform.localScale = new Vector3(1,1,1);

			//Add action listener
			int tempCount = buttonCount;
			if (internetGame) {
				tempButton.GetComponentInChildren<Text>().text = listOfOnlineButtons[buttonCount].ToString();
				tempButton.onClick.AddListener(() => OnOnlineGameClick(listOfOnlineButtons[tempCount].serverName));
			} else {
				tempButton.GetComponentInChildren<Text>().text = listOfLANButtons[buttonCount].ToString();
				tempButton.onClick.AddListener(() => OnLANGameClick(listOfLANButtons[tempCount].ip,listOfLANButtons[tempCount].serverName));
			}

			buttonCount++;
		}

		//For joining a game
		if (PhotonNetwork.connected && JoinGameButtonClicked && joinedLobby){
			_MainController.JoinServer(serverToJoin);
			NetworkScreen.GetComponent<Animator>().Play("MoveMenuUp");
			NetworkScreen.transform.localPosition = new Vector3(0,500,0);
			JoinGameButtonClicked = false;
			Invoke("SetUpLobbyScreen",2f);
		}

		//For Direct IP connection
		if (PhotonNetwork.connected && DirectJoinButtonClicked && joinedLobby){
			loadingScreen.SetActive(true);
			try {
				_MainController.JoinServer(NetworkController.roomsList[0].name);
				DirectJoinButtonClicked = false;
			} catch (System.Exception e){Debug.Log(NetworkController.roomsList.Length);}
		}
	}

	void OnLANGameClick(string ip, string serverName){
		AudioSource.PlayClipAtPoint(button_click,GameObject.Find("Main Camera").transform.position);
		loadingScreen.SetActive(true);

		if (!playerNameInput.text.Equals(""))
			_MainController.playerName = playerNameInput.text;
		else {
			_MainController.playerName = "default_";
			for(int i = 0; i < 4; i++)
			{
				_MainController.playerName += alphaNum[Random.Range(0, alphaNum.Length)];
			}
		}

		serverToJoin = serverName;
		StopCoroutine(NetworkController.CheckLocalForPhoton());
		PhotonNetwork.Disconnect();
		NetworkController.TryConnect(ip);
		NetworkController.buttonClicked = true;
		JoinGameButtonClicked = true;
	}

	void OnOnlineGameClick(string serverName){
		AudioSource.PlayClipAtPoint(button_click,GameObject.Find("Main Camera").transform.position);
		//loadingScreen.SetActive(true);

		if (!playerNameInput.text.Equals(""))
			_MainController.playerName = playerNameInput.text;
		else {
			_MainController.playerName = "default_";
			for(int i = 0; i < 4; i++)
			{
				_MainController.playerName += alphaNum[Random.Range(0, alphaNum.Length)];
			}
		}
		serverToJoin = serverName;
		JoinGameButtonClicked = true;
	}

	private void SetMenuButtonsActive(bool x){
		InternetButton.gameObject.SetActive(x);
		LANButton.gameObject.SetActive(x);
		ExitButton.gameObject.SetActive(x);
		JoinGamePanel.gameObject.SetActive(x);
		NetworkScreen.SetActive(true);
		if (!x){
			NetworkScreen.GetComponent<Animator>().Play("MoveMenuDown");
			NetworkScreen.transform.localPosition = Vector3.zero;
		} else {
			NetworkScreen.GetComponent<Animator>().Play("MoveMenuUp");
			NetworkScreen.transform.localPosition = new Vector3(0,500,0);
		}
	}

	private void TransitionLobbyScreen(bool x){
		InternetButton.gameObject.SetActive(x);
		LANButton.gameObject.SetActive(x);
		ExitButton.gameObject.SetActive(x);
		lobbyScreen.SetActive(true);
		if (!x){
			lobbyScreen.GetComponent<Animator>().Play("MoveMenuDown");
			lobbyScreen.transform.localPosition = Vector3.zero;
		} else {
			lobbyScreen.GetComponent<Animator>().Play("MoveMenuUp");
			lobbyScreen.transform.localPosition = new Vector3(0,500,0);
		}
	}

	public void InternetGameButtonClick(){
		AudioSource.PlayClipAtPoint(button_click,GameObject.Find("Main Camera").transform.position);
		SetMenuButtonsActive(false);
		GameListHeadings.text = "	Name						Host						Players						Ping						Region";
		internetGame = true;
		PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.BestRegion;
		NetworkController.TryConnect("-1");
		_MainController.gameStats = "ping";
	}

	public void LANGameButtonClick(){
		AudioSource.PlayClipAtPoint(button_click,GameObject.Find("Main Camera").transform.position);
		SetMenuButtonsActive(false);
		JoinGamePanel.gameObject.SetActive(true);
		GameListHeadings.text = "	Name						Host						Players						Ping						IP";
		internetGame = false;
		PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.SelfHosted;
		StartCoroutine(NetworkController.CheckLocalForPhoton());
		_MainController.gameStats = Network.player.ipAddress;
	}

	public void ExitButtonClick(){
		Application.Quit();
	}

	public void BackButtonClick(){
		AudioSource.PlayClipAtPoint(button_click,GameObject.Find("Main Camera").transform.position);
		SetMenuButtonsActive(true);
		buttonCount = 0;
		buttonYPos = 95;
		NetworkController.buttonClicked = false;
		listOfLANButtons.Clear();
		listOfOnlineButtons.Clear();

		//Clear existing buttons
		foreach (Transform child in ButtonPanel.transform) {
			GameObject.Destroy(child.gameObject);
		}

		try {
			PhotonNetwork.Disconnect();
		} catch (System.Exception e){};
	}

	public void HostGameButtonClick(){
		AudioSource.PlayClipAtPoint(button_click,GameObject.Find("Main Camera").transform.position);
		//loadingScreen.SetActive(true);

		if (!playerNameInput.text.Equals(""))
			_MainController.playerName = playerNameInput.text;
		else {
			_MainController.playerName = "default_";
			for(int i = 0; i < 4; i++)
			{
				_MainController.playerName += alphaNum[Random.Range(0, alphaNum.Length)];
			}
		}
		
		if (!gameNameInput.text.Equals(""))
			_MainController.gameName = gameNameInput.text;
		else {
			string gameName = "";
			for(int i = 0; i < 6; i++)
			{
				gameName += alphaNum[Random.Range(0, alphaNum.Length)];
			}
			_MainController.gameName = gameName;
		}
			
		if (!internetGame) {
			NetworkController.TryConnect(Network.player.ipAddress);
		}

		hostPanel.gameObject.SetActive(true);
		HostButtonClicked = true;

	}


	public void DirectConnectButtonClick(){
		AudioSource.PlayClipAtPoint(button_click,GameObject.Find("Main Camera").transform.position);
		if (!hostIpInput.text.Equals("")){

			if (!playerNameInput.text.Equals(""))
				_MainController.playerName = playerNameInput.text;
			else {
				_MainController.playerName = "default_";
				for(int i = 0; i < 4; i++)
				{
					_MainController.playerName += alphaNum[Random.Range(0, alphaNum.Length)];
				}
			}

			StopCoroutine(NetworkController.CheckLocalForPhoton());
			PhotonNetwork.Disconnect();
			NetworkController.buttonClicked = true;
			DirectJoinButtonClicked = true;
			NetworkController.TryConnect(hostIpInput.text);
		}
	}

	public void LobbyBackButtonClick(){

		localPlayer.LeaveGame(_MainController.playerName);

		try {
			PhotonNetwork.Disconnect();
		} catch {}

		NetworkController.redTeam.Clear();
		NetworkController.blueTeam.Clear();
		TransitionLobbyScreen(true);
	}

	private void SetUpLobbyScreen(){
		GameObject gm = PhotonNetwork.Instantiate("localPlayerPrefab",Vector3.zero,Quaternion.identity,0) as GameObject;
		localPlayer = gm.GetComponent<Menu_PlayerPhotonView>();
		localPlayer.ResetTeams();
		
		//Clear existing labels
		foreach (Transform child in redTeamPanel.transform) {
			GameObject.Destroy(child.gameObject);
		}

		//Clear existing labels
		foreach (Transform child in blueTeamPanel.transform) {
			GameObject.Destroy(child.gameObject);
		}

		lobbyGameName.text = _MainController.gameName;
		lobbyGameDetails.text = "Europe";
		TransitionLobbyScreen(false);
	}

	public void LobbyStartButtonClick(){
		bool allReady = true;

		for (int i = 0; i < NetworkController.blueTeam.Count; i++){
			if (!NetworkController.blueTeam[i].ready){
				allReady = false;
				break;
			}
		}
		for (int i = 0; i < NetworkController.redTeam.Count; i++){
			if (!NetworkController.redTeam[i].ready){
				allReady = false;
				break;
			}
		}

		if (allReady && (NetworkController.redTeam.Count > 0 || NetworkController.blueTeam.Count > 0)){
			PhotonNetwork.room.open = false;
			PhotonNetwork.room.visible = false;
			localPlayer.StartGame();
		}
	}

	public void LobbyBalanceButtonClick(){
		int diff = NetworkController.redTeam.Count - NetworkController.blueTeam.Count;

		if (diff > 1){
			while (diff > 1){
				int luckyPlayer = Random.Range(0,NetworkController.redTeam.Count-1);
				localPlayer.JoinBlueLobby(NetworkController.redTeam[luckyPlayer].playerName);
				diff--;
			}
		} else if (diff < -1) {
			while (diff < -1){
				int luckyPlayer = Random.Range(0,NetworkController.blueTeam.Count-1);
				localPlayer.JoinRedLobby(NetworkController.blueTeam[luckyPlayer].playerName);
				diff++;
			}
		}
	}

	public void LobbyJoinRedButtonClick(){
		localPlayer.JoinRedLobby(_MainController.playerName);
		_MainController.playerTeam = Char_AttributeScript.Teams.RED;
	}

	public void LobbyJoinBlueButtonClick(){
		localPlayer.JoinBlueLobby(_MainController.playerName);
		_MainController.playerTeam = Char_AttributeScript.Teams.BLUE;
	}

	public void ToggleReady(string playerName, bool value){
		localPlayer.ToggleReady(playerName, value);
	}

	public void LobbyRedrawTeams(){
		try {
			//Clear existing labels
			foreach (Transform child in redTeamPanel.transform) {
				GameObject.Destroy(child.gameObject);
			}

			float yValue = 85;
			for (int i = 0; i < NetworkController.redTeam.Count; i++){
				GameObject redLabel = Instantiate(redPlayerLabelPrefab,Vector3.zero, Quaternion.identity) as GameObject;
				redLabel.GetComponentInChildren<Text>().text = NetworkController.redTeam[i].playerName;

				redLabel.GetComponentInChildren<Toggle>().isOn = NetworkController.redTeam[i].ready;
				if (NetworkController.redTeam[i].playerName.Equals(_MainController.playerName))
					redLabel.GetComponentInChildren<Toggle>().interactable = true;
				else
					redLabel.GetComponentInChildren<Toggle>().interactable = false;
				string playerListenerName = NetworkController.redTeam[i].playerName;
				redLabel.GetComponentInChildren<Toggle>().onValueChanged.AddListener((value) => ToggleReady(playerListenerName, value));

				redLabel.transform.SetParent(redTeamPanel.transform);
				redLabel.transform.localPosition = new Vector3(0,yValue,0);
				yValue -= 30;
			}
		} catch (System.Exception e){
			Debug.LogError(e);
		}

		try {
			//Clear existing labels
			foreach (Transform child in blueTeamPanel.transform) {
				GameObject.Destroy(child.gameObject);
			}
			
			float yValue = 85;
			for (int i = 0; i < NetworkController.blueTeam.Count; i++){
				GameObject blueLabel = Instantiate(bluePlayerLabelPrefab,Vector3.zero, Quaternion.identity) as GameObject;
				blueLabel.GetComponentInChildren<Text>().text = NetworkController.blueTeam[i].playerName;

				blueLabel.GetComponentInChildren<Toggle>().isOn = NetworkController.blueTeam[i].ready;
				if (NetworkController.blueTeam[i].playerName.Equals(_MainController.playerName))
					blueLabel.GetComponentInChildren<Toggle>().interactable = true;
				else
					blueLabel.GetComponentInChildren<Toggle>().interactable = false;
				string playerListenerName = NetworkController.blueTeam[i].playerName;
				blueLabel.GetComponentInChildren<Toggle>().onValueChanged.AddListener((value) => ToggleReady(playerListenerName, value));

				blueLabel.transform.SetParent(blueTeamPanel.transform);
				blueLabel.transform.localPosition = new Vector3(0,yValue,0);
				yValue -= 30;
			}
		} catch (System.Exception e){
			Debug.LogError(e);
		}
	}

}
