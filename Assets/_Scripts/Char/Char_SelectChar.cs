using UnityEngine;
using System.Collections;

public class Char_SelectChar : Photon.MonoBehaviour {

	public GameObject thiefPrefab,builderPrefab, soldierPrefab;
	public bool spawned=false;

	// Use this for initialization
	void Start () {
		//this.name = "Player"+PhotonNetwork.countOfPlayers+"Spawner";
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			if(!spawned){
				if(Input.GetKeyDown(KeyCode.Alpha1)){
					spawned=true;
					GameObject sol = PhotonNetwork.Instantiate(soldierPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0) as GameObject;
					sol.GetComponent<Char_AttributeScript>().Respawner = this;
				}
				else if(Input.GetKeyDown(KeyCode.Alpha2)){
					spawned=true;
					GameObject thief = PhotonNetwork.Instantiate(thiefPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0) as GameObject;
					thief.GetComponent<Char_AttributeScript>().Respawner = this;
				}
				else if(Input.GetKeyDown(KeyCode.Alpha3)){
					spawned=true;
					GameObject bul = PhotonNetwork.Instantiate(builderPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0) as GameObject;
					bul.GetComponent<Char_AttributeScript>().Respawner = this;
				}
			}
		}
	}
}
