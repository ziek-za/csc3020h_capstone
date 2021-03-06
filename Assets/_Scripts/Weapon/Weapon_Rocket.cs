﻿using UnityEngine;
using System.Collections;

public class Weapon_Rocket : Photon.MonoBehaviour {

	public GameObject explosion;
	public Char_AttributeScript whoFiredMe;
	public AudioClip fire_rocket;
	public AudioClip fire_rocketTrail;
	public ParticleSystem ps;
	AudioSource audio;
	
	private RaycastHit hit;

	// Use this for initialization
	void Start () {
		Destroy(gameObject,5f);
		audio = GetComponent<AudioSource> ();
		audio.PlayOneShot (fire_rocketTrail);
		AudioSource.PlayClipAtPoint (fire_rocket, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0,5,0));

		Vector3 fwd = transform.TransformDirection(Vector3.up);
		if (Physics.Raycast(transform.position, fwd,out hit, 1)) {
			Explode(hit.transform.gameObject);
		}
	}

	public void Explode(GameObject other){
		//int vID = whoFiredMe.GetComponent<PhotonView>().viewID;
		//ExplosionAtPoint (transform.position, vID);
		// set particle system parent to null
		try {
			if (whoFiredMe.GetComponent<PhotonView>().isMine) {
				GameObject expl = PhotonNetwork.Instantiate (explosion.name,transform.position,Quaternion.identity,0) as GameObject;
				expl.GetComponent<Weapon_RocketExplosion>().whoFiredMe = whoFiredMe;

				if (other.GetComponent<Char_AttributeScript>()) {
					DamagePlayer(-20,other.gameObject.GetComponent<PhotonView>().viewID);
					try {
						if (other.GetComponent<Char_AttributeScript>().health <= 0 && 
						    whoFiredMe.team != other.gameObject.GetComponent<Char_AttributeScript>().team){
							whoFiredMe.EnableKillHUD(other.transform.GetComponent<Char_AttributeScript>().playerName);
						}
					} catch (System.NullReferenceException e){}

				}
				DestroyRocket(GetComponent<PhotonView>().viewID);
			}
		} catch {
				}
	}

	void OnCollisionEnter(Collision other){
		Explode(other.gameObject);
	}

	[RPC] void DamagePlayer(int damage, int vID){
		Char_AttributeScript cas = PhotonView.Find(vID).transform.GetComponent<Char_AttributeScript>();
		cas.ChangeHP(damage, Vector3.zero);
		if (photonView.isMine)
			photonView.RPC("DamagePlayer", PhotonTargets.OthersBuffered, damage, vID);
	}

	[RPC] void DestroyRocket(int vID){
		try {
		GameObject rocket = PhotonView.Find(vID).gameObject;
		rocket.GetComponent<Weapon_Rocket>().ps.gameObject.transform.SetParent(null);
		rocket.GetComponent<Weapon_Rocket>().ps.Stop (true);
		Destroy (rocket.GetComponent<Weapon_Rocket>().ps.gameObject, 5f);
		Destroy(rocket);
		} catch {}

		if (photonView.isMine)
			photonView.RPC("DestroyRocket", PhotonTargets.OthersBuffered, vID);
	}

}
