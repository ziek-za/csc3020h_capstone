﻿using UnityEngine;
using System.Collections;

public class Weapon_RocketLauncher : Char_BasicShootScript {

	public GameObject rocket;

	// Use this for initialization
	void Start () {
		timeBetweenShots = 1f;
		hitCrosshair = GameObject.Find ("EnemyHitCrosshair");
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine && Time.time >= shotCooldown && Input.GetButton("Fire1")) {
			shotCooldown = Time.time + timeBetweenShots;
			GameObject fired = PhotonNetwork.Instantiate(rocket.name,transform.position,transform.rotation,0) as GameObject;
			fired.GetComponent<Weapon_Rocket>().whoFiredMe = GetComponentInParent<Char_AttributeScript>();
			fired.rigidbody.AddForce(GetComponentInParent<Char_AttributeScript>().rigidbody.velocity + transform.up*400);
		}
	
	}
}
