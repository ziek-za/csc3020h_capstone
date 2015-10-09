﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Char_BasicShootScript : Photon.MonoBehaviour {

	private RaycastHit hit;
	private Ray ray;
	protected float shotCooldown;	

	public GameObject hitCrosshair, headshotCrosshair;
	public int damage = 10;

	protected Char_BasicMoveScript animInstance;

	public GameObject bulletHolePrefab;
	public ParticleSystem muzzleFlash;
	public ParticleSystem tracerEffect;
	public float timeBetweenShots = 0.333333f;
	public float weaponAccuracy = 1f; //Higher values = more accurate

	//Variables for Camera Shake
	protected Vector3 originPosition;
	protected Quaternion originRotation;
	protected float shake_decay;
	protected float shake_intensity;
	public AudioClip firePistol;
	AudioSource audio;
	
	//Weapon accuracy is a public variable that can be changed in Unity
	// 100 ~ perfect accuracy
	// 1 ~ fairly accurate
	// 0.01 ~ wildly inaccurate
	protected float GenerateRandomOffset(float distance){
		int sign = -1;
		if (Random.value > 0.5)
			sign = 1;
		return sign * Random.value * distance/weaponAccuracy;
	}

	protected void Start(){
		animInstance = GetComponentInParent<Char_BasicMoveScript> ();
		hitCrosshair = GameObject.Find ("EnemyHitCrosshair");
		headshotCrosshair = GameObject.Find ("EnemyHeadshotCrosshair");
		audio = GetComponent<AudioSource> ();
	}

	protected void DisableHitCrosshair(){
		hitCrosshair.GetComponent<RawImage>().enabled = false;
	}

	protected void EnableHitCrosshair(){
		hitCrosshair.GetComponent<RawImage>().enabled = true;
	} 

	protected void DisableHeadshotCrosshair(){
		headshotCrosshair.GetComponent<RawImage>().enabled = false;
	}
	
	protected void EnableHeadshotCrosshair(){
		headshotCrosshair.GetComponent<RawImage>().enabled = true;
	} 

	protected void ResetTracerRotation(){
		tracerEffect.transform.localRotation =  Quaternion.identity;
	}

	protected int DamageAmount(){
		return -damage;
	}

    /*protected void CameraShake(float si, float sd){
		originPosition = animInstance.FPSCameraPos.localPosition;
		originRotation = animInstance.FPSCameraPos.localRotation;
		shake_intensity = si;
		shake_decay = sd;
	}*/

	protected void Update()
	{
		/*if (photonView.isMine){
			//Screen Shake
			if (shake_intensity > 0){
				animInstance.FPSCameraPos.localPosition = originPosition + Random.insideUnitSphere * shake_intensity;
				animInstance.FPSCameraPos.localRotation = new Quaternion(
					originRotation.x + Random.Range (-shake_intensity,shake_intensity) * .2f,
					originRotation.y + Random.Range (-shake_intensity,shake_intensity) * .2f,
					originRotation.z + Random.Range (-shake_intensity,shake_intensity) * .2f,
					originRotation.w + Random.Range (-shake_intensity,shake_intensity) * .2f);
				shake_intensity -= shake_decay;
			}
		}*/

		if(photonView.isMine) {
			if(Input.GetButton("Fire1")){
				animInstance.anim.SetBool("Shooting", true);
			} else {
				animInstance.anim.SetBool("Shooting",false);
			}
		}

		if(photonView.isMine && Time.time >= shotCooldown && Input.GetButton("Fire1")) {
			//CameraShake(0.03f,0.01f);
			shotCooldown = Time.time + timeBetweenShots;
			PlayMuzzleFlash(photonView.viewID);
			audio.PlayOneShot(firePistol);
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
				} else if (hit.transform.gameObject.GetComponent<Char_AttributeScript>()){
					if (hit.transform.gameObject.GetComponent<Char_AttributeScript>().team != transform.GetComponentInParent<Char_AttributeScript>().team){
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

	[RPC] protected void PlayMuzzleFlash(int vID){
		PhotonView.Find(vID).transform.GetComponent<ParticleSystem>().Play();
		if (photonView.isMine)
			photonView.RPC("PlayMuzzleFlash", PhotonTargets.OthersBuffered, vID);
	}

	[RPC] protected void DamagePlayer(int damage, int vID, Vector3 shooterPos){
		Char_AttributeScript cas = PhotonView.Find(vID).transform.GetComponent<Char_AttributeScript>();
		cas.ChangeHP(damage, shooterPos);
		if (photonView.isMine)
			photonView.RPC("DamagePlayer", PhotonTargets.OthersBuffered, damage, vID, shooterPos);
	}

	[RPC] protected void DamageBuildingLink(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderLink>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingLink", PhotonTargets.OthersBuffered, damage, vID);
	}

	[RPC] protected void DamageBuildingTurret(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderTurret>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingTurret", PhotonTargets.OthersBuffered, damage, vID);
	}

	[RPC] protected void DamageBuildingBooster(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderBooster>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingBooster", PhotonTargets.OthersBuffered, damage, vID);
	}

	[RPC] protected void DamageDestructableObject(int damage, int vID){
		try {
			PhotonView.Find(vID).transform.GetComponent<Map_DestructableObject>().Hit(damage);
		} catch (System.Exception e) {}
		if (photonView.isMine)
			photonView.RPC("DamageDestructableObject", PhotonTargets.OthersBuffered, damage, vID);
	}
}
