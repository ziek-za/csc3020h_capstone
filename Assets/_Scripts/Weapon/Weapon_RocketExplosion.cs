using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Weapon_RocketExplosion : Photon.MonoBehaviour {

	GameObject hitCrosshair;

	List<GameObject> alreadyCollided;

	public float pushForce = 10f;
	public Char_AttributeScript whoFiredMe;
	
	// Use this for initialization
	void Start () {	
		hitCrosshair = GameObject.Find ("EnemyHitCrosshair");
		alreadyCollided = new List<GameObject>();
		particleSystem.Play();
		Invoke ("TriggerForce",0.1f);
		Invoke("DeathMethod",0.5f);
	}

	// Update is called once per frame
	void Update () {
	}

	void DeathMethod(){
		Destroy(gameObject);
	}


	//All objects in the area of effect live in alreadyCollided
	void OnTriggerEnter(Collider other){
		if (!alreadyCollided.Contains(other.gameObject)){
			alreadyCollided.Add(other.gameObject);
		}
	}
	
	void DisableHitCrosshair(){
		hitCrosshair.GetComponent<RawImage>().enabled = false;
	}
	
	void EnableHitCrosshair(){
		hitCrosshair.GetComponent<RawImage>().enabled = true;
	} 

	void TriggerForce(){
		if (whoFiredMe && whoFiredMe.gameObject.GetComponent<PhotonView>().isMine) {
			for (int i = 0; i < alreadyCollided.Count; i++){
				try {
					if (alreadyCollided[i].GetComponent<Rigidbody>() != null) {
						try {
							if (!alreadyCollided[i].GetComponent<Rigidbody>().isKinematic){
								Vector3 forceDir = alreadyCollided[i].transform.position - transform.position;
								PushForce(alreadyCollided[i].GetComponent<PhotonView>().viewID, Vector3.Normalize(forceDir) * pushForce);
							}

							float damage = -60/((alreadyCollided[i].transform.position - transform.position).magnitude + 1);

							//Hit Player
							if (alreadyCollided[i].GetComponent<Char_AttributeScript>()){

								//Less damage to self
								if (alreadyCollided[i].GetComponent<PhotonView>().isMine){
									DamagePlayer(-5,alreadyCollided[i].GetComponent<PhotonView>().viewID, transform.position);
								} else {
									DamagePlayer(Mathf.RoundToInt(damage),alreadyCollided[i].GetComponent<PhotonView>().viewID, transform.position);
								} 

								//Show kill success message
								try {
									if (whoFiredMe.team != alreadyCollided[i].GetComponent<Char_AttributeScript>().team){
										EnableHitCrosshair();
										Invoke("DisableHitCrosshair",0.1f);
										if (alreadyCollided[i].GetComponent<Char_AttributeScript>().health <= 0){
											whoFiredMe.EnableKillHUD(alreadyCollided[i].GetComponent<Char_AttributeScript>().playerName);
										}
									}
								} catch (System.NullReferenceException e){}
							} 

							//Hit Builder Link
							if (alreadyCollided[i].GetComponent<Ability_BuilderLink>()){
								DamageBuildingLink(Mathf.RoundToInt(damage-30),alreadyCollided[i].GetComponent<PhotonView>().viewID);
								EnableHitCrosshair();
								Invoke("DisableHitCrosshair",0.1f);
							}

							//Hit Builder Turret
							if (alreadyCollided[i].GetComponent<Ability_BuilderTurret>()){
								DamageBuildingTurret(Mathf.RoundToInt(damage-30),alreadyCollided[i].GetComponent<PhotonView>().viewID);
								EnableHitCrosshair();
								Invoke("DisableHitCrosshair",0.1f);
							}

							//Hit Builder Booster
							if (alreadyCollided[i].GetComponent<Ability_BuilderBooster>()){
								DamageBuildingBooster(Mathf.RoundToInt(damage-30),alreadyCollided[i].GetComponent<PhotonView>().viewID);
								EnableHitCrosshair();
								Invoke("DisableHitCrosshair",0.1f);
							}
						} catch (System.NullReferenceException e){
							//Debug.LogError("Null Ref on: " + alreadyCollided[i].name);
						}
					}

					//Hit destructible object
					if (alreadyCollided[i].GetComponentInParent<Map_DestructableObject>()){
						DamageDestructableObject(-10,alreadyCollided[i].GetComponentInParent<PhotonView>().viewID);
					} else if (alreadyCollided[i].GetComponent<Map_DestructableObject>()){
						DamageDestructableObject(-10,alreadyCollided[i].GetComponent<PhotonView>().viewID);
					} 

				} catch (System.Exception e){}
			}
		}
	}

	[RPC] void DamagePlayer(int damage, int vID, Vector3 rocketPos){
		try {
		Char_AttributeScript cas = PhotonView.Find(vID).transform.GetComponent<Char_AttributeScript>();
		cas.ChangeHP(damage, rocketPos);
		if (photonView.isMine)
			photonView.RPC("DamagePlayer", PhotonTargets.OthersBuffered, damage, vID, rocketPos);
		} catch (System.Exception e){}
	}

	[RPC] void DamageBuildingLink(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderLink>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingLink", PhotonTargets.OthersBuffered, damage, vID);
	}

	[RPC] void DamageBuildingTurret(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderTurret>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingTurret", PhotonTargets.OthersBuffered, damage, vID);
	}

	[RPC] void DamageBuildingBooster(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Ability_BuilderBooster>().ChangeHP(damage);
		if (photonView.isMine)
			photonView.RPC("DamageBuildingBooster", PhotonTargets.OthersBuffered, damage, vID);
	}
	
	[RPC] void DamageDestructableObject(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Map_DestructableObject>().Hit(damage);
		if (photonView.isMine)
			photonView.RPC("DamageDestructableObject", PhotonTargets.OthersBuffered, damage, vID);
	}

	//RPC to send the pushing force to all other clients
	[RPC] void PushForce(int vID, Vector3 force){
		PhotonView.Find(vID).gameObject.transform.rigidbody.AddForce(force);
		if (photonView.isMine)
			photonView.RPC("PushForce", PhotonTargets.OthersBuffered, vID, force);
	}
}
