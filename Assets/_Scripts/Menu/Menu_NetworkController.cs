using UnityEngine;
using System.Collections;

public class Menu_NetworkController : MonoBehaviour {

	private RoomInfo[] roomsList;

	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings("0.1");
		QualitySettings.vSyncCount = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (PhotonNetwork.room == null)
		{
			if (roomsList != null){
					for (int i = 0; i < roomsList.Length; i++)
					{
						if (_MainController.RoomJoined == true){
							Application.LoadLevel("Level");
							PhotonNetwork.JoinRoom(roomsList[i].name);
							//_MainController.ImportMapObject("2");
							_MainController.RoomJoined = false;
						}
					}
				}
		}
	}

	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
		Debug.Log(roomsList.Length);
	}
}
