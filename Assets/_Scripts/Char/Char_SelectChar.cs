using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Char_SelectChar : Photon.MonoBehaviour {

	public GameObject thiefPrefab, builderPrefab, soldierPrefab;
	public bool spawned=false;
	public static int classNo=9;

	public static Char_AttributeScript.Teams currentTeam = Char_AttributeScript.Teams.NONE;
	public static Vector3 spawnLocation = Vector3.zero;

	private Level_GUIController gc;
	// Use this for initialization
	void Start () {
		// initialize the GUI Controller object
		gc = GameObject.Find ("GUI Controller").GetComponent<Level_GUIController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			//Debug.Log("HELLO, is it me you're looking for?");
			if(!spawned){
				// SOLDIER
				if(classNo==0){
					spawned=true;
					GameObject sol = PhotonNetwork.Instantiate(soldierPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					sol.GetComponent<Char_AttributeScript>().Respawner = this;
					sol.GetComponent<Char_AttributeScript>().team = currentTeam;
					// turn on the solider ability HUD
					gc.soldierHUD.gameObject.SetActive(true);
					gc.thiefHUD.gameObject.SetActive(false);
					gc.builderHUD.gameObject.SetActive(false);
				}
				// THIEF
				else if(classNo==1){
					spawned=true;
					GameObject thief = PhotonNetwork.Instantiate(thiefPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					thief.GetComponent<Char_AttributeScript>().Respawner = this;
					thief.GetComponent<Char_AttributeScript>().team = currentTeam;
					// turn on the theif ability HUD
					gc.soldierHUD.gameObject.SetActive(false);
					gc.thiefHUD.gameObject.SetActive(true);
					gc.builderHUD.gameObject.SetActive(false);
				}
				// BUILDER
				else if(classNo==2){
					spawned=true;
					GameObject bul = PhotonNetwork.Instantiate(builderPrefab.name, spawnLocation, Quaternion.identity, 0) as GameObject;
					bul.GetComponent<Char_AttributeScript>().Respawner = this;
					bul.GetComponent<Char_AttributeScript>().team = currentTeam;
					// turn on the builder ability HUD
					gc.soldierHUD.gameObject.SetActive(false);
					gc.thiefHUD.gameObject.SetActive(false);
					gc.builderHUD.gameObject.SetActive(true);
				}
				if(spawned){
					Screen.lockCursor=true;
					//if (Camera.main.GetComponent<BlurEffect>())
						//Debug.Log("blur here");
					Camera.main.GetComponent<BlurEffect>().enabled=false;
					GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(0,0,0);
					// Turn the HUD on
					gc.HUDPivot.SetActive(true);
				}
			}
		}
	}
}
