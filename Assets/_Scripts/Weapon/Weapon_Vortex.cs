using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_Vortex : Photon.MonoBehaviour {

	public float pullForce = 15f;
	public float duration = 2f;
	public SphereCollider triggerCollider;

	public ParticleSystem vortexParticles;

	bool rocketInVortex = false;
	//float rocketPullForce = 0f;

	List<GameObject> alreadyCollided = new List<GameObject>();
	bool recievedExplosion, channeling = false;

	// Use this for initialization
	void Start () {
		RecieveVortexRPC(transform.position, true);
	}

	void ChannelingTime(){
		try {
			for (int i = 0; i < alreadyCollided.Count; i++){
				if (alreadyCollided[i].GetComponent<Weapon_Rocket>()){
					alreadyCollided[i].GetComponent<Weapon_Rocket>().Explode(alreadyCollided[i]); //Blow up rocket at vortex end 
				} else if (alreadyCollided[i].GetComponent<Rigidbody>() != null && 
				           !alreadyCollided[i].GetComponent<Rigidbody>().isKinematic){
					Vector3 forceDir =  alreadyCollided[i].transform.position - transform.position; //Shoot objects out at vortex end
					alreadyCollided[i].transform.rigidbody.velocity = forceDir * pullForce * 0.1f;


					if (alreadyCollided[i].GetComponent<Char_AttributeScript>()){
						//Damage if rocket
						if (rocketInVortex){
							float damage = -40/((alreadyCollided[i].transform.position - transform.position).magnitude + 1);
							DamagePlayer(Mathf.RoundToInt(damage),alreadyCollided[i].GetComponent<PhotonView>().viewID);
							//if (alreadyCollided[i].GetComponent<Char_AttributeScript>().health <= 0){
							//	transform.parent.parent.parent.GetComponent<Char_AttributeScript>().EnableKillHUD(hit.transform.GetComponent<Char_AttributeScript>().playerName);
							//}
						}
					}

				}
			}
		} catch (MissingReferenceException e){}
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < alreadyCollided.Count; i++){
			try {
				if (alreadyCollided[i].GetComponent<Rigidbody>() != null){
					Vector3 forceDir = transform.position - alreadyCollided[i].transform.position;
					if (alreadyCollided[i].GetComponent<Weapon_Rocket>()){
						//Vector3 rocketPull = new Vector3(forceDir.x,-0.01f,forceDir.z)*rocketPullForce;
						Vector3 rocketPull = new Vector3(forceDir.x,0,forceDir.z);//*rocketPullForce;
						alreadyCollided[i].transform.rigidbody.AddForce(rocketPull * pullForce * 0.6f);
						//rocketPullForce += 0.1f;

					} else if (!alreadyCollided[i].GetComponent<Rigidbody>().isKinematic) {
						//Debug.Log((Vector3.Normalize(forceDir)*));
						alreadyCollided[i].transform.rigidbody.velocity = (forceDir * pullForce * 0.1f);
						//alreadyCollided[i].transform.rigidbody.AddForce(forceDir * pullForce);
					}
				}
			} catch (MissingReferenceException e){}
		}
		//Effect only starts when THIS client recieves an explosion
		if (recievedExplosion){
			if (!channeling){
				GameObject vp = Instantiate(vortexParticles.gameObject,transform.position,Quaternion.identity) as GameObject;
				vp.GetComponent<ParticleSystem>().Play();
				Destroy(vp,5f);
				Invoke ("ChannelingTime",duration);
				channeling = true;
			}
		}	 
	}

	[RPC] void KillGameObject(int vID){
		Destroy(PhotonView.Find(vID).gameObject);
		if (photonView.isMine)
				photonView.RPC("KillGameObject", PhotonTargets.OthersBuffered, vID);
	}

	//All objects in the area of effect live in alreadyCollided
	void OnTriggerEnter(Collider other){
		if (!alreadyCollided.Contains(other.gameObject) && other.gameObject.GetComponent<Weapon_ForceGrenade>() == null){
			alreadyCollided.Add(other.gameObject);
			if (other.gameObject.GetComponent<Weapon_Rocket>()){
				other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
				rocketInVortex = true;
			}

			//Need to disable player movement in vortex
			if (other.gameObject.GetComponent<Char_AttributeScript>()){
				DisablePlayerControls(true,other.gameObject.GetComponent<PhotonView>().viewID);
			}
		}
	}

	//All objects in the area of effect live in alreadyCollided
	void OnTriggerExit(Collider other){
		if (alreadyCollided.Contains(other.gameObject)){
			alreadyCollided.Remove(other.gameObject);
		}
	}

	//RPC to tell other clients to start the vortex
	[RPC] void RecieveVortexRPC(Vector3 vortexCenter, bool re){
		recievedExplosion = true;
		if (photonView.isMine) {
			photonView.RPC("RecieveVortexRPC",PhotonTargets.OthersBuffered, vortexCenter, true);
		}
	}

	[RPC] void DamagePlayer(int damage, int vID){
		Char_AttributeScript cas = PhotonView.Find(vID).transform.GetComponent<Char_AttributeScript>();
		cas.ChangeHP(damage, Vector3.zero);
		if (photonView.isMine)
			photonView.RPC("DamagePlayer", PhotonTargets.OthersBuffered, damage, vID);
	}

	[RPC] void DisablePlayerControls(bool controls, int vID){
		PhotonView.Find(vID).transform.GetComponent<Char_BasicMoveScript>().inVortex = controls;
		if (photonView.isMine)
			photonView.RPC("DisablePlayerControls", PhotonTargets.OthersBuffered, controls, vID);
	}

}
