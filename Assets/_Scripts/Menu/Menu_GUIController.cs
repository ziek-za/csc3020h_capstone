﻿using UnityEngine;
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
	
	// Update is called once per frame
	void Update () {
	
	}

	public void EnterIpAddress(){
		_MainController.hostIP = IPaddressInput.text;
		NetworkController.TryConnect();
		//Debug.Log(IPaddressInput.text);
	}

	public void HostGameButtonClick(){
		if (!playerNameInput.text.Equals(""))
			_MainController.playerName = playerNameInput.text;
		else
			_MainController.playerName = "default";

		_MainController.CreateServer();
	}

	public void JoinGameButtonClick(){
		if (!playerNameInput.text.Equals(""))
			_MainController.playerName = playerNameInput.text;
		else
			_MainController.playerName = "default";

		_MainController.RoomJoined = true;
	}
}
