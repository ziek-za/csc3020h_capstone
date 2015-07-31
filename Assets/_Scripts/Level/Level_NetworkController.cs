using UnityEngine;
using System.Collections;

public class Level_NetworkController : Photon.MonoBehaviour {

	public GameObject thiefPrefab,builderPrefab, soldierPrefab;
	public bool spawned=false;
	//public Camera cam;
	// Use this for initialization
	void Start () {
		//PhotonNetwork.ConnectUsingSettings("0.1");
		//QualitySettings.vSyncCount = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			if(!spawned){
				if(Input.GetKeyDown(KeyCode.Alpha1)){
					spawned=true;
					PhotonNetwork.Instantiate(soldierPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0);
				}
				else if(Input.GetKeyDown(KeyCode.Alpha2)){
					spawned=true;
					PhotonNetwork.Instantiate(thiefPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0);
				}
				else if(Input.GetKeyDown(KeyCode.Alpha3)){
					spawned=true;
					PhotonNetwork.Instantiate(builderPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0);
				}
			}
		}
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
	}
}
