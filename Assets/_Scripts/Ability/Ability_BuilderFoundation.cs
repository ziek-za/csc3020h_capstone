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
		//Debug.Log(completion);
		if (photonView.isMine){
			Debug.Log (completion);
			if (completion >= required){
				tempObject = PhotonNetwork.Instantiate(completedBuilding.name,transform.position,Quaternion.identity,0) as GameObject;
				SetTeam (tempObject.GetPhotonView().viewID,(int)currentTeam);
				DestroyBox(GetComponent<PhotonView>().viewID);
			}
		}
	}

	public void Build(int amount){
		completion += amount;
	}

	[RPC] void DestroyBox(int vID){
		Destroy(PhotonView.Find(vID).gameObject);

		if (photonView.isMine)
			photonView.RPC("DestroyBox", PhotonTargets.OthersBuffered, vID);
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
