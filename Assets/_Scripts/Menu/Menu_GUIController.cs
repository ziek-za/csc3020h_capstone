using UnityEngine;
using System.Collections;

public class Menu_GUIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Load string lookup object from MainController
		_MainController.ImportStringLookup ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void HostGameButtonClick(){
		_MainController.CreateServer();
	}

	public void JoinGameButtonClick(){
		_MainController.RoomJoined = true;
	}
}
