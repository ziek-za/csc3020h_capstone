using UnityEngine;
using System.Collections;

public class Ability_ForceGrenade: Photon.MonoBehaviour {

	public Transform grenadePrefab;
	public Transform grenadePosition;
	public int energyCost = 30;
	public float cooldown;

	GameObject cameraDirection;
	
	float initialForwardVelocity = 25f;
	float initialUpwardsVelocity = 5f;
	string mode = "push";
	bool offCooldown = true;
	GameObject gr;

	// Use this for initialization
	void Start () {
		cameraDirection = transform.FindChild("CameraPosition").gameObject; //Used so grenade is always thrown forwards
	}

	void cooledDown(){
		offCooldown = true;
	}

	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			if (Input.GetButtonDown("Fire2") && transform.GetComponent<Char_AttributeScript>().energy >= energyCost && offCooldown) {
				//Uses energy
				transform.GetComponent<Char_AttributeScript>().energy -= energyCost;
				//Start cooldown timer
				Invoke("cooledDown",cooldown);
				GetComponent<Char_AttributeScript>().HUD.vortexAndExplosionIcon.ActivateCooldownGUI(cooldown);

				offCooldown = false;
				//Creates the actual grenade over the photon network
				gr = PhotonNetwork.Instantiate(grenadePrefab.name, grenadePosition.position, Quaternion.identity, 0) as GameObject;
				gr.rigidbody.velocity = (cameraDirection.transform.forward * initialForwardVelocity) + 
										(cameraDirection.transform.up * initialUpwardsVelocity);
				gr.GetComponent<Weapon_ForceGrenade>().mode = mode;
				gr.GetComponent<Weapon_ForceGrenade>().whoThrewMe = GetComponentInParent<Char_AttributeScript>();
				gr.rigidbody.angularVelocity = transform.right*1000;
			} 
			else if (Input.GetButtonDown("Fire2") && gr){
				gr.GetComponent<Weapon_ForceGrenade>().Explode(gr.GetComponent<Weapon_ForceGrenade>().mode);
			}

			//Changing modes
			if (Input.GetButtonDown("Ability 1")){
				mode = "push";
				GetComponent<Char_AttributeScript>().HUD.vortexIcon.enabled = false;
				GetComponent<Char_AttributeScript>().HUD.explosionIcon.enabled = true;
			}
			if (Input.GetButtonDown("Ability 2")){
				mode = "pull";
				GetComponent<Char_AttributeScript>().HUD.vortexIcon.enabled = true;
				GetComponent<Char_AttributeScript>().HUD.explosionIcon.enabled = false;
			}

		}				               				              
	}
}
