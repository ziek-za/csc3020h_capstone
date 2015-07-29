using UnityEngine;
using System.Collections;

public class Char_AttributeScript : Photon.MonoBehaviour {
	
	int health = 100;
	Level_GUIController HUD;

	// Use this for initialization
	void Start () {
		if (photonView.isMine)
			HUD = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine)
			HUD.UpdateHUD(health);
		if (health <= 0 || Input.GetKeyDown(KeyCode.Escape)){
			KillPlayer(this.gameObject.GetComponent<PhotonView>().viewID);
		}
	}

	[RPC] void KillPlayer(int vID){
		Destroy(PhotonView.Find(vID).gameObject);
		if (photonView.isMine)
			photonView.RPC("KillPlayer", PhotonTargets.OthersBuffered, vID);
	}

	public void ChangeHP(int amount){
		health += amount;
	}
}
