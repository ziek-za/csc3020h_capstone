using UnityEngine;
using System.Collections;

public class Char_SelectChar : Photon.MonoBehaviour {

	public GameObject thiefPrefab,builderPrefab, soldierPrefab;
	public bool spawned=false;

	// Use this for initialization
	void Start () {
	
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
}
