using UnityEngine;
using System.Collections;

public class Weapon_RocketLauncher : Char_BasicShootScript {

	public GameObject rocket;
	public GameObject charMesh;

	// Use this for initialization
	void Start () {
		//timeBetweenShots = 1f;
		base.Start();
		//hitCrosshair = GameObject.Find ("EnemyHitCrosshair");
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

		if(photonView.isMine) {
			if(Input.GetButton("Fire1")){
				base.animInstance.anim.SetBool("Shooting", true);
			} else {
				base.animInstance.anim.SetBool("Shooting",false);
			}
		}

		if(photonView.isMine && Time.time >= shotCooldown && Input.GetButton("Fire1")) {
			shotCooldown = Time.time + timeBetweenShots;
			GameObject fired = PhotonNetwork.Instantiate(rocket.name,transform.position,transform.rotation,0) as GameObject;
			fired.GetComponent<Weapon_Rocket>().whoFiredMe = GetComponentInParent<Char_AttributeScript>();
			fired.rigidbody.AddForce(GetComponentInParent<Char_AttributeScript>().rigidbody.velocity + transform.up*400);

		}
	
	}
}
