using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability_BuilderLink : Map_LinkScript {

	//Set this value when initializing GameObject
	//public Char_AttributeScript.Teams currentTeam;
	
	//public ParticleSystem redBeam, blueBeam;

	public float lifeTime = 40f;
	public int health = 100;

	float lifetimeAccum = 0;

	//Level_GUIController gui;

	// Use this for initialization
	void Start () {
	}

	public void ChangeHP(int change){
		health += change;
	}

	void Update(){
		lifetimeAccum += Time.deltaTime;

		if (lifetimeAccum >= lifeTime){
			KillSelf();
		}

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

	public void SetLifetime(float baselifetime){
		lifetimeAccum = baselifetime;
	}

	void InitBlue(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
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
