using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Char_SelectChar : Photon.MonoBehaviour {

	public GameObject thiefPrefab,builderPrefab, soldierPrefab;
	public bool spawned=false;
	public static int classNo=9;
	// Use this for initialization
	void Start () {
		//this.name = "Player"+PhotonNetwork.countOfPlayers+"Spawner";
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			//Debug.Log("HELLO, is it me you're looking for?");
			if(!spawned){
				if(classNo==0){
					spawned=true;
					GameObject sol = PhotonNetwork.Instantiate(soldierPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0) as GameObject;
					sol.GetComponent<Char_AttributeScript>().Respawner = this;
				}
				else if(classNo==1){
					spawned=true;
					GameObject thief = PhotonNetwork.Instantiate(thiefPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0) as GameObject;
					thief.GetComponent<Char_AttributeScript>().Respawner = this;
				}
				else if(classNo==2){
					spawned=true;
					GameObject bul = PhotonNetwork.Instantiate(builderPrefab.name, new Vector3(1,10f,0), Quaternion.identity, 0) as GameObject;
					bul.GetComponent<Char_AttributeScript>().Respawner = this;
				}
				if(spawned){
					Screen.lockCursor=true;
					//if (Camera.main.GetComponent<BlurEffect>())
						//Debug.Log("blur here");
					Camera.main.GetComponent<BlurEffect>().enabled=false;
					GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(0,0,0);
				}
			}
		}
	}
}
