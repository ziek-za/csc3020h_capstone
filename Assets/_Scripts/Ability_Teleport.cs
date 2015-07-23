using UnityEngine;
using System.Collections;

public class Ability_Teleport : MonoBehaviour {
	public float teleportDistance = 15f;
	public GameObject projection;

	// Use this for initialization

	void Start () {
		PhotonNetwork.Instantiate(projection.name, new Vector3(transform.position.x,transform.position.y,transform.position.z+2), Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 projectionPlacement = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 2);
		//projection.transform.position = projectionPlacement;
		//Destroy (projection);
		//Debug.Log (projection.transform.position);
		Debug.Log (projectionPlacement);
		if (Input.GetButtonDown ("Teleport")) {
			Invoke ("Teleport", 0.2f); //Delay for teleporting
		}
	}

	void Teleport(){

		//projection.transform.Translate (Vector3.forward * 5);
		Vector2 screenCentrePoint = new Vector2 (Screen.width / 2, Screen.height / 2);
		Ray ray = Camera.main.ScreenPointToRay (screenCentrePoint);

		Vector3 teleportDirection = new Vector3 (0, ray.direction.y , 1);
		transform.Translate (teleportDirection * teleportDistance);
		Camera.main.transform.Translate(teleportDirection * teleportDistance);
	}
	
}
