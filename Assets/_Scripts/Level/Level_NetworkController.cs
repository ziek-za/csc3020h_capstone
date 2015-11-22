using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfo {
	public PlayerInfo(string newName){//}, int newRespID){
		name = newName;
		photonPlayerName = "";
		kills = 0;
		deaths = 0;
		team = Char_AttributeScript.Teams.NONE;
		ping = 0;
		//respawnerID = newRespID;
	}
	public void increaseKills(){
		kills++;
	}
	public void increaseDeaths(){
		deaths++;
	}
	public string name, photonPlayerName;
	public int ping, kills, deaths;
	public Char_AttributeScript.Teams team;
	//public int respawnerID;
}

public class Level_NetworkController : Photon.MonoBehaviour {

	public Transform mapController;
	public Transform respawner;

	public List<PlayerInfo> bluePlayers, redPlayers, neutPlayers;

	public static bool firstPlayer = false;
	public Level_GUIController GUIController;

	// Use this for initialization
	void Start () {
		//PhotonNetwork.ConnectUsingSettings("0.1");
		//QualitySettings.vSyncCount = 1;
		bluePlayers = new List<PlayerInfo>();
		redPlayers = new List<PlayerInfo>();
		if (neutPlayers == null) {
			neutPlayers = new List<PlayerInfo>();
		}
		JoinedRoom();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnPhotonPlayerConnected(PhotonPlayer other){
		PlayerInfo newPlayer = new PlayerInfo("blank_name");
		neutPlayers.Add(newPlayer);
	}

	void OnPhotonPlayerDisconnected(PhotonPlayer other){
		Debug.Log("Player Left " + other.ToString());
	}

	void JoinedRoom()
	{
		Debug.Log("Connected to Room");
		PhotonNetwork.Instantiate(respawner.name,Vector3.zero,Quaternion.identity,0);
		PlayerInfo newPlayer = new PlayerInfo(_MainController.playerName);
		neutPlayers.Add(newPlayer);

		Debug.Log("PhotonNetwork.playerList.Length "+PhotonNetwork.playerList.Length);

		if (neutPlayers.Count <= 1){
			for (int i = 0; i < PhotonNetwork.playerList.Length - 1; i++){
				newPlayer = new PlayerInfo("blank_name");
				neutPlayers.Add(newPlayer);
			}
		}

		//-1 so don't count us
		//for (int i = 0; i < PhotonNetwork.playerList.Length - 1; i++){
		//	newPlayer = new PlayerInfo("blank_name");
		//	neutPlayers.Add(newPlayer);
		//}
		
		Map_TerrainController tc = Terrain.activeTerrain.GetComponent<Map_TerrainController>();
		tc.SetTerrainHeightMap();
		tc.SetFreezeMap();
		tc.SetClampMap ();
		Debug.Log ("hm reset");

		if (firstPlayer){
			Debug.Log("Host Joined");
			firstPlayer = false;
			//mapController.GetComponent<Level_MapController>().SetLevelObjects(false);
			//tc.SetTerrainTexture (0, 0, tc.terrain.terrainData.alphamapWidth, tc.terrain.terrainData.alphamapHeight);
		}			
	}
}
