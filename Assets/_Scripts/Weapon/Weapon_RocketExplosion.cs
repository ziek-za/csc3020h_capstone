using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_RocketExplosion : Photon.MonoBehaviour {

	List<GameObject> alreadyCollided;

	public float pushForce = 10f;
	
	// Use this for initialization
	void Start () {	
		alreadyCollided = new List<GameObject>();
		particleSystem.Play();
		Destroy(gameObject,0.5f);
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
	
	void OnTriggerExit(Collider other){
		if (alreadyCollided.Contains(other.gameObject)){
			alreadyCollided.Remove(other.gameObject);
		}
	}

	void TriggerForce(){
		for (int i = 0; i < alreadyCollided.Count; i++){
			    if (alreadyCollided[i].GetComponent<Rigidbody>() != null &&
			    !alreadyCollided[i].GetComponent<Rigidbody>().isKinematic){
				try {
					Vector3 forceDir = alreadyCollided[i].transform.position - transform.position;
					PushForceExplosion(alreadyCollided[i].GetComponent<PhotonView>().viewID, Vector3.Normalize(forceDir) * pushForce);
				} catch (System.NullReferenceException e){
					Debug.LogError("Null Ref on: " + alreadyCollided[i].name);
				}
			}
		}
	}

	//RPC to send the pushing force to all other clients
	[RPC] void PushForceExplosion(int vID, Vector3 force){
		PhotonView.Find(vID).gameObject.transform.rigidbody.AddForce(force);
		if (photonView.isMine)
			photonView.RPC("PushForceExplosion", PhotonTargets.OthersBuffered, vID, force);
	}
}
