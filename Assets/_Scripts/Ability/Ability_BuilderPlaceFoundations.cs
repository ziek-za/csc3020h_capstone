using UnityEngine;
using System.Collections;

public class Ability_BuilderPlaceFoundations : Photon.MonoBehaviour {

	public float maxDistance = 2f;
	public GameObject linkFoundation;
	public GameObject turretFoundation;
	public GameObject boosterFoundation;
	public GameObject distanceIndicator;
	public GameObject aimingPoint;

	private float placeDistance;

	public int placeLinkEnergyCost = 10;
	public int placeLinkCD = 20;

	public int placeTurretEnergyCost = 10;
	public int placeTurretCD = 20;

	public int placeBoosterEnergyCost = 10;
	public int placeBoosterCD = 20;
	
	Vector3 teleportDirection;
	bool linkOffCooldown = true, turretOffCooldown = true, boosterOffCooldown = true;
	public GameObject currentLink, currentTurret, currentBooster;
	 
	// Use this for initialization
	void Start () {
		aimingPoint = Instantiate (aimingPoint, transform.position, Quaternion.identity) as GameObject;
		distanceIndicator = Instantiate (distanceIndicator, transform.position, Quaternion.identity) as GameObject;
		aimingPoint.transform.parent = transform;
		distanceIndicator.transform.parent = transform;
		distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
	}

	void linkCooledDown(){
		linkOffCooldown = true;
	}

	void turretCooledDown(){
		turretOffCooldown = true;
	}

	void boosterCooledDown(){
		boosterOffCooldown = true;
	}

	void CastShadowBuildingRay(){
		placeDistance=maxDistance;
		
		aimingPoint.transform.parent = transform;
		Vector2 screenCentrePoint = new Vector2 (Screen.width / 2, Screen.height / 2);
		Ray ray = Camera.main.ScreenPointToRay (screenCentrePoint);
		RaycastHit hit;
		if(Physics.Raycast (ray, out hit, Camera.main.farClipPlane)){
			if(hit.distance<maxDistance){
				placeDistance=hit.distance;
			}
		}

		teleportDirection = new Vector3 (0, ray.direction.y , 1);
		
		Vector3 projectionPosition = teleportDirection * placeDistance;
		projectionPosition.y += 1.5f;
		aimingPoint.transform.localPosition = projectionPosition;
		//projection.transform.localPosition = new Vector3(proje
		
		//Floor projection
		RaycastHit floorHit;
		Ray floorRay = new Ray(aimingPoint.transform.position, Vector3.down);
		if(Physics.Raycast(floorRay, out floorHit, Mathf.Infinity)){
			Debug.DrawRay(floorRay.origin, floorHit.point-floorRay.origin, Color.red);
			distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = true;
			Vector3 floorPosition = floorHit.point+new Vector3(0,0.5f,0)+floorHit.normal*0.01f;
			//Quaternion floorRotation = Quaternion.FromToRotation(-Vector3.forward, floorHit.normal);
			distanceIndicator.transform.position = floorPosition;
			Vector3 facingAway = transform.rotation.eulerAngles;
			facingAway.y += 180;
			distanceIndicator.transform.rotation=Quaternion.Euler(facingAway);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine){
			if (Input.GetButton("Building 1") && linkOffCooldown){

				CastShadowBuildingRay();

				if (Input.GetButtonDown("Fire2")
				    && transform.GetComponent<Char_AttributeScript>().energy >= placeLinkEnergyCost && linkOffCooldown){
					if (currentLink)
						DamageBuildingLink(-1000,currentLink.GetComponent<PhotonView>().viewID);

					transform.GetComponent<Char_AttributeScript>().energy -= placeLinkEnergyCost;
					Invoke("linkCooledDown",placeLinkCD);
					GetComponent<Char_AttributeScript>().HUD.linkIcon.ActivateCooldownGUI(placeLinkCD);
					distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
					linkOffCooldown = false;
					GameObject link = PhotonNetwork.Instantiate(linkFoundation.name,
					                                            distanceIndicator.transform.position,distanceIndicator.transform.rotation,0);
					//link.GetComponent<Ability_BuilderFoundation>().currentTeam = transform.GetComponent<Char_AttributeScript>().team;
					link.GetComponent<Ability_BuilderFoundation>().whoBuiltMe = this;
					SetBoxTeam (link.GetComponent<PhotonView>().viewID,gameObject.GetComponent<PhotonView>().viewID,(int)transform.GetComponent<Char_AttributeScript>().team);
				}

			} else if (Input.GetButton("Building 2") && turretOffCooldown){
				
				CastShadowBuildingRay();
				
				if (Input.GetButtonDown("Fire2")
				    && transform.GetComponent<Char_AttributeScript>().energy >= placeTurretEnergyCost && turretOffCooldown){
					if (currentTurret)
						DamageBuildingTurret(-1000,currentTurret.GetComponent<PhotonView>().viewID);

					transform.GetComponent<Char_AttributeScript>().energy -= placeTurretEnergyCost;
					Invoke("turretCooledDown",placeTurretCD);
					GetComponent<Char_AttributeScript>().HUD.turretIcon.ActivateCooldownGUI(placeTurretCD);
					distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
					turretOffCooldown = false;
					GameObject turret = PhotonNetwork.Instantiate(turretFoundation.name,
					                                              distanceIndicator.transform.position,distanceIndicator.transform.rotation,0);
					SetBoxTeam (turret.GetComponent<PhotonView>().viewID,gameObject.GetComponent<PhotonView>().viewID,(int)transform.GetComponent<Char_AttributeScript>().team);
				}
				//GameObject link = PhotonNetwork.Instantiate(linkFoundation.name,transform.position,Quaternion.identity,0);
				//link.GetComponent<Ability_BuilderFoundation>().currentTeam = transform.GetComponent<Char_AttributeScript>().team;
			} 

			else if (Input.GetButton("Building 3") && boosterOffCooldown){
				
				CastShadowBuildingRay();
				
				if (Input.GetButtonDown("Fire2")
				    && transform.GetComponent<Char_AttributeScript>().energy >= placeBoosterEnergyCost && boosterOffCooldown){

					if (currentBooster){
						DamageBuildingBooster(-1000,currentBooster.GetComponent<PhotonView>().viewID);
					}

					transform.GetComponent<Char_AttributeScript>().energy -= placeBoosterEnergyCost;
					Invoke("boosterCooledDown",placeBoosterCD);
					GetComponent<Char_AttributeScript>().HUD.boosterIcon.ActivateCooldownGUI(placeBoosterCD);
					distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
					boosterOffCooldown = false;
					GameObject booster = PhotonNetwork.Instantiate(boosterFoundation.name,
					                                              distanceIndicator.transform.position,distanceIndicator.transform.rotation,0);
					SetBoxTeam (booster.GetComponent<PhotonView>().viewID,gameObject.GetComponent<PhotonView>().viewID,(int)transform.GetComponent<Char_AttributeScript>().team);
				}
				//GameObject link = PhotonNetwork.Instantiate(linkFoundation.name,transform.position,Quaternion.identity,0);
				//link.GetComponent<Ability_BuilderFoundation>().currentTeam = transform.GetComponent<Char_AttributeScript>().team;
			}
			else {
				distanceIndicator.transform.GetComponent<MeshRenderer> ().enabled = false;
			}
		}
	}

	[RPC] public void DamageBuildingLink(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderLink>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingLink", PhotonTargets.OthersBuffered, damage, vID);
	}
	
	[RPC] public void DamageBuildingTurret(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderTurret>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingTurret", PhotonTargets.OthersBuffered, damage, vID);
	}
	
	[RPC] public void DamageBuildingBooster(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderBooster>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingBooster", PhotonTargets.OthersBuffered, damage, vID);
	}

	[RPC] void SetBoxTeam(int vID, int builderID, int team){
		PhotonView.Find(vID).GetComponent<Ability_BuilderFoundation>().currentTeam = (Char_AttributeScript.Teams) team;
		PhotonView.Find(vID).GetComponent<Ability_BuilderFoundation>().whoBuiltMe = PhotonView.Find(builderID).GetComponent<Ability_BuilderPlaceFoundations>();
		
		if (photonView.isMine)
			photonView.RPC("SetBoxTeam", PhotonTargets.OthersBuffered, vID, builderID, team);
	}
}
