﻿using UnityEngine;
using System.Collections;

public class Ability_BuilderPlaceFoundations : Photon.MonoBehaviour {

	public float maxDistance = 2f;
	public GameObject linkFoundation;
	public GameObject distanceIndicator;
	public GameObject aimingPoint;

	private float placeDistance;

	public int placeLinkEnergyCost = 10;
	public int placeLinkCD = 25;
	
	Vector3 teleportDirection;
	bool linkOffCooldown = true;
	 
	// Use this for initialization
	void Start () {
		aimingPoint = Instantiate (aimingPoint, transform.position, Quaternion.identity) as GameObject;
		distanceIndicator = Instantiate (distanceIndicator, transform.position, Quaternion.identity) as GameObject;
		aimingPoint.transform.parent = transform;
		distanceIndicator.transform.parent = transform;
		distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
	}

	void linkCooledDown(){
		linkOffCooldown = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine){
			if (Input.GetButton("Building 1") && linkOffCooldown){
				placeDistance=maxDistance;
				
				aimingPoint.transform.parent = transform;
				Vector2 screenCentrePoint = new Vector2 (Screen.width / 2, Screen.height / 2);
				Ray ray = Camera.main.ScreenPointToRay (screenCentrePoint);
				RaycastHit hit;
				if(Physics.Raycast (ray, out hit, Camera.main.farClipPlane)){
					if(hit.distance<maxDistance){
						placeDistance=hit.distance;
					}
				}
				
				
				teleportDirection = new Vector3 (0, ray.direction.y , 1);
				
				Vector3 projectionPosition = teleportDirection * placeDistance;
				projectionPosition.y += 1.5f;
				aimingPoint.transform.localPosition = projectionPosition;
				//projection.transform.localPosition = new Vector3(proje
				
				//Floor projection
				RaycastHit floorHit;
				Ray floorRay = new Ray(aimingPoint.transform.position, Vector3.down);
				if(Physics.Raycast(floorRay, out floorHit, Mathf.Infinity)){
					Debug.DrawRay(floorRay.origin, floorHit.point-floorRay.origin, Color.red);
					distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = true;
					Vector3 floorPosition = floorHit.point+new Vector3(0,0.5f,0)+floorHit.normal*0.01f;
					Quaternion floorRotation = Quaternion.FromToRotation(-Vector3.forward, floorHit.normal);
					distanceIndicator.transform.position = floorPosition;
					distanceIndicator.transform.rotation=floorRotation;
				}

				if (Input.GetButtonDown("Fire2")
				    && transform.GetComponent<Char_AttributeScript>().energy >= placeLinkEnergyCost && linkOffCooldown){
					transform.GetComponent<Char_AttributeScript>().energy -= placeLinkEnergyCost;
					Invoke("linkCooledDown",placeLinkCD);
					distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
					linkOffCooldown = false;
					GameObject link = PhotonNetwork.Instantiate(linkFoundation.name,
					                                distanceIndicator.transform.position,distanceIndicator.transform.rotation,0);
					link.GetComponent<Ability_BuilderFoundation>().currentTeam = transform.GetComponent<Char_AttributeScript>().team;
				}
				//GameObject link = PhotonNetwork.Instantiate(linkFoundation.name,transform.position,Quaternion.identity,0);
				//link.GetComponent<Ability_BuilderFoundation>().currentTeam = transform.GetComponent<Char_AttributeScript>().team;
			}
			else if (Input.GetButtonUp("Building 1")){
				distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
			}
		}
	}
}
