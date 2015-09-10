using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Char_AttributeScript : Photon.MonoBehaviour {

	private RaycastHit hit;
	private Ray ray;
	public string playerName;

	public List<string> buffs;
	public int health = 125;
	public int speed = 125;
	public int energy = 100;
	float enegryRegenRate = 0.5f;
	float linkRateCounter = 0.5f; //These values determine link regen rate
	float energyTrickeRate = 1f;
	public enum Teams {RED, BLUE, NONE};
	public Teams team = Teams.NONE;

	public GameObject weapon1, weapon2, weapon3;

	Level_GUIController HUD;
	public Char_SelectChar Respawner;

	public Map_LinkScript currentLink;

	// Use this for initialization
	void Start () {
		if (photonView.isMine){
			buffs= new List<string>();
			HUD = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();

			if(team == Teams.RED){
				joinTeam(new Vector3(Color.red.r, Color.red.g, Color.red.b), 0);
			}
			else if(team == Teams.BLUE){
				joinTeam(new Vector3(Color.blue.r, Color.blue.g, Color.blue.b), 1);
			}

			SetPlayerName(GetComponent<PhotonView>().viewID, playerName);
			Debug.Log(team);

			InvokeRepeating("energyTrickle",energyTrickeRate,energyTrickeRate);
		}
	}

	void energyTrickle(){
		if (energy < 100){
			energy++;
		}
	}

	[RPC] void NetworkChangeWeapons(int vID, int weapon){
		if (weapon == 1){
			weapon1.SetActive(true);
			weapon2.SetActive(false);
			weapon3.SetActive(false);
		} else if (weapon == 2){
			weapon1.SetActive(false);
			weapon2.SetActive(true);
			weapon3.SetActive(false);
		} else if (weapon == 3){
			weapon1.SetActive(false);
			weapon2.SetActive(false);
			weapon3.SetActive(true);
		}

		if (photonView.isMine)
			photonView.RPC ("NetworkChangeWeapons",PhotonTargets.OthersBuffered,vID,weapon);
	}

	void ChangeWeapons(){
		if (Input.GetButton("1")){
			NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,1);
		} else if (Input.GetButton("2")){
			NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,2);
		} else if (Input.GetButton("3")){
			NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,3);
		}
	}

	void LinkRegenEnergy(){
		if (energy < 100){
			energy ++;
		}
	}

	// Update is called once per frame
	void Update () {
		if (photonView.isMine){

			//Get playerName
			Vector2 screenCenterPoint = new Vector2(Screen.width/2, Screen.height/2);
			ray = Camera.main.ScreenPointToRay(screenCenterPoint);

			if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane)) 
			{
				Debug.DrawLine(transform.position, hit.point, Color.green);
				if (hit.transform.gameObject.GetComponent<Char_AttributeScript>()){
				}
			}

			if (currentLink && currentLink.currentTeam == team){
				linkRateCounter += Time.deltaTime;
				if (linkRateCounter >= enegryRegenRate){
					linkRateCounter -= enegryRegenRate;
					LinkRegenEnergy();
				} else {
					linkRateCounter = enegryRegenRate;
				}
			}
			HUD.UpdateHUDHealth(health);
			HUD.UpdateHUDEnergy(energy);
			ChangeWeapons();
			if (health <= 0 || Input.GetKey(KeyCode.P)){
				if (currentLink != null){
					ReduceCounter(currentLink.GetComponent<PhotonView>().viewID);
				}

				//Disabling various crosshairs
				try {
					GetComponentInChildren<Char_BasicShootScript>().hitCrosshair.GetComponent<RawImage>().enabled = false;
				} catch (System.NullReferenceException e){}
				try {
					GetComponentInChildren<Weapon_BuilderGlove>().buildCrosshair.GetComponent<RawImage>().enabled = false;
				} catch (System.NullReferenceException e){}

				Screen.lockCursor=false;
				GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(10,5,5);
				Camera.main.GetComponent<BlurEffect>().enabled=true;
				KillPlayer(this.gameObject.GetComponent<PhotonView>().viewID);
				Char_SelectChar.classNo=10;
				Respawner.spawned=false;


				//Resets the link buttons to show correct colors
				//HUD.SetUpLinkButtons();
				
			}
		}
	}

	[RPC] void ReduceCounter(int linkID){
		PhotonView.Find(linkID).GetComponent<Map_LinkScript>().PlayerDeathMethod(collider);
		if (photonView.isMine)
			photonView.RPC("ReduceCounter", PhotonTargets.OthersBuffered, linkID);
	}

	[RPC] public void EnterLink(int playerID,int linkID){
		if (PhotonView.Find (linkID).GetComponent<Map_LinkScript> ()){
			PhotonView.Find (playerID).GetComponent<Char_AttributeScript> ().currentLink =
				PhotonView.Find (linkID).GetComponent<Map_LinkScript> ();
		} 
		if (PhotonView.Find (linkID).GetComponent<Ability_BuilderLink> ()){
			PhotonView.Find (playerID).GetComponent<Char_AttributeScript> ().currentLink =
				PhotonView.Find (linkID).GetComponent<Ability_BuilderLink> ();
		}

		if (photonView.isMine)
			photonView.RPC("EnterLink", PhotonTargets.OthersBuffered, playerID, linkID);
	}

	[RPC] public void ExitLink(int playerID){
		PhotonView.Find (playerID).GetComponent<Char_AttributeScript> ().currentLink = null;
		if (photonView.isMine)
			photonView.RPC("ExitLink", PhotonTargets.OthersBuffered, playerID);
	}

	[RPC] void KillPlayer(int vID){
		Destroy(PhotonView.Find(vID).gameObject);
		
		if (photonView.isMine)
			photonView.RPC("KillPlayer", PhotonTargets.OthersBuffered, vID);
	}

	public void ChangeHP(int amount){
		health += amount;
	}

	[RPC] void joinTeam(Vector3 color, int myTeam)
	{
		renderer.material.color = new Color(color.x, color.y, color.z, 1f);
		team = (Teams)myTeam;
		if (photonView.isMine)
			photonView.RPC("joinTeam", PhotonTargets.OthersBuffered, color, myTeam);
	}

	[RPC] void SetPlayerName(int vID, string pName){
		PhotonView.Find(vID).GetComponent<Char_AttributeScript>().playerName = pName;
		if (photonView.isMine)
			photonView.RPC("SetPlayerName", PhotonTargets.OthersBuffered, vID, pName);
	}
}
