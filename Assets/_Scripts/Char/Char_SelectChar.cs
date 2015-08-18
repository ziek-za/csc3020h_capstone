using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Char_SelectChar : Photon.MonoBehaviour {

	public GameObject thiefPrefab, builderPrefab, soldierPrefab;
	public bool spawned=false;
	public static int classNo=9;

	public static Char_AttributeScript.Teams currentTeam = Char_AttributeScript.Teams.NONE;
	public static Vector3 spawnLocation = Vector3.zero;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			//Debug.Log("HELLO, is it me you're looking for?");
			if(!spawned){
				if(classNo==0){
					spawned=true;
					GameObject sol = PhotonNetwork.Instantiate(soldierPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					sol.GetComponent<Char_AttributeScript>().Respawner = this;
					sol.GetComponent<Char_AttributeScript>().team = currentTeam;
				}
				else if(classNo==1){
					spawned=true;
					GameObject thief = PhotonNetwork.Instantiate(thiefPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					thief.GetComponent<Char_AttributeScript>().Respawner = this;
					thief.GetComponent<Char_AttributeScript>().team = currentTeam;
				}
				else if(classNo==2){
					spawned=true;
					GameObject bul = PhotonNetwork.Instantiate(builderPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					bul.GetComponent<Char_AttributeScript>().Respawner = this;
					bul.GetComponent<Char_AttributeScript>().team = currentTeam;
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
