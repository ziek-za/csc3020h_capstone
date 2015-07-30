using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_Vortex : Photon.MonoBehaviour {
		
	private Terrain terrain;
	private Map_TerrainController MTC;

	public float pullForce = 50f;
	public float duration = 2f;
	public SphereCollider triggerCollider;

	List<GameObject> alreadyCollided;
	bool recievedExplosion = false;

	// Use this for initialization
	void Start () {
		terrain = Terrain.activeTerrain;
		MTC = terrain.GetComponent<Map_TerrainController>();
		alreadyCollided = new List<GameObject>();
		PullTerrain(transform.position, true);
		//PhotonNetwork.Destroy(gameObject,duration);
		Invoke ("ChannelingTime",duration);
	}

	void ChannelingTime(){
		KillGameObject(transform.GetComponent<PhotonView>().viewID);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < alreadyCollided.Count; i++){
			if (alreadyCollided[i].GetComponent<Rigidbody>() != null){
				Vector3 forceDir = transform.position - alreadyCollided[i].transform.position;
				alreadyCollided[i].transform.rigidbody.AddForce(forceDir * pullForce);
			}
		}
		if (recievedExplosion)
			MTC.ManipulateTerrain(transform.position, 5f, "pull", 30f, 2f, 7f);
		//PullTerrain(transform.position);
		 
	}

	[RPC] void KillGameObject(int vID){
		Destroy(PhotonView.Find(vID).gameObject);
		if (photonView.isMine)
			photonView.RPC("KillGameObject", PhotonTargets.OthersBuffered, vID);
	}

	void OnTriggerEnter(Collider other){
		if (!alreadyCollided.Contains(other.gameObject) && other.gameObject.GetComponent<Weapon_ForceGrenade>() == null){
			alreadyCollided.Add(other.gameObject);
		}
		//Invoke("DelayForce",0.1f);
		//Invoke("DelayForce",0.5f);
		//Invoke("DelayForce",1f);
		//Invoke("DelayForce",1.5f);
	}

	[RPC] void PullTerrain(Vector3 vortexCenter, bool re){
		recievedExplosion = true;
		//MTC.ManipulateTerrain(vortexCenter, 5f, "pull", 30f, 2f, 7f);
		if (photonView.isMine) {
			photonView.RPC("PullTerrain",PhotonTargets.OthersBuffered, vortexCenter, true);
		}
	}	

}
