using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_GUIController : MonoBehaviour {

	public InputField playerNameInput;
	public InputField IPaddressInput;

	public Button HostButton;
	public Button JoinButton;

	public Menu_NetworkController NetworkController;

	// Use this for initialization
	void Start () {
		// Load string lookup object from MainController

		_MainController.ImportStringLookup ();

	}

	bool madeRoom = false, hostButtonClicked = false;

	// Update is called once per frame
	void Update () {
		if (PhotonNetwork.connected && !madeRoom && hostButtonClicked){
			madeRoom = true;
			Debug.LogError("Room hosted");
			if (!playerNameInput.text.Equals(""))
				_MainController.playerName = playerNameInput.text;
			else
				_MainController.playerName = "default";
			
			_MainController.CreateServer();
		}
	}

	public void EnterIpAddress(){
		_MainController.hostIP = IPaddressInput.text;
		//NetworkController.TryConnect();
		//Debug.Log(IPaddressInput.text);
	}

	public void HostGameButtonClick(){
		NetworkController.TryConnect(Network.player.ipAddress);
		hostButtonClicked = true;
	}

	public void JoinGameButtonClick(){
		if (!playerNameInput.text.Equals(""))
			_MainController.playerName = playerNameInput.text;
		else
			_MainController.playerName = "default";

		_MainController.RoomJoined = true;
		StartCoroutine(NetworkController.CheckLocalForPhoton());
	}
}
