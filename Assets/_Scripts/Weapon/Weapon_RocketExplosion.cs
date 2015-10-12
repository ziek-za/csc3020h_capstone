using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Weapon_RocketExplosion : Photon.MonoBehaviour {

	public RawImage hitCrosshair;

	List<GameObject> alreadyCollided;

	public float pushForce = 10f;
	public Char_AttributeScript whoFiredMe;
	public ParticleSystem ps;
	//public AudioClip explode;
	AudioSource audio;

	public float buildingAffectRadius = 4f;
	// Use this for initialization
	void Start () {	
		audio = GetComponent<AudioSource> ();
		audio.Play ();
		//hitCrosshair = GameObject.Find ("EnemyHitCrosshair");
		alreadyCollided = new List<GameObject>();
		//particleSystem.Play();
		//AudioSource.PlayClipAtPoint (explode, transform.position);
		ps.gameObject.transform.SetParent(null);
		Destroy (ps.gameObject, 2.5f);
		Invoke ("TriggerForce",0.1f);
		Invoke("DeathMethod",1.0f);
		// Set particle system parent to null and destroy 2.5 seconds later
		// set particle system parent to null

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
		hitCrosshair.enabled = false;
	}
	
	void EnableHitCrosshair(){
		Debug.Log ("enabling cross hair");
		hitCrosshair.enabled = true;
	} 
	// no cross hair for indirect damage
	//kill message when not dead
	void TriggerForce(){
		if (whoFiredMe && whoFiredMe.gameObject.GetComponent<PhotonView>().isMine) {
			for (int i = 0; i < alreadyCollided.Count; i++) {
				try {
					if (alreadyCollided[i].GetComponent<Rigidbody>()) {
						if (!alreadyCollided[i].GetComponent<Rigidbody>().isKinematic){
							Vector3 forceDir = alreadyCollided[i].transform.position - transform.position;
							PushForce(alreadyCollided[i].GetComponent<PhotonView>().viewID, Vector3.Normalize(forceDir) * pushForce);
						}
						// Falloff on damage
						float damage = -60/((alreadyCollided[i].transform.position - transform.position).magnitude + 1);

						//Hit Player
						if (alreadyCollided[i].GetComponent<Char_AttributeScript>()){

							//Less damage to self
							if (alreadyCollided[i].GetComponent<PhotonView>().isMine){
								DamagePlayer(-5,alreadyCollided[i].GetComponent<PhotonView>().viewID, transform.position);
							} else {
								DamagePlayer(Mathf.RoundToInt(damage),alreadyCollided[i].GetComponent<PhotonView>().viewID, transform.position);
								// Show cross hair when damaging enemy players
								if (whoFiredMe.team != alreadyCollided[i].GetComponent<Char_AttributeScript>().team) {
									EnableHitCrosshair();
									Invoke("DisableHitCrosshair",0.1f);
									//Show kill success message if player is dead
									if (alreadyCollided[i].GetComponent<Char_AttributeScript>().health <= 0){
										whoFiredMe.EnableKillHUD(alreadyCollided[i].GetComponent<Char_AttributeScript>().playerName);
									}
								}
							} 
						} 

						//Hit Builder Link
						if (alreadyCollided[i].GetComponent<Ability_BuilderLink>()){
							DamageBuildingLink(Mathf.RoundToInt(damage-30),alreadyCollided[i].GetComponent<PhotonView>().viewID);
							EnableHitCrosshair();
							Invoke("DisableHitCrosshair",0.1f);
						//Hit Builder Turret
						} else if (alreadyCollided[i].GetComponent<Ability_BuilderTurret>()){
							DamageBuildingTurret(Mathf.RoundToInt(damage-30),alreadyCollided[i].GetComponent<PhotonView>().viewID);
							EnableHitCrosshair();
							Invoke("DisableHitCrosshair",0.1f);
						//Hit Builder Booster
						} else if (alreadyCollided[i].GetComponent<Ability_BuilderBooster>()){
							DamageBuildingBooster(Mathf.RoundToInt(damage-30),alreadyCollided[i].GetComponent<PhotonView>().viewID);
							EnableHitCrosshair();
							Invoke("DisableHitCrosshair",0.1f);
						}
					}
					//Hit terrain
					if (alreadyCollided[i].name.Equals("TerrainObject")) {
						PushTerrain(transform.position);
					//Hit destructible object
					} else if (alreadyCollided[i].GetComponentInParent<Map_DestructableObject>()){
						if (Vector3.Distance(alreadyCollided[i].transform.position, transform.position) <= buildingAffectRadius) {
							DamageDestructableObject(-10,alreadyCollided[i].GetComponentInParent<PhotonView>().viewID);
						}
					} else if (alreadyCollided[i].GetComponent<Map_DestructableObject>()){
						if (Vector3.Distance(alreadyCollided[i].transform.position, transform.position) <= buildingAffectRadius) {
							DamageDestructableObject(-10,alreadyCollided[i].GetComponent<PhotonView>().viewID);
						}
					} 

				} catch {}
			}
		}
	}

	//Used to modify the terrain on PUSH only
	[RPC] void PushTerrain(Vector3 explosion_pos){		
		//2 1.1
		Map_TerrainController mtc = Terrain.activeTerrain.GetComponent<Map_TerrainController> ();
		mtc.ManipulateTerrain(explosion_pos, 5f, "push", 30f, 2f, 2f);
		if (photonView.isMine) {
			photonView.RPC("PushTerrain",PhotonTargets.OthersBuffered, explosion_pos);
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
