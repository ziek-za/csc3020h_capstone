using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_ForceGrenade : Photon.MonoBehaviour {

	public SphereCollider triggerArea;
	public float pushForce = 700f;
	public Transform vortex;
	public float fuseTime = 5;

	private Terrain terrain;
	private Map_TerrainController MTC;

	List<GameObject> alreadyCollided;
	public string mode;

	public ParticleSystem fuse, explosionEffect;
	bool detonated = false;
	bool madeVortex = false;

	// Use this for initialization
	void Start () {	
		terrain = Terrain.activeTerrain;
		alreadyCollided = new List<GameObject>();
		MTC = terrain.GetComponent<Map_TerrainController>();
		StartCoroutine("StartFuse");
		fuse.Play();
	}

	IEnumerator StartFuse(){
		yield return new WaitForSeconds(fuseTime);
		Explode(mode);
	}

	//All objects in the area of effect live in alreadyCollided
	void OnTriggerEnter(Collider other){
		if (!alreadyCollided.Contains(other.gameObject) && other.gameObject.GetComponent<Weapon_ForceGrenade>() == null){
			alreadyCollided.Add(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other){
		if (alreadyCollided.Contains(other.gameObject)){
			alreadyCollided.Remove(other.gameObject);
		}
	}

	void OnCollisionEnter(Collision collision){
		Debug.Log("collide");
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.drag = 1000f;
	}

	//Method is called by the Ability_ForceGrenade script when the grenade should activate (i.e. after fuse time)
	public void Explode(string newMode){
		if (!detonated){
			detonated = true;
			triggerArea.enabled = true;
			Invoke ("TriggerForce",0.05f);
			if (mode.Equals("push"))
				PlayExplosionParticleEffect();
			mode = newMode; //Mode (push/pull) is recieved when this method is called
			Invoke("KillGODelay",0.5f); //Slight delay when removing the grenade to make sure forces have time to be applied
		}
	}

	void KillGODelay(){
		KillGameObject(transform.GetComponent<PhotonView>().viewID);
	}

	//Method that applies the push force OR creates the pull vortex
	void TriggerForce(){
		fuse.gameObject.SetActive(false);
		GetComponent<MeshRenderer>().enabled = false;

		for (int i = 0; i < alreadyCollided.Count; i++){

			//Break Destructible objects
			try {
				if (alreadyCollided[i].GetComponentInParent<Map_DestructableObject>()){
					DamageDestructableObject(-10,alreadyCollided[i].GetComponentInParent<PhotonView>().viewID);
				} else if (alreadyCollided[i].GetComponent<Map_DestructableObject>()){
					DamageDestructableObject(-10,alreadyCollided[i].GetComponent<PhotonView>().viewID);
				} 
			} catch (System.Exception e){}

			if (alreadyCollided[i].name.Equals("TerrainObject")){
				if (mode.Equals("push")){
					PushTerrain(transform.position);
				}
				else if (mode.Equals("pull")){
					PullTerrain(transform.position);
				}
			}
				
			if (mode.Equals("push") &&
			    alreadyCollided[i].GetComponent<Rigidbody>() != null &&
			    !alreadyCollided[i].GetComponent<Rigidbody>().isKinematic){
				try {
					Vector3 forceDir = alreadyCollided[i].transform.position - transform.position;
					PushForceExplosion(alreadyCollided[i].GetComponent<PhotonView>().viewID, Vector3.Normalize(forceDir) * pushForce);
				} catch (System.NullReferenceException e){
					Debug.LogError("Null Ref on: " + alreadyCollided[i].name);
				}
			}
			else if (mode.Equals("pull") && !madeVortex){
				PhotonNetwork.Instantiate(vortex.name, transform.position, Quaternion.identity, 0);
				madeVortex = true; //Break because only want to create one vortex
			}
		}
	}

	//Used to modify the terrain on PUSH only
	[RPC] void PushTerrain(Vector3 explosion_pos){		
		//2 1.1
		MTC.ManipulateTerrain(explosion_pos, 5f, "push", 30f, 2f, 2f);
		if (photonView.isMine) {
			photonView.RPC("PushTerrain",PhotonTargets.OthersBuffered, explosion_pos);
		}
	}

	//Used to modify the terrain on PULL only
	[RPC] void PullTerrain(Vector3 explosion_pos){		
		MTC.ManipulateTerrain(transform.position, 5f, "pull", 30f, 2f, 2f);
		if (photonView.isMine) {
			photonView.RPC("PullTerrain",PhotonTargets.OthersBuffered, explosion_pos);
		}
	}

	//RPC to send the pushing force to all other clients
	[RPC] void PushForceExplosion(int vID, Vector3 force){
		PhotonView.Find(vID).gameObject.transform.rigidbody.AddForce(force);
		if (photonView.isMine)
			photonView.RPC("PushForceExplosion", PhotonTargets.OthersBuffered, vID, force);
	}

	[RPC] void PlayExplosionParticleEffect(){
		explosionEffect.Play();
		if (photonView.isMine)
			photonView.RPC("PlayExplosionParticleEffect", PhotonTargets.OthersBuffered);
	}

	//RPC to tell other clients to remove their grenade gameObjects as well
	[RPC] void KillGameObject(int vID){
		Destroy(PhotonView.Find(vID).gameObject);
		if (photonView.isMine)
			photonView.RPC("KillGameObject", PhotonTargets.OthersBuffered, vID);
	}

	[RPC] void DamageDestructableObject(int damage, int vID){
		PhotonView.Find(vID).transform.GetComponent<Map_DestructableObject>().Hit(damage);
		if (photonView.isMine)
			photonView.RPC("DamageDestructableObject", PhotonTargets.OthersBuffered, damage, vID);
	}
}
