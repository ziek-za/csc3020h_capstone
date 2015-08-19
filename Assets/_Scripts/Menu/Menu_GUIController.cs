using UnityEngine;
using System.Collections;

public class Menu_GUIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Load string lookup object from MainController

		_MainController.ImportStringLookup ();
		_MainController.playerName = generateName ();
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

	public string generateName(){
		int randomNum = Random.Range (0, 10);
		string randomName = randomNum.ToString ();
		return randomName;
		}
}
