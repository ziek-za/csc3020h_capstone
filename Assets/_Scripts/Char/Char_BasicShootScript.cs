using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Char_BasicShootScript : Photon.MonoBehaviour {

	private RaycastHit hit;
	private Ray ray;
	protected float shotCooldown;	

	public GameObject hitCrosshair;
	public int damage = 10;

	public GameObject bulletHolePrefab;
	public ParticleSystem muzzleFlash;
	public ParticleSystem tracerEffect;
	public float timeBetweenShots = 0.333333f;
	public float weaponAccuracy = 1f; //Higher values = more accurate

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
		hitCrosshair = GameObject.Find ("EnemyHitCrosshair");
	}

	protected void DisableHitCrosshair(){
		hitCrosshair.GetComponent<RawImage>().enabled = false;
	}

	protected void EnableHitCrosshair(){
		hitCrosshair.GetComponent<RawImage>().enabled = true;
	} 

	protected void ResetTracerRotation(){
		tracerEffect.transform.localRotation =  Quaternion.identity;
	}

	protected int DamageAmount(){
		return -damage;
	}

	protected void Update()
	{
		if(photonView.isMine) {
			if(Input.GetButton("Fire1")){
				Char_BasicMoveScript.anim.SetBool("Shooting", true);
			} else {
				Char_BasicMoveScript.anim.SetBool("Shooting",false);
			}
		}

		if(photonView.isMine && Time.time >= shotCooldown && Input.GetButton("Fire1")) {
			shotCooldown = Time.time + timeBetweenShots;
			//muzzleFlash.Play();
			PlayMuzzleFlash(photonView.viewID);
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
				} else if (hit.collider.GetComponent<Map_DestructableObject>()) {
					DamageDestructableObject(-1, hit.transform.GetComponent<PhotonView>().viewID);
				//Bullet holes only on static objects and terrain
				} else if (hit.transform.gameObject.GetComponent<Terrain>() || 
				           (hit.transform.GetComponent<Rigidbody>() && hit.rigidbody.isKinematic)) {
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

	[RPC] protected void DamageDestructableObject(int damage, int vID){
		try {
			PhotonView.Find(vID).transform.GetComponent<Map_DestructableObject>().Hit(damage);
		} catch (System.Exception e) {}
		if (photonView.isMine)
			photonView.RPC("DamageDestructableObject", PhotonTargets.OthersBuffered, damage, vID);
	}
}
