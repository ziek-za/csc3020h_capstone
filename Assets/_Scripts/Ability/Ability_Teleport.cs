using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability_Teleport : Photon.MonoBehaviour {
	public float maxDistance = 3f;
	public GameObject projection;
	public GameObject distanceIndicator;
	private float teleportDistance;

	public int energyCost = 50;
	public int cooldown = 5;

	public Material redMat;

	public ParticleSystem teleportOut, teleportIn;
	public AudioSource audio;
	public AudioClip teleport;

	Vector3 teleportDirection;
	bool offCooldown = true;

	// Use this for initialization

	void Start () {
		projection = Instantiate (projection, transform.position, Quaternion.identity) as GameObject;
		distanceIndicator = Instantiate (distanceIndicator, transform.position, Quaternion.identity) as GameObject;
		projection.transform.GetComponent<MeshRenderer> ().enabled = false;
		projection.transform.Find ("SparkleParticles").GetComponent<ParticleRenderer> ().enabled = false;
		projection.transform.Find("BasicGun").GetComponent<MeshRenderer> ().enabled = false;
		distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
		//projection.transform.GetComponentInChildren<MeshRenderer> ().enabled = false;
		projection.transform.parent = transform;
		distanceIndicator.transform.parent = transform;

		//Red team looks red
		if (GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.RED){
			projection.GetComponent<MeshRenderer> ().material = redMat;
			projection.transform.Find("BasicGun").GetComponent<MeshRenderer> ().material = redMat;
			distanceIndicator.GetComponent<MeshRenderer>().materials[0].color = Color.red;
		}

	}

	void cooledDown(){
		offCooldown = true;
	}

	// Update is called once per frame
	void Update () {


		if (photonView.isMine && Input.GetButton ("Ability 1")) {

			projection.transform.GetComponent<MeshRenderer> ().enabled = true;
			projection.transform.Find ("SparkleParticles").GetComponent<ParticleRenderer> ().enabled = true;
			projection.transform.Find("BasicGun").GetComponent<MeshRenderer> ().enabled = true;
			teleportDistance=maxDistance;

			projection.transform.parent = transform;
			Vector2 screenCentrePoint = new Vector2 (Screen.width / 2, Screen.height / 2);
			Ray ray = Camera.main.ScreenPointToRay (screenCentrePoint);
			RaycastHit hit;
			if(Physics.Raycast (ray, out hit, Camera.main.farClipPlane)){
				if(hit.distance<maxDistance){
					teleportDistance=hit.distance;
				}
			}


			teleportDirection = new Vector3 (0, ray.direction.y , 1);

			Vector3 projectionPosition = teleportDirection * teleportDistance;
			projectionPosition.y += 1.5f;
			projection.transform.localPosition = projectionPosition;
			//projection.transform.localPosition = new Vector3(proje

			//Floor projection
			RaycastHit floorHit;
			Ray floorRay = new Ray(projection.transform.position, Vector3.down);
			if(Physics.Raycast(floorRay, out floorHit, Mathf.Infinity)){
				Debug.DrawRay(floorRay.origin, floorHit.point-floorRay.origin, Color.red);
				distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = true;
				Vector3 floorPosition = floorHit.point+floorHit.normal*0.01f;
				Quaternion floorRotation = Quaternion.FromToRotation(-Vector3.forward, floorHit.normal);
				distanceIndicator.transform.position = floorPosition;
				distanceIndicator.transform.rotation=floorRotation;
			}

			if(Input.GetButtonDown ("Fire2") && 
			   transform.GetComponent<Char_AttributeScript>().energy >= energyCost && offCooldown)
			{
				transform.GetComponent<Char_AttributeScript>().energy -= energyCost;
				Invoke("cooledDown",cooldown);
				TeleoutEffect(transform.position);
				GetComponent<Char_AttributeScript>().HUD.teleportIcon.ActivateCooldownGUI(cooldown);
				offCooldown = false;


				Invoke ("Teleport", 0.2f); //Delay for teleporting
			}
		}
		if (photonView.isMine && Input.GetButtonUp ("Ability 1")) {
			projection.transform.GetComponent<MeshRenderer> ().enabled = false;
			projection.transform.Find("BasicGun").GetComponent<MeshRenderer> ().enabled = false;
			projection.transform.Find ("SparkleParticles").GetComponent<ParticleRenderer> ().enabled = false;
			distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
		}
	}

	void Teleport(){
		transform.Translate (teleportDirection * teleportDistance);
		Camera.main.transform.Translate(teleportDirection * teleportDistance);
		rigidbody.velocity = Vector3.zero;
		TeleInEffect(transform.position);
	}

	[RPC] void TeleoutEffect(Vector3 position){
		audio.PlayOneShot (teleport);
		GameObject teleOut = Instantiate(teleportOut.gameObject,position,Quaternion.identity) as GameObject;
		teleOut.GetComponent<ParticleSystem>().Play();
		Destroy(teleOut,2f);

		if (photonView.isMine)
			photonView.RPC("TeleoutEffect",PhotonTargets.OthersBuffered, position);
	}

	[RPC] void TeleInEffect(Vector3 position){
		GameObject teleIn = Instantiate(teleportIn.gameObject,position,Quaternion.identity) as GameObject;
		teleIn.GetComponent<ParticleSystem>().Play();
		Destroy(teleIn,2f);
		
		if (photonView.isMine)
			photonView.RPC("TeleInEffect",PhotonTargets.OthersBuffered, position);
	}
	
}
