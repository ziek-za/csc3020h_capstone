using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_ForceGrenade : Photon.MonoBehaviour {

	public SphereCollider triggerArea;
	public float explosionPower = 100f;
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

	void OnTriggerEnter(Collider other){
		if (!alreadyCollided.Contains(other.gameObject) && other.gameObject.GetComponent<Weapon_ForceGrenade>() == null){
			alreadyCollided.Add(other.gameObject);
		}
	}

	public void Explode(string newMode){
		triggerArea.enabled = true;
		Invoke ("TriggerForce",0.05f);
		mode = newMode;
		//TriggerForce();
		Destroy(gameObject,0.1f);
	}

	void TriggerForce(){
		for (int i = 0; i < alreadyCollided.Count; i++){
			if (mode.Equals("push") && alreadyCollided[i].GetComponent<Rigidbody>() != null){
				Vector3 forceDir = alreadyCollided[i].transform.position - transform.position;
				alreadyCollided[i].transform.rigidbody.AddForce(forceDir * explosionPower);
			}
			else if (mode.Equals("pull")){
				PhotonNetwork.Instantiate(vortex.name, transform.position, Quaternion.identity, 0);
			}				//go.transform.rigidbody.AddForce(forceDir * -explosionPower);
		}
	}	
}
