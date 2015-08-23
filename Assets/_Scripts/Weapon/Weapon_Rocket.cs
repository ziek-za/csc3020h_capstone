﻿using UnityEngine;
using System.Collections;

public class Weapon_Rocket : MonoBehaviour {

	public GameObject explosion;

	// Use this for initialization
	void Start () {
		Destroy(gameObject,10f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0,5,0));
		//rigidbody.velocity += Physics.gravity*0.005f;
	}

	public void Explode(){
		GameObject expl = Instantiate (explosion,transform.position,Quaternion.identity) as GameObject;
		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision other){
		Explode();
		/*GameObject expl = Instantiate (explosion,transform.position,Quaternion.identity) as GameObject;
		expl.GetComponent<ParticleSystem>().Play();
		Destroy(expl,5f);
		Destroy(gameObject);*/
	}
}
