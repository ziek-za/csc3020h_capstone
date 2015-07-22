using UnityEngine;
using System.Collections;

public class Char_BasicShootScript : MonoBehaviour {

	public float timeBetweenShots = 0.3f;

	private RaycastHit hit;
	private Ray ray;
	private float shotCooldown;	

	void Start(){
		shotCooldown = timeBetweenShots;
	}

	void FixedUpdate(){
		if (shotCooldown > 0f)
			shotCooldown -= 0.02f;
	}

	void Update()
	{
		Vector2 screenCenterPoint = new Vector2(Screen.width/2, Screen.height/2);
		ray = Camera.main.ScreenPointToRay(screenCenterPoint);

		if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane)) 
		{
			Debug.DrawLine(transform.position, hit.point);
			if(Input.GetButtonDown("Fire1")) {
				if (hit.transform.gameObject.GetComponent<Char_BasicHealthScript>()){
					Char_BasicHealthScript hs = hit.transform.gameObject.GetComponent<Char_BasicHealthScript>();
					hs.health -= 10;
					//Quaternion hitDirection = Quaternion.FromToRotation(Vector3.forward, hit.normal);
					//Vector3.Normalize(hitDirection);
					hit.transform.gameObject.rigidbody.AddForce(Vector3.Normalize(ray.direction)*100f);
					shotCooldown = timeBetweenShots;
				}
			}
		}
	
	}
}
