using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_Vortex : Photon.MonoBehaviour {
		
	private Terrain terrain;
	private Map_TerrainController MTC;

	public float pullForce = 15f;
	public float duration = 2f;
	public SphereCollider triggerCollider;

	//float rocketPullForce = 0f;

	List<GameObject> alreadyCollided = new List<GameObject>();
	bool recievedExplosion, channeling = false;

	// Use this for initialization
	void Start () {
		terrain = Terrain.activeTerrain;
		MTC = terrain.GetComponent<Map_TerrainController>();
		PullTerrain(transform.position, true);
	}

	void ChannelingTime(){
		for (int i = 0; i < alreadyCollided.Count; i++){
			if (alreadyCollided[i].GetComponent<Weapon_Rocket>()){
				Debug.Log("boom");
				alreadyCollided[i].GetComponent<Weapon_Rocket>().Explode();
			}
		}
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

					} else {
						alreadyCollided[i].transform.rigidbody.AddForce(forceDir * pullForce);
					}
				}
			} catch (MissingReferenceException e){}
		}
		//Effect only starts when THIS client recieves an explosion
		if (recievedExplosion){
			if (!channeling){
				Invoke ("ChannelingTime",duration);
				MTC.ManipulateTerrain(transform.position, 5f, "pull", 30f, 20f, 0.2f);
				channeling = true;
			}
			//MTC.ManipulateTerrain(transform.position, 5f, "pull", 30f, 2f, 7f); //Rising over time
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
			}
		}
	}

	//All objects in the area of effect live in alreadyCollided
	void OnTriggerExit(Collider other){
		if (alreadyCollided.Contains(other.gameObject)){
			alreadyCollided.Remove(other.gameObject);
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
