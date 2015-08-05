using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_ForceGrenade : Photon.MonoBehaviour {

	public SphereCollider triggerArea;
	public float pushForce = 700f;
	public Transform vortex;

	List<GameObject> alreadyCollided;
	string mode;

	// Use this for initialization
	void Start () {	
		alreadyCollided = new List<GameObject>();
		mode = "none";
	}
	
	// Update is called once per frame
	void Update () {	
	}

	//All objects in the area of effect live in alreadyCollided
	void OnTriggerEnter(Collider other){
		if (!alreadyCollided.Contains(other.gameObject) && other.gameObject.GetComponent<Weapon_ForceGrenade>() == null){
			alreadyCollided.Add(other.gameObject);
		}
	}

	//Method is called by the Ability_ForceGrenade script when the grenade should activate (i.e. after fuse time)
	public void Explode(string newMode){
		triggerArea.enabled = true;
		Invoke ("TriggerForce",0.05f);
		mode = newMode; //Mode (push/pull) is recieved when this method is called
		Invoke("KillGODelay",0.1f); //Slight delay when removing the grenade to make sure forces have time to be applied
	}

	void KillGODelay(){
		KillGameObject(transform.GetComponent<PhotonView>().viewID);
	}

	//Method that applies the push force OR creates the pull vortex
	void TriggerForce(){
		for (int i = 0; i < alreadyCollided.Count; i++){
			if (mode.Equals("push") && alreadyCollided[i].GetComponent<Rigidbody>() != null){
				Vector3 forceDir = alreadyCollided[i].transform.position - transform.position;
				PushForceExplosion(alreadyCollided[i].GetComponent<PhotonView>().viewID, Vector3.Normalize(forceDir) * pushForce);
			}
			else if (mode.Equals("pull")){
				PhotonNetwork.Instantiate(vortex.name, transform.position, Quaternion.identity, 0);
				break; //Break because only want to create one vortex
			}
		}
	}

	//RPC to send the pushing force to all other clients
	[RPC] void PushForceExplosion(int vID, Vector3 force){
		PhotonView.Find(vID).gameObject.transform.rigidbody.AddForce(force);
		if (photonView.isMine)
			photonView.RPC("PushForceExplosion", PhotonTargets.OthersBuffered, vID, force);
	}

	//RPC to tell other clients to remove their grenade gameObjects as well
	[RPC] void KillGameObject(int vID){
		Destroy(PhotonView.Find(vID).gameObject);
		if (photonView.isMine)
			photonView.RPC("KillGameObject", PhotonTargets.OthersBuffered, vID);
	}
}
