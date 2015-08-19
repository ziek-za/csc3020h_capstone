using UnityEngine;
using System.Collections;

public class Ability_BuilderFoundation : Photon.MonoBehaviour {

	int completion = 0;
	int required = 100;

	public GameObject completedBuilding;
	public GameObject parentBuilder;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (completion >= required){
			GameObject tempLink = PhotonNetwork.Instantiate(completedBuilding.name,transform.position,Quaternion.identity,0) as GameObject;
			tempLink.GetComponent<Ability_BuilderLink>().SetTeam(parentBuilder.GetComponent<Char_AttributeScript>().team);
			PhotonNetwork.Destroy(gameObject);
			//Invoke ("Die",0.1f);
		}
	}

	public void Build(int amount){
		completion += amount;
	}

	void Die(){
		PhotonNetwork.Destroy(gameObject);
	}
}
