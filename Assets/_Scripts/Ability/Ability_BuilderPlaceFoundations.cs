using UnityEngine;
using System.Collections;

public class Ability_BuilderPlaceFoundations : Photon.MonoBehaviour {

	public GameObject linkFoundation;
	 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine){
			if (Input.GetButtonDown("Building 1")){
				GameObject link = PhotonNetwork.Instantiate(linkFoundation.name,transform.position,Quaternion.identity,0);
				link.GetComponent<Ability_BuilderFoundation>().currentTeam = transform.GetComponent<Char_AttributeScript>().team;
			}
		}
	}
}
