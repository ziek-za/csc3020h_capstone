using UnityEngine;
using System.Collections;

public class Level_NetworkController : MonoBehaviour {

	public Transform mapController;
	public Transform respawner;

	public static bool firstPlayer = false;

	// Use this for initialization
	void Start () {
		//PhotonNetwork.ConnectUsingSettings("0.1");
		//QualitySettings.vSyncCount = 1;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnJoinedRoom()
	{
		// Spawn player
//		PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0);
		//GameObject.Find ("SamplePlayer");
		//Debug.Log (playerPrefab.transform);

		//Camera cam = transform.position (new Vector3 (1, 1, 1));

		//Camera.main.transform.Translate (1,10,5);
		//playerPrefab.AddComponent (Camera.main);
		//Camera.main.transform.Translate (1,10,0);
		//Camera.main.transform.LookAt (playerPrefab.transform);
		Debug.Log("Connected to Room");
		PhotonNetwork.Instantiate(respawner.name,Vector3.zero,Quaternion.identity,0);

		/*
		if (PhotonNetwork.countOfPlayers == 1){
				mapController.GetComponent<Level_MapController>().SetupLevel("1");
				//terrain.terrainData.SetHeights (0, 0, flatten_buf);
				//terrainController.GetComponent<Map_TerrainController>().SetTerrainHeightMap ();
		} else {
			Debug.Log("Not host");
		}*/
		
		Map_TerrainController tc = Terrain.activeTerrain.GetComponent<Map_TerrainController>();
		tc.SetFreezeMap();
		tc.SetTerrainHeightMap();
		tc.SetTerrainTexture (0, 0, tc.terrain.terrainData.alphamapWidth, tc.terrain.terrainData.alphamapHeight);
		Debug.Log ("hm reset");

		if (firstPlayer){
			firstPlayer = false;
			mapController.GetComponent<Level_MapController>().SetLevelObjects();
		}			
	}
}
