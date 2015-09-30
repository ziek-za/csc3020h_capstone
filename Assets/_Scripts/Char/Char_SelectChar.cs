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
					// Set the current class in the char attribute script
					sol.GetComponent<Char_AttributeScript>().current_class = Char_AttributeScript.Class.SOLDIER;
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
					// Set the current class in the char attribute script
					thief.GetComponent<Char_AttributeScript>().current_class = Char_AttributeScript.Class.THIEF;
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
					// Set the current class in the char attribute script
					bul.GetComponent<Char_AttributeScript>().current_class = Char_AttributeScript.Class.BUILDER;
				}
				if(spawned){
					Screen.lockCursor=true;
					//if (Camera.main.GetComponent<BlurEffect>())
						//Debug.Log("blur here");
					Camera.main.GetComponent<BlurEffect>().enabled=false;
					GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(0,0,0);
					// Turn the HUD on
					gc.HUDPivot.SetActive(true);
					gc.isDeadBool = false;
					// Set active weapon
					gc.SetWeaponIcon(0, 1);
				} else {
					// Turn the HUD off
					gc.isDeadBool = true;
					gc.ResetIconsCooldown();
					gc.HUDPivot.SetActive(false);
				}
			}
		}
	}
}
