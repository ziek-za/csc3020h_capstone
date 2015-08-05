using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Char_AttributeScript : Photon.MonoBehaviour {

	public List<string> buffs;
	public int health = 125;
	public int speed = 125;
	public int energy = 100;
	public enum Teams {RED, BLUE};
	public Teams team;

	Level_GUIController HUD;
	public Char_SelectChar Respawner;

	// Use this for initialization
	void Start () {
		if (photonView.isMine){
			buffs= new List<string>();
			HUD = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();
			//Respawner = GameObject.Find("PlayerRespawner(Clone)").GetComponent<Char_SelectChar>();
			//team=Random.Range(0,2);
			int teamColour=Random.Range(1,3);
			if(teamColour==1){
				team=Teams.RED;
				joinTeam(new Vector3(Color.red.r, Color.red.g, Color.red.b));
			}
			else if(teamColour==2){
				team=Teams.BLUE;
				joinTeam(new Vector3(Color.blue.r, Color.blue.g, Color.blue.b));
			}
			Debug.Log(team);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine){
			//Debug.Log("Speed: "+speed);
			HUD.UpdateHUD(health);
			if (health <= 0 || Input.GetKeyDown(KeyCode.P)){
				KillPlayer(this.gameObject.GetComponent<PhotonView>().viewID);
				Respawner.spawned=false;
			}
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

	[RPC] void joinTeam(Vector3 color)
	{
		renderer.material.color = new Color(color.x, color.y, color.z, 1f);	
		if (photonView.isMine){
			photonView.RPC("joinTeam", PhotonTargets.OthersBuffered, color);
		}
	}
}
