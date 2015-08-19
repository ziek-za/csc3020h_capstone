using UnityEngine;
using System.Collections;

public class Weapon_BuilderGlove : Photon.MonoBehaviour {

	private RaycastHit hit;
	private Ray ray;

	public ParticleSystem onHit, laserSystem;
	public float range = 7;

	// Use this for initialization
	void Start () {
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

						if (hit.transform.GetComponent<Ability_BuilderFoundation>()){
							hit.transform.GetComponent<Ability_BuilderFoundation>().Build(1,
						      	transform.GetComponentInParent<Char_AttributeScript>().team);
						}
					} else {
						onHit.startSize = 0f;
					}

						
				}
			}
		}
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
