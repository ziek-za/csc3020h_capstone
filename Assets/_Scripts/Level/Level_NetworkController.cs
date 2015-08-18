using UnityEngine;
using System.Collections;

public class Level_NetworkController : Photon.MonoBehaviour {

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
		Debug.Log("Connected to Room");
		PhotonNetwork.Instantiate(respawner.name,Vector3.zero,Quaternion.identity,0);
		
		Map_TerrainController tc = Terrain.activeTerrain.GetComponent<Map_TerrainController>();
		tc.SetFreezeMap();
		tc.SetTerrainHeightMap();
		Debug.Log ("hm reset");

		if (firstPlayer){
			Debug.Log("Host Joined");
			firstPlayer = false;
			tc.SetClampMap ();
			tc.SetTerrainTexture (0, 0, tc.terrain.terrainData.alphamapWidth, tc.terrain.terrainData.alphamapHeight);
			mapController.GetComponent<Level_MapController>().SetLevelObjects(false);
		}			
	}
}
