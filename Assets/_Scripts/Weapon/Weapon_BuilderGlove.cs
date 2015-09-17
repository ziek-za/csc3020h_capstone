using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Weapon_BuilderGlove : Photon.MonoBehaviour {

	private RaycastHit hit;
	private Ray ray;

	public ParticleSystem onHit, laserSystem;
	public float range = 7;

	public GameObject buildCrosshair;

	int buildRate = 3;
	int bRCounter = 0;

	// Use this for initialization
	void Start () {
		buildCrosshair = GameObject.Find ("BuildHitCrosshair");
		if (transform.GetComponentInParent<Char_AttributeScript>().team == Char_AttributeScript.Teams.BLUE){
			onHit.startColor = Color.blue;
			laserSystem.startColor = Color.blue;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine){
			if (Input.GetButtonDown("Fire1")){
				PlayLaser(transform.GetComponent<PhotonView>().viewID);
				//laserSystem.Play();

			} else if (Input.GetButtonUp("Fire1")){
				StopLaser(transform.GetComponent<PhotonView>().viewID);
				//laserSystem.Stop();
			}

			if (Input.GetButton("Fire1")){
				Vector2 screenCenterPoint = new Vector2(Screen.width/2, Screen.height/2);
				ray = Camera.main.ScreenPointToRay(screenCenterPoint);
				Physics.Raycast(ray, out hit, Camera.main.farClipPlane);
				
				if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane)) 
				{
					//In range of beam
					if (hit.distance < range){
						Debug.DrawLine(transform.position, hit.point, Color.green);
						onHit.startSize = 0.5f;

						//Building a foundation
						if (hit.transform.GetComponent<Ability_BuilderFoundation>() && 
						    transform.GetComponentInParent<Char_AttributeScript>().energy > 0){
							//Adjust build rate here
							bRCounter++;
							if (bRCounter > buildRate){
								if (transform.GetComponentInParent<Char_AttributeScript>().team == 
								    hit.transform.GetComponent<Ability_BuilderFoundation>().currentTeam)
								{
									buildCrosshair.GetComponent<RawImage>().enabled = true;
									Build (1,hit.transform.GetComponent<PhotonView>().viewID);
									transform.GetComponentInParent<Char_AttributeScript>().energy--;
									bRCounter = 0;
								}
							}
						}

						//Repairing a link
						else if (hit.transform.GetComponent<Ability_BuilderLink>() && 
						         transform.GetComponentInParent<Char_AttributeScript>().energy > 0){
							bRCounter++;
							if (bRCounter > buildRate){
								if (transform.GetComponentInParent<Char_AttributeScript>().team == 
								    hit.transform.GetComponent<Ability_BuilderLink>().currentTeam &&
								    hit.transform.GetComponent<Ability_BuilderLink>().health < 100)
								{
									buildCrosshair.GetComponent<RawImage>().enabled = true;
									RepairLink (2,hit.transform.GetComponent<PhotonView>().viewID);
									transform.GetComponentInParent<Char_AttributeScript>().energy--;
									bRCounter = 0;
								} else {
									buildCrosshair.GetComponent<RawImage>().enabled = false;
								}
							}
						}
					} else {
						onHit.startSize = 0f;
					}

						
				}
			} else {
				buildCrosshair.GetComponent<RawImage>().enabled = false;
			}
		}
	}

	[RPC] void RepairLink(int amount, int vID){
		PhotonView.Find(vID).GetComponent<Ability_BuilderLink>().ChangeHP(amount);
		if (photonView.isMine)
			photonView.RPC("RepairLink",PhotonTargets.OthersBuffered,amount,vID);
	}

	[RPC] void Build(int amount, int vID){
		try {
			PhotonView.Find(vID).GetComponent<Ability_BuilderFoundation>().Build(amount);
		} catch (System.NullReferenceException e){}
		if (photonView.isMine)
			photonView.RPC("Build",PhotonTargets.OthersBuffered,amount,vID);
	}

	[RPC] void PlayLaser(int vID){
		PhotonView.Find(vID).transform.GetComponentInChildren<ParticleSystem>().Play();
		if (photonView.isMine)
			photonView.RPC("PlayLaser", PhotonTargets.OthersBuffered, vID);
	}

	[RPC] void StopLaser(int vID){
		PhotonView.Find(vID).transform.GetComponentInChildren<ParticleSystem>().Stop();
		if (photonView.isMine)
			photonView.RPC("StopLaser", PhotonTargets.OthersBuffered, vID);
	}
}
