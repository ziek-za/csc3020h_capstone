using UnityEngine;
using System.Collections;

public class Weapon_RocketLauncher : Char_BasicShootScript {

	public GameObject rocket;
	public GameObject charMesh;

	// Use this for initialization
	void Start () {
		//timeBetweenShots = 1f;
		hitCrosshair = GameObject.Find ("EnemyHitCrosshair");
	}

	public void RotateForRocketLauncher(bool forwards){
		if (forwards){
			charMesh.transform.Rotate(new Vector3(0,-70,0));
		} else {
			charMesh.transform.Rotate(new Vector3(0,70,0));
		}
	}
	
	// Update is called once per frame
	void Update () {

		/*
		if(photonView.isMine && Input.GetButtonDown("Fire1")){
			Debug.Log("Rotate");

			Quaternion rotToTarget = Quaternion.Euler(new Vector3(0,-60,0) + charMesh.transform.rotation.eulerAngles);
			charMesh.transform.rotation = Quaternion.Slerp(charMesh.transform.rotation, 
			                                                    rotToTarget, 1f * Time.deltaTime);

			charMesh.transform.Rotate(new Vector3(0,-70,0));
		} else if(photonView.isMine && Input.GetButtonUp("Fire1")){
			Debug.Log("Rotate back");

			Quaternion rotToTarget = Quaternion.Euler(new Vector3(0,60,0) + charMesh.transform.rotation.eulerAngles);
			charMesh.transform.rotation = Quaternion.Slerp(charMesh.transform.rotation, 
			                                                    rotToTarget, 1f * Time.deltaTime);

			charMesh.transform.Rotate(new Vector3(0,70,0));
		}
		*/

		if(photonView.isMine && Time.time >= shotCooldown && Input.GetButton("Fire1")) {
			shotCooldown = Time.time + timeBetweenShots;
			GameObject fired = PhotonNetwork.Instantiate(rocket.name,transform.position,transform.rotation,0) as GameObject;
			fired.GetComponent<Weapon_Rocket>().whoFiredMe = GetComponentInParent<Char_AttributeScript>();
			fired.rigidbody.AddForce(GetComponentInParent<Char_AttributeScript>().rigidbody.velocity + transform.up*400);

		}
	
	}
}
