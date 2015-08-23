using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_RocketExplosion : Photon.MonoBehaviour {

	List<GameObject> alreadyCollided;
	//public SphereCollider radius;

	public float pushForce = 10f;
	
	// Use this for initialization
	void Start () {	
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
			Debug.Log(other.gameObject.name);
		}
	}

	/*
	void OnTriggerExit(Collider other){
		if (alreadyCollided.Contains(other.gameObject)){
			alreadyCollided.Remove(other.gameObject);
		}
	}*/

	void TriggerForce(){
		for (int i = 0; i < alreadyCollided.Count; i++){
			    if (alreadyCollided[i].GetComponent<Rigidbody>() != null &&
			    !alreadyCollided[i].GetComponent<Rigidbody>().isKinematic){
				try {
					Vector3 forceDir = alreadyCollided[i].transform.position - transform.position;
					PushForce(alreadyCollided[i].GetComponent<PhotonView>().viewID, Vector3.Normalize(forceDir) * pushForce);
				} catch (System.NullReferenceException e){
					Debug.LogError("Null Ref on: " + alreadyCollided[i].name);
				}
			}
		}
	}

	//RPC to send the pushing force to all other clients
	[RPC] void PushForce(int vID, Vector3 force){
		PhotonView.Find(vID).gameObject.transform.rigidbody.AddForce(force);
		if (photonView.isMine)
			photonView.RPC("PushForceExplosion", PhotonTargets.OthersBuffered, vID, force);
	}
}
