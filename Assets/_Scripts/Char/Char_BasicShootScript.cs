﻿using UnityEngine;
using System.Collections;

public class Char_BasicShootScript : Photon.MonoBehaviour {

	private RaycastHit hit;
	private Ray ray;
	private float shotCooldown;	

	public GameObject bulletHolePrefab;
	public ParticleSystem muzzleFlash;
	public ParticleSystem tracerEffect;
	public float timeBetweenShots = 0.33333f;
	public float weaponAccuracy = 1f; //Higher values = more accurate

	//Weapon accuracy is a public variable that can be changed in Unity
	// 100 ~ perfect accuracy
	// 1 ~ fairly accurate
	// 0.01 ~ wildly inaccurate
	private float GenerateRandomOffset(float distance){
		int sign = -1;
		if (Random.value > 0.5)
			sign = 1;
		return sign * Random.value * distance/weaponAccuracy;
	}

	void Start(){
	}

	void Update()
	{
		if(photonView.isMine && Time.time >= shotCooldown && Input.GetButton("Fire1")) {
			shotCooldown = Time.time + timeBetweenShots;
			//muzzleFlash.Play();
			PlayMuzzleFlash(transform.position,transform.rotation);
			tracerEffect.Play();

			//First cast a perfectly accurate ray to get the distance to target
			Vector2 screenCenterPoint = new Vector2(Screen.width/2, Screen.height/2);
			ray = Camera.main.ScreenPointToRay(screenCenterPoint);
			Physics.Raycast(ray, out hit, Camera.main.farClipPlane);
			
			//Then ray cast randomly within in a circle to form a 'cone' closer targets are therefore more accurate and
			//further targets are harder to hit
			Vector2 screenOffset = new Vector2(screenCenterPoint.x + GenerateRandomOffset(hit.distance),
			                                   screenCenterPoint.y + GenerateRandomOffset(hit.distance));
			ray = Camera.main.ScreenPointToRay(screenOffset);
			if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane)) 
			{
				Debug.DrawLine(transform.position, hit.point, Color.red);
				if (hit.transform.gameObject.GetComponent<Char_AttributeScript>()){
					hit.transform.gameObject.rigidbody.AddForce(Vector3.Normalize(ray.direction)*100f);
				} else {
					//Create a bullet hole
					Vector3 bulletHolePosition = hit.point + hit.normal * 0.01f;
					Quaternion bulletHoleRotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
					GameObject hole = Instantiate(bulletHolePrefab, bulletHolePosition, bulletHoleRotation) as GameObject;
					Destroy(hole,10f);
				}
			}
		}
	}

	[RPC] void PlayMuzzleFlash(Vector3 position, Quaternion rotation){
		Vector3 mfPos = muzzleFlash.transform.position;
		Quaternion mfRot = muzzleFlash.transform.rotation;
		muzzleFlash.transform.position = position;
		muzzleFlash.transform.rotation = rotation;
		muzzleFlash.Play();
		muzzleFlash.transform.position = mfPos;
		muzzleFlash.transform.rotation = mfRot;
		if (photonView.isMine)
			photonView.RPC("PlayMuzzleFlash", PhotonTargets.OthersBuffered, position, rotation);
	}
}
