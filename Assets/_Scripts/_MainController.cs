using UnityEngine;
using System.Collections;

public class _MainController{

	public static bool RoomJoined;

	public static string ExampleLookupMethod(){
		return "some info";
	}

	public static void CreateServer(){
		Application.LoadLevel("Level");
		if (PhotonNetwork.room == null){
			PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString("N"), true, true, 5);
		}
	}

	public static void JoinServer(string serverName){
		Application.LoadLevel("Level");
		PhotonNetwork.JoinRoom(serverName);
	}

}
