using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_Vortex : Photon.MonoBehaviour {
		
	private Terrain terrain;
	private Map_TerrainController MTC;

	public float pullForce = 15f;
	public float duration = 2f;
	public SphereCollider triggerCollider;

	List<GameObject> alreadyCollided = new List<GameObject>();
	bool recievedExplosion, channeling = false;

	// Use this for initialization
	void Start () {
		terrain = Terrain.activeTerrain;
		MTC = terrain.GetComponent<Map_TerrainController>();
		PullTerrain(transform.position, true);
	}

	void ChannelingTime(){
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < alreadyCollided.Count; i++){
			if (alreadyCollided[i].GetComponent<Rigidbody>() != null){
				Vector3 forceDir = transform.position - alreadyCollided[i].transform.position;
				alreadyCollided[i].transform.rigidbody.AddForce(forceDir * pullForce);
			}
		}
		//Effect only starts when THIS client recieves an explosion
		if (recievedExplosion){
			if (!channeling){
				Invoke ("ChannelingTime",duration);
				channeling = true;
			}
			MTC.ManipulateTerrain(transform.position, 5f, "pull", 30f, 2f, 7f);
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
		}
	}

	//RPC to tell other clients to start exploding
	[RPC] void PullTerrain(Vector3 vortexCenter, bool re){
		recievedExplosion = true;
		if (photonView.isMine) {
			photonView.RPC("PullTerrain",PhotonTargets.OthersBuffered, vortexCenter, true);
		}
	}	

}
