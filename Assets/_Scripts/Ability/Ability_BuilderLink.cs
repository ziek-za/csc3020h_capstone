using UnityEngine;
using System.Collections;

public class Ability_BuilderLink : MonoBehaviour {

	//Set this value when initializing GameObject
	public Char_AttributeScript.Teams currentTeam;
	
	public ParticleSystem redBeam, blueBeam;

	public float lifeTime = 30f;

	Level_GUIController gui;

	// Use this for initialization
	void Start () {
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
		Invoke("KillSelf",lifeTime);
	}

	void InitBlue(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		blueBeam.Play();
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
