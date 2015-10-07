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
	float enegryRegenRate = 0.1f;
	float linkRateCounter = 0.1f; //These values determine link regen rate
	float energyTrickeRate = 1f;
	public enum Teams {RED, BLUE, NONE};
	public Teams team = Teams.NONE;

	public GameObject weapon1, weapon2, weapon3;

	public enum Class {BUILDER, THIEF, SOLDIER};
	public enum Weapon {PISTOL, SHOTGUN, RIFLE, ROCKETLAUNCHER, GLOVE};
	public Class current_class;

	public GameObject thirdPersonPlayer, pistolMuzzleFlash, pistolFPSMuzzle; 
	public GameObject secondaryMuzzleFlash, secondaryFPSMuzzle, thirdPersonPistol, thirdPersonSecondary; 
	public GameObject builderGloveMuzzle, builderGloveFPSPos, thirdPersonBuilderGlove;

	public Level_GUIController HUD;
	Char_BasicMoveScript animInstance;
	public SkinnedMeshRenderer armour;
	public Char_SelectChar Respawner;

	public Map_LinkScript currentLink;

	// Use this for initialization
	void Start () {

		animInstance = GetComponent<Char_BasicMoveScript> ();

		if (photonView.isMine){
			buffs= new List<string>();
			HUD = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();
			animInstance.anim.SetBool("Pistol",true);
			animInstance.anim.SetBool("SecondaryWeapon",false);

			pistolMuzzleFlash.transform.parent = weapon1.transform;
			pistolMuzzleFlash.transform.position = pistolFPSMuzzle.transform.position;
			pistolMuzzleFlash.transform.rotation = Quaternion.identity;

			secondaryMuzzleFlash.transform.parent = weapon2.transform;
			secondaryMuzzleFlash.transform.position = secondaryFPSMuzzle.transform.position;
			secondaryMuzzleFlash.transform.localRotation = Quaternion.identity;

			//thirdPersonSecondary.SetActive(false);
			//secondaryMuzzleFlash.transform.parent.gameObject.SetActive(false);


			/*SkinnedMeshRenderer[] meshes = thirdPersonPlayer.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < meshes.Length; i++){
				meshes[i].enabled = false;
			}*/

			SkinnedMeshRenderer[] fpWeapon1 = weapon1.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < fpWeapon1.Length; i++){
				fpWeapon1[i].enabled = true;
			}

			SkinnedMeshRenderer[] fpWeapon2 = weapon2.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < fpWeapon2.Length; i++){
				fpWeapon2[i].enabled = true;
			}

			SkinnedMeshRenderer[] fpBuilderGlove = weapon3.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < fpBuilderGlove.Length; i++){
				fpBuilderGlove[i].enabled = true;
			}

			if(team == Teams.RED){
				joinTeam(new Vector3(Color.red.r, Color.red.g, Color.red.b), 0);
			}
			else if(team == Teams.BLUE){
				joinTeam(new Vector3(Color.blue.r, Color.blue.g, Color.blue.b), 1);
			}

			SetPlayerName(GetComponent<PhotonView>().viewID, _MainController.playerName);

			InvokeRepeating("energyTrickle",energyTrickeRate,energyTrickeRate);
		}
		weapon2.SetActive(false);
		thirdPersonSecondary.SetActive (false);
		weapon3.SetActive(false);
		thirdPersonBuilderGlove.SetActive(false); 
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
			thirdPersonPistol.SetActive(true);
			thirdPersonSecondary.SetActive(false);
			thirdPersonBuilderGlove.SetActive(false);
			//Debug.LogError(GetComponent<PhotonView>().viewID+" Pistol beforehand: "+animInstance.anim.GetBool("Pistol"));
			animInstance.anim.SetBool("Pistol",true);
			animInstance.anim.SetBool("SecondaryWeapon",false);
			//Debug.LogError(GetComponent<PhotonView>().viewID+" Pistol afterwards: "+animInstance.anim.GetBool("Pistol"));
		} else if (weapon == 2){
			weapon1.SetActive(false);
			weapon2.SetActive(true);
			weapon3.SetActive(false);
			thirdPersonPistol.SetActive(false);
			thirdPersonSecondary.SetActive(true);
			thirdPersonBuilderGlove.SetActive(false);
			//Debug.LogError(GetComponent<PhotonView>().viewID+" Secondary beforehand: "+animInstance.anim.GetBool("SecondaryWeapon"));
			animInstance.anim.SetBool("Pistol",false);
			animInstance.anim.SetBool("SecondaryWeapon",true);
			//Debug.LogError(GetComponent<PhotonView>().viewID+" Secondary bafterhand: "+animInstance.anim.GetBool("SecondaryWeapon"));

		} else if (weapon == 3){
			weapon1.SetActive(false);
			weapon2.SetActive(false);
			weapon3.SetActive(true);
			thirdPersonPistol.SetActive(false);
			thirdPersonSecondary.SetActive(false);
			thirdPersonBuilderGlove.SetActive(true);
		}

		if (photonView.isMine)
			photonView.RPC ("NetworkChangeWeapons",PhotonTargets.OthersBuffered,vID,weapon);
	}

	void ChangeWeapons(){
		if (Input.GetButtonDown("1")){
			NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,1);
			// Change weapon
			HUD.SetWeaponIcon(current_class, 1);
		} else if (Input.GetButtonDown("2")){
			NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,2);
			// Change weapon
			HUD.SetWeaponIcon(current_class, 2);
		} else if (Input.GetButton("3")){
			if (weapon3.name.Equals("BuilderGlove"))
			NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,3);
			// Change weapon
			HUD.SetWeaponIcon(current_class, 3);
		}
	}

	void LinkRegenEnergy(){
		if (energy < 100){
			energy ++;
		}
	}
	

	/*void DisableKillHUD(){
		HUD.playerKilledLabel.text = "";
		HUD.playerKilledLabel.CrossFadeAlpha(1,0.1f,false);
	}*/
	
	public void EnableKillHUD(string killedName){
		HUD.playerKilledLabel.CrossFadeAlpha(1,0.000001f,false);
		HUD.playerKilledLabel.text = "YOU JUST KILLED " + killedName;
		//Invoke("DisableKillHUD",2f);
		HUD.playerKilledLabel.CrossFadeAlpha(0,2,false);
	} 

	// Update is called once per frame
	void Update () {
		if (photonView.isMine){

			HUD.playerNameLabel.text = "";
			//Get playerName
			Vector2 screenCenterPoint = new Vector2(Screen.width/2, Screen.height/2);
			ray = Camera.main.ScreenPointToRay(screenCenterPoint);

			if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane)) 
			{
				Debug.DrawLine(transform.position, hit.point, Color.green);
				if (hit.transform.gameObject.GetComponent<Char_AttributeScript>() && 
				    hit.transform.gameObject.GetComponent<Char_AttributeScript>() != this){
					HUD.playerNameLabel.text = hit.transform.gameObject.GetComponent<Char_AttributeScript>().playerName;
				}
			}

			if (currentLink && currentLink.currentTeam == team){
				linkRateCounter += Time.deltaTime;
				if (linkRateCounter >= enegryRegenRate){
					linkRateCounter -= enegryRegenRate;
					LinkRegenEnergy();
				}
			} else {
				linkRateCounter = enegryRegenRate;
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
				gameObject.GetComponent<Char_BasicMoveScript>().inVortex=true;
				animInstance.anim.SetBool ("Dead", true);
				StartCoroutine("KillPlayerWait",this.gameObject.GetComponent<PhotonView>().viewID);
				//KillPlayer(this.gameObject.GetComponent<PhotonView>().viewID);
				Char_SelectChar.classNo=10;
				Respawner.spawned=false;
				try {
					GetComponentInChildren<WeaponSniperScript>().sniperCrosshair.GetComponent<RawImage>().enabled = false;
					Camera.main.fieldOfView = 60;
				} catch (System.Exception e) {}

				try {
					GetComponent<Ability_BuilderPlaceFoundations>().DamageBuildingLink(-1000,GetComponent<Ability_BuilderPlaceFoundations>().currentLink.GetComponent<PhotonView>().viewID);
				} catch (System.Exception e){}

				try {
					GetComponent<Ability_BuilderPlaceFoundations>().DamageBuildingTurret(-1000,GetComponent<Ability_BuilderPlaceFoundations>().currentTurret.GetComponent<PhotonView>().viewID);
				} catch (System.Exception e){}

				try {
					GetComponent<Ability_BuilderPlaceFoundations>().DamageBuildingBooster(-1000,GetComponent<Ability_BuilderPlaceFoundations>().currentBooster.GetComponent<PhotonView>().viewID);
				} catch (System.Exception e){}


				//Resets the link buttons to show correct colors
				//HUD.SetUpLinkButtons();
				
			}

			if (Input.GetKey(KeyCode.M)){
				Screen.lockCursor = true;
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

	IEnumerator KillPlayerWait(int vID){
		yield return new WaitForSeconds(0.417f);
		KillPlayer (vID);
	}

	[RPC] void KillPlayer(int vID){
		GameObject[] goList = GameObject.FindGameObjectsWithTag("Turret");
		for (int i = 0; i < goList.Length; i++){
			if (goList[i].GetComponent<Ability_BuilderTurret>().trackedEnemies.Contains(PhotonView.Find(vID).gameObject))
				goList[i].GetComponent<Ability_BuilderTurret>().trackedEnemies.Remove(PhotonView.Find(vID).gameObject);
		}

		Destroy(PhotonView.Find(vID).gameObject);
		if (photonView.isMine)
			photonView.RPC("KillPlayer", PhotonTargets.OthersBuffered, vID);
	}

	public void ChangeHP(int amount, Vector3 shooterPos){
		health += amount;

		if (photonView.isMine){
			if (!shooterPos.Equals(Vector3.zero)){

				Vector3 shotVector = transform.position - shooterPos;

				float angle = Vector3.Angle(shotVector, -transform.right);
				Vector3 cross = Vector3.Cross(shotVector, transform.right);
				if (cross.y > 0) angle = -angle;

				Vector3 rotationVector = new Vector3(0,0,angle);
				HUD.shotIndicatorPivot.transform.rotation = Quaternion.Euler(rotationVector);

				HUD.shotIndicator.GetComponent<Image>().CrossFadeAlpha(1,0.00001f,false);
				HUD.shotIndicator.GetComponent<Image>().enabled = true;
				HUD.shotIndicator.GetComponent<Image>().CrossFadeAlpha(0,2,false);
			}
		}
	}

	[RPC] void joinTeam(Vector3 color, int myTeam)
	{
		renderer.material.color = new Color(color.x, color.y, color.z, 1f);
		armour.material.color = new Color(color.x, color.y, color.z, 1f);
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
