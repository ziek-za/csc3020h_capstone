using UnityEngine;
using System.Collections;
using System.Net.Sockets;

public class Menu_NetworkController : MonoBehaviour {

	private RoomInfo[] roomsList;
	bool connectedToIP = false;

	public Menu_GUIController GUIController;

	// Use this for initialization
	void Start () {
		//PhotonNetwork.PhotonServerSettings.ServerAddress = "192.168.1.107"; 
		//PhotonNetwork.PhotonServerSettings.ServerAddress = "0.0.0.0"; 
		//PhotonNetwork.ConnectUsingSettings("0.1");
		//QualitySettings.vSyncCount = 1;

		//PhotonNetwork.networkingPeer.ServerAddress = "0.0.0.0:5055";
		//Debug.LogError(PhotonNetwork.ServerAddress);
		_MainController.hostIP = "0.0.0.0";
		TryConnect();
		//InvokeRepeating("TryConnect",0,5);
	}

	public void TryConnect(){
		try {
			PhotonNetwork.PhotonServerSettings.ServerAddress = _MainController.hostIP; 
			PhotonNetwork.ConnectUsingSettings("0.1");
			QualitySettings.vSyncCount = 1;
			//InvokeRepeating("TryConnect",5,5);
		} catch (SocketException e){
			Debug.Log ("here");
		}
	}
	
	// Update is called once per frame
	void Update () {
		//if (connectedToIP){
			if (PhotonNetwork.room == null)
			{
				if (roomsList != null){
						for (int i = 0; i < roomsList.Length; i++)
						{
							if (_MainController.RoomJoined == true){
								Application.LoadLevel("Level");
								PhotonNetwork.JoinRoom(roomsList[i].name);
								_MainController.ImportMapObject("2");
								_MainController.RoomJoined = false;
							}
						}
					}
			}
		//}
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
		GUIController.HostButton.interactable = true;
		GUIController.JoinButton.interactable = true;
		//CancelInvoke("TryConnect");
	}
}
