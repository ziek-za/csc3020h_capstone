using UnityEngine;
using System.Collections;

public class Level_NetworkController : MonoBehaviour {

	public GameObject playerPrefab;

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
		PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(1,1,0), Quaternion.identity, 0);
		Debug.Log("Connected to Room");
	}
}
