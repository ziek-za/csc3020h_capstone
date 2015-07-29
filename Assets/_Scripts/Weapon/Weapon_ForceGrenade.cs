using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon_ForceGrenade : Photon.MonoBehaviour {

	public SphereCollider triggerArea;
	public float explosionPower = 100f;

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
		if (!alreadyCollided.Contains(other.gameObject)){
			if (other.GetComponent<Rigidbody>() != null)
				alreadyCollided.Add(other.gameObject);
		}
	}

	public void Explode(string mode){
		triggerArea.enabled = true;
		Invoke ("TriggerForce",0.05f);
		mode = this.mode;
		Destroy(gameObject,0.1f);
	}

	void TriggerForce(){
		for (int i = 0; i < alreadyCollided.Count; i++){
			Debug.Log (alreadyCollided[i].name);
			Debug.Log (explosionPower);
			StartCoroutine(ApplyForce(alreadyCollided[i]));
		}
	}

	IEnumerator ApplyForce(GameObject go){
		yield return new WaitForSeconds(0.1f);
		Vector3 forceDir = go.transform.position - transform.position;
		if (mode.Equals("push"))
			go.transform.rigidbody.AddForce(forceDir * explosionPower);
		else if (mode.Equals("pull"))
			go.transform.rigidbody.AddForce(forceDir * -explosionPower);
	}
}
