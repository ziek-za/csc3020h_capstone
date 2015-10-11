using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability_BuilderBooster : Photon.MonoBehaviour {

	public Char_AttributeScript.Teams currentTeam;
	public int health = 100;
	public Ability_BuilderPlaceFoundations whoBuiltMe;
	public GameObject[] partsToColour;
	public AudioClip build_booster, rampSound;

	float boostCooldown = 1f;
	float boostCooldownTimer = 1f;

	Rigidbody currentBoost;
	
	// Use this for initialization
	void Start () {
		whoBuiltMe.currentBooster = this.gameObject;
		AudioSource.PlayClipAtPoint (build_booster, transform.position);
		// Set HP on GUI for building
		if (photonView.isMine)
			whoBuiltMe.GetComponent<Char_AttributeScript> ().HUD.boosterIcon.SetBuildingHealth (health);
	}
	
	public void ChangeHP(int change){
		health += change;
		// Set HP on GUI for building
		if (photonView.isMine)
			whoBuiltMe.GetComponent<Char_AttributeScript> ().HUD.boosterIcon.ActivateBuildingHealth (health);
	}
	
	void Update(){

		boostCooldownTimer += Time.deltaTime;

		if (currentBoost){
			if (boostCooldownTimer > boostCooldown) {
				currentBoost.AddForce(-transform.forward*500 + Vector3.up*1000);
				//currentBoost.AddForce(Vector3.up*2000);
				boostCooldownTimer = 0;
			}
			currentBoost = null;
		}

		if (health <= 0){
			KillSelf();
		}		
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.GetComponent<Char_AttributeScript>() && collision.gameObject.GetComponent<Char_AttributeScript>().team == currentTeam) {
			AudioSource.PlayClipAtPoint (rampSound, transform.position);
			currentBoost = collision.rigidbody;
		}
	}
	
	public void SetTeam(Char_AttributeScript.Teams newTeam){
		currentTeam = newTeam;
		if (currentTeam == Char_AttributeScript.Teams.BLUE)
			InitBlue();
		else if (currentTeam == Char_AttributeScript.Teams.RED)
			InitRed();
	}
	
	void InitBlue(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		//this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		for (int i = 0; i < partsToColour.Length; i++){
			partsToColour[i].GetComponent<MeshRenderer>().materials[0].color = Color.blue;
		}
	}
	
	void InitRed(){
		currentTeam = Char_AttributeScript.Teams.RED;
		//this.GetComponent<MeshRenderer>().materials[0].color =  Color.red;
		for (int i = 0; i < partsToColour.Length; i++){
			partsToColour[i].GetComponent<MeshRenderer>().materials[0].color = Color.red;
		}
	}
	
	void KillSelf(){
		gameObject.tag = "Untagged";
		particleSystem.Play();
		Invoke("ActuallyRemove",0.1f);
	}
	
	void ActuallyRemove(){
		Destroy(gameObject);
	}
}
