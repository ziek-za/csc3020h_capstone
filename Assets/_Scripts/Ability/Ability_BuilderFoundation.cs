using UnityEngine;
using System.Collections;

public class Ability_BuilderFoundation : Photon.MonoBehaviour {

	int completion = 0;
	int required = 60;

	public GameObject completedBuilding;

	public Char_AttributeScript.Teams currentTeam;

	GameObject tempObject;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (completion >= required){
			tempObject = PhotonNetwork.Instantiate(completedBuilding.name,transform.position,Quaternion.identity,0) as GameObject;
			SetTeam (tempObject.GetPhotonView().viewID,(int)currentTeam);
			PhotonNetwork.Destroy(gameObject);
		}
	}

	public bool Build(int amount, Char_AttributeScript.Teams builderTeam){
		if (builderTeam == currentTeam){
			completion += amount;
			return true;
		}
		return false;
	}

	[RPC] void SetTeam(int vID,int team){
		if (PhotonView.Find(vID).transform.GetComponent<Ability_BuilderLink>()) //Need one for each building type
			PhotonView.Find(vID).transform.GetComponent<Ability_BuilderLink>().SetTeam((Char_AttributeScript.Teams)team);
		else
			Debug.LogError("Need an if statement for that kind of building here");

		if (photonView.isMine)
			photonView.RPC("SetTeam", PhotonTargets.OthersBuffered,vID,team);
	}
}
