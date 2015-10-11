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

	// Use this for initialization
	void Start () {
		// Load string lookup object from MainController
		_MainController.ImportStringLookup ();
		listOfLANButtons = new List<LANgameInfo>();
		listOfOnlineButtons = new List<OnlineGameInfo>();
	}

	bool HostButtonClicked = false, JoinGameButtonClicked = false, DirectJoinButtonClicked = false;

	void OnJoinedLobby(){
		joinedLobby = true;
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log(PhotonNetwork.connectionStateDetailed.ToString());

		if (HostButtonClicked || JoinGameButtonClicked){
			loadingScreen.SetActive(true);
		}

		//For Host
		if (PhotonNetwork.connected && HostButtonClicked && joinedLobby){
			HostButtonClicked = false;	
			_MainController.CreateServer();
		}

		//Creating list of joinable games for LAN Games
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
			JoinGameButtonClicked = false;
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
		JoinGameButtonClicked = true;
	}

	private void SetMenuButtonsActive(bool x){
		InternetButton.gameObject.SetActive(x);
		LANButton.gameObject.SetActive(x);
		ExitButton.gameObject.SetActive(x);
		JoinGamePanel.gameObject.SetActive(x);
		NetworkScreen.SetActive(!x);
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
}
