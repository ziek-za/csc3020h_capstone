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
			if(attribInstance.weapon1.GetActive()){//If pistol
				//AudioSource.PlayClipAtPoint(fire_pistol,transform.position);
				SendShootSound(GetComponentInParent<PhotonView>().viewID, transform.position, 1);
			}
			else if(attribInstance.weapon2.GetActive()){//If secondary weapon
				if(attribInstance.current_class == Char_AttributeScript.Class.BUILDER){//If it is builder
					//AudioSource.PlayClipAtPoint(fire_shotgun,transform.position, 3);
					SendShootSound(GetComponentInParent<PhotonView>().viewID, transform.position, 3);
				}
				else if(attribInstance.current_class == Char_AttributeScript.Class.SOLDIER){//If it is soldier
					//Handled by itself seperately
				}
				else if(attribInstance.current_class == Char_AttributeScript.Class.THIEF){//If it is thief
					//AudioSource.PlayClipAtPoint(fire_rifle,transform.position, 2);
					SendShootSound(GetComponentInParent<PhotonView>().viewID, transform.position, 2);
				}
			}
			else if(attribInstance.weapon3.GetActive()){//If glove (only builder)
				if(attribInstance.current_class == Char_AttributeScript.Class.BUILDER){//Only builder has 3rd weapon
					//AudioSource.PlayClipAtPoint(fire_glove,transform.position, 4);
					SendShootSound(GetComponentInParent<PhotonView>().viewID, transform.position, 4);
				}
			}
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

					//Headshots do 2x damage
					if (hit.collider.gameObject.CompareTag("Head")){
						
						if (hit.transform.gameObject.GetComponent<Char_AttributeScript>().team != transform.GetComponentInParent<Char_AttributeScript>().team){
							DamagePlayer(2*DamageAmount(), hit.transform.GetComponent<PhotonView>().viewID, transform.position);
							EnableHeadshotCrosshair();
							Invoke("DisableHeadshotCrosshair",0.1f);
							if (hit.transform.gameObject.GetComponent<Char_AttributeScript>().health <= 0){
								transform.parent.parent.parent.GetComponent<Char_AttributeScript>().EnableKillHUD(hit.transform.GetComponent<Char_AttributeScript>().playerName);
							}
						}


					//Damaging enemy players
					}else if (hit.transform.gameObject.GetComponent<Char_AttributeScript>()){
						if (hit.transform.gameObject.GetComponent<Char_AttributeScript>().team != transform.parent.parent.parent.GetComponent<Char_AttributeScript>().team){
							DamagePlayer(DamageAmount(), hit.transform.GetComponent<PhotonView>().viewID, transform.position);
							EnableHitCrosshair();
							Invoke("DisableHitCrosshair",0.1f);
							
							if (hit.transform.gameObject.GetComponent<Char_AttributeScript>().health <= 0){
								transform.parent.parent.parent.GetComponent<Char_AttributeScript>().EnableKillHUD(hit.transform.GetComponent<Char_AttributeScript>().playerName);
							}
							
						}
						
						//Damaging builder 'links'
					} else if (hit.transform.gameObject.GetComponent<Ability_BuilderLink>()) {
						DamageBuildingLink(DamageAmount(),hit.transform.GetComponent<PhotonView>().viewID);
						EnableHitCrosshair();
						Invoke("DisableHitCrosshair",0.1f);

						//Damaging builder 'turrets'
					} else if (hit.transform.gameObject.GetComponent<Ability_BuilderTurret>()) {
						DamageBuildingTurret(DamageAmount(),hit.transform.GetComponent<PhotonView>().viewID);
						EnableHitCrosshair();
						Invoke("DisableHitCrosshair",0.1f);

						//Damaging builder 'boosters'
					} else if (hit.transform.gameObject.GetComponent<Ability_BuilderBooster>()) {
						DamageBuildingBooster(DamageAmount(),hit.transform.GetComponent<PhotonView>().viewID);
						EnableHitCrosshair();
						Invoke("DisableHitCrosshair",0.1f);

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
					} else if (!hit.collider.transform.CompareTag("MapEdge")) {
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
