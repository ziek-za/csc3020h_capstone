using UnityEngine;
using System.Collections;

public class Weapon_ShotgunScript : Char_BasicShootScript {

	private RaycastHit hit;
	private Ray ray;

	// Use this for initialization
	void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine) {
			if(Input.GetButton("Fire1")){
				animInstance.anim.SetBool("Shooting", true);
			} else {
				animInstance.anim.SetBool("Shooting",false);
			}
		}

		if(photonView.isMine && Time.time >= shotCooldown && Input.GetButton("Fire1")) {

			shotCooldown = Time.time + timeBetweenShots;
			PlayMuzzleFlash(photonView.viewID);
			tracerEffect.Play();

			for (int i = 0; i < 5; i++){
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
					//Damaging enemy players
					if (hit.transform.gameObject.GetComponent<Char_AttributeScript>()){
						if (hit.transform.gameObject.GetComponent<Char_AttributeScript>().team != transform.parent.parent.parent.GetComponent<Char_AttributeScript>().team){
							DamagePlayer(DamageAmount(), hit.transform.GetComponent<PhotonView>().viewID, transform.position);
							float timeTillHit = Vector3.Magnitude(hit.point - transform.position) / 90f;
							Invoke ("EnableHitCrosshair",timeTillHit);
							Invoke("DisableHitCrosshair",timeTillHit + 0.1f);
							
							if (hit.transform.gameObject.GetComponent<Char_AttributeScript>().health <= 0){
								transform.parent.parent.parent.GetComponent<Char_AttributeScript>().EnableKillHUD(hit.transform.GetComponent<Char_AttributeScript>().playerName);
							}
							
						}
						
						//Damaging builder 'links'
					} else if (hit.transform.gameObject.GetComponent<Ability_BuilderLink>()) {
						DamageBuildingLink(DamageAmount(),hit.transform.GetComponent<PhotonView>().viewID);
						float timeTillHit = Vector3.Magnitude(hit.point - transform.position) / 90f;
						Invoke ("EnableHitCrosshair",timeTillHit);
						Invoke("DisableHitCrosshair",timeTillHit + 0.1f);

						//Damaging buildings
					} else if (hit.collider.GetComponentInParent<Map_DestructableObject>()) {
						DamageDestructableObject(-1, hit.transform.GetComponentInParent<PhotonView>().viewID);
						Vector3 bulletHolePosition = hit.point + hit.normal * 0.01f;
						Quaternion bulletHoleRotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
						GameObject hole = Instantiate(bulletHolePrefab, bulletHolePosition, bulletHoleRotation) as GameObject;
						try {
							hole.transform.parent = hit.transform.GetComponentInChildren<Collider>().transform;
						} catch (System.Exception e){}
						Destroy(hole,10f);
					} else if (hit.collider.GetComponent<Map_DestructableObject>()) {
						DamageDestructableObject(-1, hit.transform.GetComponent<PhotonView>().viewID);
						Vector3 bulletHolePosition = hit.point + hit.normal * 0.01f;
						Quaternion bulletHoleRotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
						GameObject hole = Instantiate(bulletHolePrefab, bulletHolePosition, bulletHoleRotation) as GameObject;
						hole.transform.parent = hit.transform;
						Destroy(hole,10f);
						//Bullet holes only on static objects and terrain
					} else {
						Vector3 bulletHolePosition = hit.point + hit.normal * 0.01f;
						Quaternion bulletHoleRotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
						GameObject hole = Instantiate(bulletHolePrefab, bulletHolePosition, bulletHoleRotation) as GameObject;
						Destroy(hole,10f);
					}
				}
			}
		}
	}
}
