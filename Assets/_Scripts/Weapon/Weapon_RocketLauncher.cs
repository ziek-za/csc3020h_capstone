using UnityEngine;
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
			GameObject fired = Instantiate(rocket,transform.position,transform.rotation) as GameObject;
			fired.rigidbody.AddForce(transform.up*200);
		}
	
	}
}
