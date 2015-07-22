using UnityEngine;
using System.Collections;

public class Ability_Teleport : MonoBehaviour {
	public float teleportDistance = 15f;
	// Use this for initialization

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Teleport")) {
			Invoke ("Teleport", 0.2f); //Delay for teleporting
		}
	}

	void Teleport(){
		Vector2 screenCentrePoint = new Vector2 (Screen.width / 2, Screen.height / 2);
		Ray ray = Camera.main.ScreenPointToRay (screenCentrePoint);

		Vector3 teleportDirection = new Vector3 (0, ray.direction.y , 1);
		transform.Translate (teleportDirection * teleportDistance);
		Camera.main.transform.Translate(teleportDirection * teleportDistance);
	}
	
}
