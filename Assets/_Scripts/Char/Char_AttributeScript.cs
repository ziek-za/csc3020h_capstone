using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Char_AttributeScript : Photon.MonoBehaviour {

	public List<string> buffs;
	public int health = 125;
	public int speed = 125;
	public int energy = 100;

	Level_GUIController HUD;
	Char_SelectChar Respawner;

	// Use this for initialization
	void Start () {
		if (photonView.isMine){
			buffs= new List<string>();
			HUD = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine){
			//Debug.Log("Speed: "+speed);
			HUD.UpdateHUD(health);
		}
		if (health <= 0 || Input.GetKeyDown(KeyCode.P)){
			KillPlayer(this.gameObject.GetComponent<PhotonView>().viewID);
			Respawner.spawned=false;
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
