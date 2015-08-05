using UnityEngine;
using System.Collections;

public class Level_NetworkController : MonoBehaviour {

	public Transform mapController, terrainController;
	public Transform respawner;

	//Variable to reset the hm when room first created
	public static bool hmReset;

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
		if (!hmReset){
			mapController.GetComponent<Level_MapController>().SetupLevel("1");
			//terrain.terrainData.SetHeights (0, 0, flatten_buf);
			//terrainController.GetComponent<Map_TerrainController>().SetTerrainHeightMap ();
			Debug.Log ("hm reset");
			hmReset = true;
		}

	}
}
