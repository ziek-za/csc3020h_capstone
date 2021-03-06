﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability_BuilderTurret : Photon.MonoBehaviour {

	public Char_AttributeScript.Teams currentTeam;
	public int health = 100;
	public GameObject muzzle;
	float timeBetweenShots = 0.1f;
	float shotCooldown;	

	int damage = 5;
	int localTargetID = -1;
	string localTargetName = "";

	public ParticleSystem muzzleFlash, tracerEffect;
	
	public List<GameObject> trackedEnemies;
	public Ability_BuilderPlaceFoundations whoBuiltMe;
	public GameObject[] partsToColour;
	public AudioClip build_turret;
	public AudioClip shoot_turret;

	Quaternion originalRotation, leftPanEdge, rightPanEdge, currentEdge;

	// Use this for initialization
	void Start () {
		whoBuiltMe.currentTurret = this.gameObject;
		AudioSource.PlayClipAtPoint (build_turret, transform.position);
		trackedEnemies = new List<GameObject>();
		originalRotation = transform.rotation;
		leftPanEdge.eulerAngles = originalRotation.eulerAngles + new Vector3(0,60f,0);
		rightPanEdge.eulerAngles = originalRotation.eulerAngles + new Vector3(0,-60f,0);
		currentEdge = leftPanEdge;
		// Set HP on GUI for building
		if (photonView.isMine)
			whoBuiltMe.GetComponent<Char_AttributeScript> ().HUD.turretIcon.SetBuildingHealth (health);
	}
	
	public void ChangeHP(int change){
		health += change;
		// Set HP on GUI for building
		if (photonView.isMine)
			whoBuiltMe.GetComponent<Char_AttributeScript> ().HUD.turretIcon.ActivateBuildingHealth (health);
	}

	float ClampToDegrees(float input){
		if (input < 0)
			return input + 360;
		else if (input > 360)
			return input - 360;
		else
			return input;
	}

	Quaternion ClampRotation(Quaternion toClamp){

		//Debug.Log(originalRotation.eulerAngles.y + 80);

		//X rotation i.e. up/down
		if (toClamp.eulerAngles.x < ClampToDegrees(originalRotation.eulerAngles.x-30) && toClamp.eulerAngles.x > ClampToDegrees(originalRotation.eulerAngles.x+60)){
			toClamp.eulerAngles = new Vector3(ClampToDegrees(originalRotation.eulerAngles.x-30),toClamp.eulerAngles.y,toClamp.eulerAngles.z);
		} else if (toClamp.eulerAngles.x < ClampToDegrees(originalRotation.eulerAngles.x-40) && toClamp.eulerAngles.x > ClampToDegrees(originalRotation.eulerAngles.x+50)){
			toClamp.eulerAngles = new Vector3(ClampToDegrees(originalRotation.eulerAngles.x+50),toClamp.eulerAngles.y,toClamp.eulerAngles.z);
		}

		/*
		//Y rotation i.e. left/right
		if (toClamp.eulerAngles.y < ClampToDegrees(originalRotation.eulerAngles.y-70) && toClamp.eulerAngles.y > ClampToDegrees(originalRotation.eulerAngles.y+80)){
			toClamp.eulerAngles = new Vector3(toClamp.eulerAngles.x,ClampToDegrees(originalRotation.eulerAngles.y-70),toClamp.eulerAngles.z);
		} else if (toClamp.eulerAngles.y < ClampToDegrees(originalRotation.eulerAngles.y-80) && toClamp.eulerAngles.y > ClampToDegrees(originalRotation.eulerAngles.y+70)){
			toClamp.eulerAngles = new Vector3(toClamp.eulerAngles.x,ClampToDegrees(originalRotation.eulerAngles.y+70),toClamp.eulerAngles.z);
		} 
		*/

		return toClamp;
	}

	bool RotationsEqual(Quaternion q1, Quaternion q2, float error){
		if (Mathf.Abs(Mathf.Abs(q1.eulerAngles.x) - Mathf.Abs(q2.eulerAngles.x)) > error)
			return false;
		if (Mathf.Abs(Mathf.Abs(q1.eulerAngles.y) - Mathf.Abs(q2.eulerAngles.y)) > error)
			return false;
		if (Mathf.Abs(Mathf.Abs(q1.eulerAngles.z) - Mathf.Abs(q2.eulerAngles.z)) > error)
			return false;
		return true;
	}

	void PanMode(){
		if (RotationsEqual(transform.rotation,leftPanEdge,5f)){
			currentEdge = rightPanEdge;
		} else if (RotationsEqual(transform.rotation,rightPanEdge,5f)){
			currentEdge = leftPanEdge;
		}
		transform.rotation = Quaternion.RotateTowards(ClampRotation(transform.rotation),currentEdge,0.2f);
	}

	void Update(){
		if (health <= 0){
			KillSelf();
		}

		if (!PhotonView.Find(localTargetID) && 
		    whoBuiltMe.GetComponent<PhotonView>().isMine &&
		    localTargetID != -1){
			whoBuiltMe.GetComponent<Char_AttributeScript>().EnableKillHUD(localTargetName);
			localTargetID = -1;
		}

		if (trackedEnemies.Count > 0){
			for (int i = 0; i < trackedEnemies.Count; i++){
				RaycastHit hit;
				Ray enemyTrackingRay = new Ray(transform.position, trackedEnemies[i].transform.position-transform.position);
				if(Physics.Raycast(enemyTrackingRay, out hit, 100f)){
					if (hit.transform.GetComponent<Char_AttributeScript>()){ //If we have a target player
						Vector3 offsetPos = transform.position - trackedEnemies[i].transform.position;
						Quaternion rotToTarget = Quaternion.LookRotation(offsetPos);
						float amountToRotate = Quaternion.Angle( transform.rotation, rotToTarget );
						transform.rotation = Quaternion.Slerp( ClampRotation(transform.rotation), rotToTarget, 20f * Time.deltaTime);

						if (Time.time >= shotCooldown){
							shotCooldown = Time.time + timeBetweenShots;
							muzzle.transform.rotation = Quaternion.RotateTowards(muzzle.transform.rotation,
							                                                     Quaternion.LookRotation(trackedEnemies[i].transform.position-muzzle.transform.position),
							                                                     5f);
							PlayMuzzleFlash(photonView.viewID);
							DamagePlayer(-damage, hit.transform.GetComponent<PhotonView>().viewID, transform.position);

							if (localTargetID != hit.transform.GetComponent<PhotonView>().viewID){
								localTargetID = hit.transform.GetComponent<PhotonView>().viewID;
								localTargetName = hit.transform.GetComponent<Char_AttributeScript>().playerName;
							}

							//Debug.Log(hit.transform.gameObject.GetComponent<Char_AttributeScript>().health);

							/*
							if (hit.transform.gameObject.GetComponent<Char_AttributeScript>().health <= 60 && whoBuiltMe.GetComponent<PhotonView>().isMine){
								Debug.Log("Kill message");
								localTargetID *= -1;
								whoBuiltMe.GetComponent<Char_AttributeScript>().EnableKillHUD(hit.transform.GetComponent<Char_AttributeScript>().playerName);
							}*/
						}
						break; //Turret must only have a single target at a time
						
					} else {
						PanMode();
					}
				} else {
					PanMode();
				}
			} //End For loop
		} else {
			PanMode();
		}
	}

	[RPC] void PlayMuzzleFlash(int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderTurret>().muzzleFlash.Play();
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderTurret>().tracerEffect.Play();
		AudioSource.PlayClipAtPoint (shoot_turret, transform.position);
		if (photonView.isMine)
			photonView.RPC("PlayMuzzleFlash", PhotonTargets.OthersBuffered, vID);
	}
	
	[RPC] void DamagePlayer(int damage, int vID, Vector3 shooterPos){
		try {
			Char_AttributeScript cas = PhotonView.Find(vID).transform.GetComponent<Char_AttributeScript>();
			cas.ChangeHP(damage, shooterPos);
			if (cas.health < 0){
				trackedEnemies.Remove(cas.gameObject);
			}
		} catch (System.Exception e){}
		if (photonView.isMine)
			photonView.RPC("DamagePlayer", PhotonTargets.OthersBuffered, damage, vID, shooterPos);
	}
	
	public void SetTeam(Char_AttributeScript.Teams newTeam){
		currentTeam = newTeam;
		if (currentTeam == Char_AttributeScript.Teams.BLUE)
			InitBlue();
		else if (currentTeam == Char_AttributeScript.Teams.RED)
			InitRed();
		//Invoke("KillSelf",lifeTime);
	}
	
	void InitBlue(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		for (int i = 0; i < partsToColour.Length; i++){
			partsToColour[i].GetComponent<MeshRenderer>().materials[0].color = Color.blue;
		}
	}

	void InitRed(){
		currentTeam = Char_AttributeScript.Teams.RED;
		this.GetComponent<MeshRenderer>().materials[0].color = Color.red;
		for (int i = 0; i < partsToColour.Length; i++){
			partsToColour[i].GetComponent<MeshRenderer>().materials[0].color =  Color.red;
		}
	}

	
	void OnTriggerExit(Collider other){
		if (other.GetComponent<Char_AttributeScript>() && other.GetComponent<Char_AttributeScript>().team != currentTeam &&
		    trackedEnemies.Contains(other.gameObject)){
			//Debug.Log("exit");
			trackedEnemies.Remove(other.gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Char_AttributeScript>() && other.GetComponent<Char_AttributeScript>().team != currentTeam){
			//Debug.Log("enter");
			trackedEnemies.Add(other.gameObject);
		}
	}

	void KillSelf(){
		gameObject.tag = "Untagged";
		particleSystem.Play();
		Invoke("ActuallyRemove",0.1f);
	}
	
	void ActuallyRemove(){
		Destroy(transform.parent.gameObject);
	}
}
