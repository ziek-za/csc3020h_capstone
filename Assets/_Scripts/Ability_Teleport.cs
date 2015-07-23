using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability_Teleport : MonoBehaviour {
	public float teleportDistance = 15f;
	public GameObject projection;
	public GameObject distanceIndicator;
	
	Vector3 teleportDirection;

	// Use this for initialization

	void Start () {
		projection = Instantiate (projection, transform.position, Quaternion.identity) as GameObject;
		projection.transform.GetComponent<MeshRenderer> ().enabled = false;
		projection.transform.Find("BasicGun").GetComponent<MeshRenderer> ().enabled = false;
		//projection.transform.GetComponentInChildren<MeshRenderer> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetButton ("Teleport")) {
			projection.transform.GetComponent<MeshRenderer> ().enabled = true;
			projection.transform.Find("BasicGun").GetComponent<MeshRenderer> ().enabled = true;
			projection.transform.parent = transform;
			Vector2 screenCentrePoint = new Vector2 (Screen.width / 2, Screen.height / 2);
			Ray ray = Camera.main.ScreenPointToRay (screenCentrePoint);
			
			teleportDirection = new Vector3 (0, ray.direction.y , 1);
			
			Vector3 projectionPosition = teleportDirection * teleportDistance;
			projection.transform.localPosition = projectionPosition;
			if(Input.GetButtonDown ("Fire2")){
				Debug.Log("Fire2");
				Invoke ("Teleport", 0.2f); //Delay for teleporting
			}
		}
		if (Input.GetButtonUp ("Teleport")) {
			projection.transform.GetComponent<MeshRenderer> ().enabled = false;
			projection.transform.Find("BasicGun").GetComponent<MeshRenderer> ().enabled = false;
				}
	}

	void Teleport(){




		transform.Translate (teleportDirection * teleportDistance);
		Camera.main.transform.Translate(teleportDirection * teleportDistance);
	}
	
}
