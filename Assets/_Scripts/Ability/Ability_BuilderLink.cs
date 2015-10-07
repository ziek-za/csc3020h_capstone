using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability_BuilderLink : Map_LinkScript {

	//Set this value when initializing GameObject
	//public Char_AttributeScript.Teams currentTeam;
	
	//public ParticleSystem redBeam, blueBeam;

	public int health = 100;
	public Ability_BuilderPlaceFoundations whoBuiltMe;

	//Level_GUIController gui;

	// Use this for initialization
	void Start () {
		whoBuiltMe.currentLink = this.gameObject;
		// Set HP on GUI for building
		whoBuiltMe.GetComponent<Char_AttributeScript> ().HUD.linkIcon.SetBuildingHealth (health);
	}

	public void ChangeHP(int change){
		health += change;
		// Set HP on GUI for building
		whoBuiltMe.GetComponent<Char_AttributeScript> ().HUD.linkIcon.ActivateBuildingHealth (health);
	}

	void Update(){
		if (health <= 0){
			KillSelf();
		}
	}
	
	public void SetTeam(Char_AttributeScript.Teams newTeam){
		gui = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();
		currentTeam = newTeam;
		redBeam.Stop();
		blueBeam.Stop();
		if (currentTeam == Char_AttributeScript.Teams.BLUE)
			InitBlue();
		else if (currentTeam == Char_AttributeScript.Teams.RED)
			InitRed();
		gui.SetUpLinkButtons();
		//Invoke("KillSelf",lifeTime);
	}

	void InitBlue(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		newLinkPart1.materials[0].color =  Color.blue;
		newLinkPart2.materials[0].color =  Color.blue;
		blueBeam.Play();
	}

	void OnTriggerExit(Collider other){
		if (other.GetComponent<Char_AttributeScript>() && 
		    other.GetComponent<Char_AttributeScript>().team == currentTeam){
			other.GetComponent<Char_AttributeScript>().
				ExitLink(other.GetComponent<PhotonView>().viewID);
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Char_AttributeScript>() && 
		    other.GetComponent<Char_AttributeScript>().team == currentTeam){
			other.GetComponent<Char_AttributeScript>().
				EnterLink(other.GetComponent<PhotonView>().viewID, GetComponent<PhotonView>().viewID);
		}
	}
	
	void InitRed(){
		currentTeam = Char_AttributeScript.Teams.RED;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.red;
		newLinkPart1.materials[0].color =  Color.red;
		newLinkPart2.materials[0].color =  Color.red;
		redBeam.Play();
	}

	void KillSelf(){
		gameObject.tag = "Untagged";
		gui.SetUpLinkButtons();
		particleSystem.Play();
		Invoke("ActuallyRemove",0.1f);
	}

	void ActuallyRemove(){
		Destroy(gameObject);
	}
}
