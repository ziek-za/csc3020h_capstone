﻿using UnityEngine;
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

	public GameObject gloveEmitter, pistolMuzzleEmitter, secondaryMuzzleEmitter;

	public Level_GUIController HUD;
	Char_BasicMoveScript animInstance;
	public SkinnedMeshRenderer armour;
	public Char_SelectChar Respawner;
	public AudioClip pain1, pain2, pain3;
	public AudioClip healEnergy;
	bool hurtThreshold1,hurtThreshold2,hurtThreshold3,hurtThreshold4=false;
	//public AudioSource audioAttrib;
	public AudioSource audio;
	public AudioSource healSource;
	public AudioSource deathSource;

	public Map_LinkScript currentLink;

	bool prevWeaponGlove = false, prevWeaponRL = false;
	float respawnTimer = -10f;
	Color playerColor;
	float ScrollingMessageTimeout = 1f;

	//Used for player follow cam when dead
	Transform followCamPos;

	// Use this for initialization
	void Start () {

		animInstance = GetComponent<Char_BasicMoveScript> ();
		//audio = GetComponent<AudioSource> ();
		HUD = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();

		if (photonView.isMine){
			buffs= new List<string>();

			animInstance.anim.SetBool("Pistol",true);
			animInstance.anim.SetBool("SecondaryWeapon",false);

			pistolMuzzleFlash.transform.parent = weapon1.transform;
			pistolMuzzleFlash.transform.position = pistolFPSMuzzle.transform.position;
			pistolMuzzleFlash.transform.rotation = Quaternion.identity;

			pistolMuzzleEmitter.transform.position = pistolFPSMuzzle.transform.position;

			secondaryMuzzleFlash.transform.parent = weapon2.transform;
			secondaryMuzzleFlash.transform.position = secondaryFPSMuzzle.transform.position;
			secondaryMuzzleFlash.transform.localRotation = Quaternion.identity;

			secondaryMuzzleEmitter.transform.position = secondaryFPSMuzzle.transform.position;

			if (builderGloveMuzzle && !builderGloveMuzzle.transform.name.Equals("EmptyGameObject")){
				builderGloveMuzzle.transform.parent = weapon3.transform;
				builderGloveMuzzle.transform.position = builderGloveFPSPos.transform.position;
				//builderGloveMuzzle.transform.localRotation = Quaternion.identity;
				builderGloveMuzzle.transform.localRotation = Quaternion.Euler(new Vector3(15,60,290));
				//builderGloveMuzzle.transform.GetComponentInChildren<ParticleSystem>().transform.localPosition = Vector3.zero;

				gloveEmitter.transform.localPosition = new Vector3(0.3810017f, -0.261f, 1.086f);
				//gloveEmitter.GetComponentInChildren<ParticleSystem>().Play();
			}
				

			//thirdPersonSecondary.SetActive(false);
			//secondaryMuzzleFlash.transform.parent.gameObject.SetActive(false);

			MeshRenderer[] fpWeapon2 = weapon2.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < fpWeapon2.Length; i++){
				fpWeapon2[i].enabled = true;
			}

			MeshRenderer[] meshes = thirdPersonPlayer.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < meshes.Length; i++){
				meshes[i].enabled = false;
			}

			SkinnedMeshRenderer[] skMeshes = thirdPersonPlayer.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < skMeshes.Length; i++){
				skMeshes[i].enabled = false;
			}
			
			SkinnedMeshRenderer[] skfpWeapon1 = weapon1.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < skfpWeapon1.Length; i++){
				skfpWeapon1[i].enabled = true;
			}
			
			SkinnedMeshRenderer[] skfpWeapon2 = weapon2.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < skfpWeapon2.Length; i++){
				skfpWeapon2[i].enabled = true;
			}
			
			MeshRenderer[] fpBuilderGlove = weapon3.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < fpBuilderGlove.Length; i++){
				fpBuilderGlove[i].enabled = true;
			}

			SetPlayerName(GetComponent<PhotonView>().viewID, _MainController.playerName);

			if(team == Teams.RED){
				joinTeam(new Vector3(Color.red.r, Color.red.g, Color.red.b), 0, playerName + " spawned as RED");
				playerColor = Color.red;
			}
			else if(team == Teams.BLUE){
				joinTeam(new Vector3(Color.blue.r, Color.blue.g, Color.blue.b), 1, playerName + " spawned as BLUE");
				playerColor = Color.blue;
			}

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
			animInstance.anim.SetBool("Glove",false);

			if (weapon2.name.Equals("RocketLauncher") && prevWeaponRL){
				secondaryMuzzleFlash.GetComponent<Weapon_RocketLauncher>().RotateForRocketLauncher(false);
				prevWeaponRL = false;
			}

			if (weapon3.name.Equals("Builder_FP_glove") && prevWeaponGlove){
				builderGloveMuzzle.GetComponent<Weapon_BuilderGlove>().RotateForGlove(false);
				prevWeaponGlove = false;
			}

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
			animInstance.anim.SetBool("Glove",false);
			//Debug.LogError(GetComponent<PhotonView>().viewID+" Secondary bafterhand: "+animInstance.anim.GetBool("SecondaryWeapon"));

			if (weapon2.name.Equals("RocketLauncher") && !prevWeaponRL){
				secondaryMuzzleFlash.GetComponent<Weapon_RocketLauncher>().RotateForRocketLauncher(true);
				prevWeaponRL = true;
			}

			if (weapon3.name.Equals("Builder_FP_glove") && prevWeaponGlove){
				builderGloveMuzzle.GetComponent<Weapon_BuilderGlove>().RotateForGlove(false);
				prevWeaponGlove = false;
			}

		} else if (weapon == 3){

			Debug.Log("weapon 3");

			weapon1.SetActive(false);
			weapon2.SetActive(false);
			weapon3.SetActive(true);
			thirdPersonPistol.SetActive(false);
			thirdPersonSecondary.SetActive(false);
			thirdPersonBuilderGlove.SetActive(true);
			animInstance.anim.SetBool("Pistol",false);
			animInstance.anim.SetBool("SecondaryWeapon",false);
			animInstance.anim.SetBool("Glove",true);

			if (weapon3.name.Equals("Builder_FP_glove") && !prevWeaponGlove){
				builderGloveMuzzle.GetComponent<Weapon_BuilderGlove>().RotateForGlove(true);
				prevWeaponGlove = true;
			}
		}

		if (photonView.isMine)
			photonView.RPC ("NetworkChangeWeapons",PhotonTargets.OthersBuffered,vID,weapon);
	}

	void ChangeWeapons(){
		if (!gameObject.GetComponent<Char_BasicMoveScript>().respawning){
			if (Input.GetButtonDown("1")){
				NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,1);
				try {
					GetComponentInChildren<WeaponSniperScript>().sniperCrosshair.GetComponent<RawImage>().enabled = false;
					Camera.main.fieldOfView = 60;
				} catch (System.Exception e) {}
				// Change weapon
				HUD.SetWeaponIcon(current_class, 1);
			} else if (Input.GetButtonDown("2")){
				NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,2);
				// Change weapon
				HUD.SetWeaponIcon(current_class, 2);
			} else if (Input.GetButton("3")){
				if (weapon3.name.Equals("Builder_FP_glove")){
					NetworkChangeWeapons(transform.GetComponent<PhotonView>().viewID,3);
					// Change weapon
					HUD.SetWeaponIcon(current_class, 3);
				}
			}
		}
	}

	void LinkRegenEnergy(){
		if (energy < 100){
			energy ++;
		}
	}
	
	public void EnableKillHUD(string killedName){
		HUD.playerKilledLabel.CrossFadeAlpha(1,0.000001f,false);
		HUD.playerKilledLabel.text = "YOU JUST KILLED " + killedName;
		//Invoke("DisableKillHUD",2f);
		HUD.playerKilledLabel.CrossFadeAlpha(0,2,false);
		if (ScrollingMessageTimeout < 0){
			SendScrollingKillMessage(playerName + " just killed " + killedName, new Vector3(playerColor.r,playerColor.g,playerColor.b));
			ScrollingMessageTimeout = 1f;
		}
		UpdateScoreboardKills(playerName,killedName);
	} 

	// Update is called once per frame
	void Update () {
		ScrollingMessageTimeout -= Time.deltaTime;
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
					if(!healSource.isPlaying && energy!=100){
						healSource.Play();//OneShot(healEnergy);
					}
				}
			} else {
				healSource.Stop();
				linkRateCounter = enegryRegenRate;
			}
			HUD.UpdateHUDHealth(health);
			HUD.UpdateHUDEnergy(energy);
			ChangeWeapons();

			//Everything that needs to be done when a player dies
			if ((health <= 0 || Input.GetKey(KeyCode.P))&&respawnTimer == -10){
				try {
					deathSource.Play ();
				} catch {}
				respawnTimer = 8f;
				if (currentLink != null){
					ReduceCounter(currentLink.GetComponent<PhotonView>().viewID);
				}

				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				for (int i = 0; i < players.Length; i++){
					if (players[i].GetComponent<Char_AttributeScript>() != this &&
					    players[i].GetComponent<Char_AttributeScript>().team == team){
						followCamPos = players[i].transform;
						break;
					}
				}

				//Disabling various crosshairs
				try {
					GetComponentInChildren<Char_BasicShootScript>().hitCrosshair.GetComponent<RawImage>().enabled = false;
				} catch (System.NullReferenceException e){}
				try {
					GetComponentInChildren<Weapon_BuilderGlove>().buildCrosshair.GetComponent<RawImage>().enabled = false;
				} catch (System.NullReferenceException e){}

				gameObject.GetComponent<Char_BasicMoveScript>().inVortex=true;
				gameObject.GetComponent<Char_BasicMoveScript>().respawning=true;
				weapon1.SetActive(false);
				weapon2.SetActive(false);
				weapon3.SetActive(false);
				animInstance.anim.SetBool ("Dead", true);
				TurnOffColliders(GetComponent<PhotonView>().viewID);

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

			}

			//Count down respawn timer
			if (respawnTimer > 0){
				respawnTimer -= Time.deltaTime;
				HUD.respawnTimerText.gameObject.SetActive(true);

				//Follow an ally while we respawn
				if (followCamPos){
					Camera.main.transform.position = followCamPos.position + Vector3.right*5 + Vector3.up;
					Camera.main.transform.LookAt(followCamPos.position);
					HUD.playerNameLabel.text = followCamPos.GetComponent<Char_AttributeScript>().playerName;
				}

				int seconds = Mathf.RoundToInt(respawnTimer);
				if (seconds > 0)
					HUD.respawnTimerText.text = seconds.ToString();
			} else if (respawnTimer < 0 && respawnTimer != -10) {
				HUD.respawnTimerText.gameObject.SetActive(false);
				GameObject.Find("CharacterSelectionGUI").transform.localScale=new Vector3(10,5,5);
				HUD.spawnPreviewCamera.gameObject.SetActive(true);
				Camera.main.GetComponent<BlurEffect>().enabled=true;
				Screen.lockCursor=false;
				StartCoroutine("KillPlayerWait",this.gameObject.GetComponent<PhotonView>().viewID);
				respawnTimer = -1;

			}

			if (Input.GetKey(KeyCode.M)){
				Screen.lockCursor = true;
			}
		}
	}

	void playHurtSound(){
		Debug.Log ("Hurt sound ");
		int randomChance = Random.Range (1, 4);
		if(randomChance==1){
			int randomSound = Random.Range (1, 4);

			if (randomSound == 1) {
				//GameObject hurtSound = AudioSource.PlayClipAtPoint(pain1,transform.position) as GameObject;
			audio.clip = pain1;
				//audio.PlayOneShot(pain1);
				//hurtSound.transform.SetParent(transform);
			}
			else if(randomSound==2){
				//GameObject hurtSound = AudioSource.PlayClipAtPoint(pain2,transform.position) as GameObject;
				//hurtSound.transform.SetParent(transform);
			audio.clip=pain2;
		//		audio.PlayOneShot(pain2);
			}
			else if(randomSound==3){
			audio.clip=pain3;
				//GameObject hurtSound = AudioSource.PlayClipAtPoint(pain3,transform.position) as GameObject;
				//hurtSound.transform.SetParent(transform);
				//audio.PlayOneShot(pain3);
			}
		Debug.Log (audio.clip);
			audio.Play ();
		}
	}


	void KillCountTimeout(){
		prevKillsToUpdate = "";
		prevDeathsToUpdate = "";
	}

	[RPC] void TurnOffColliders(int vID){
		PhotonView.Find(vID).GetComponent<CapsuleCollider>().enabled = false;
		PhotonView.Find(vID).GetComponentInChildren<SphereCollider>().enabled = false;
		PhotonView.Find(vID).rigidbody.isKinematic = true;
		//PhotonView.Find(vID).transform.position = new Vector3(-1000,-1000,-1000);
		if (photonView.isMine)
			photonView.RPC("TurnOffColliders", PhotonTargets.OthersBuffered, vID);
	}

	string prevKillsToUpdate = "", prevDeathsToUpdate = "";
	[RPC] void UpdateScoreboardKills(string killsToUpdate, string deathsToUpdate){
		if (!(prevKillsToUpdate == prevKillsToUpdate && prevDeathsToUpdate == deathsToUpdate)) {
			prevKillsToUpdate = killsToUpdate;
			prevDeathsToUpdate = deathsToUpdate;
			Invoke("KillCountTimeout",1f);
			Level_NetworkController nc = HUD.NetworkController;
			for (int i = 0; i < nc.redPlayers.Count; i++){
				if (nc.redPlayers[i].name.Equals(killsToUpdate)){
					nc.redPlayers[i].increaseKills();
					Debug.Log("Kill update R " + killsToUpdate + " " + nc.redPlayers[i].kills);
				}
				if (nc.redPlayers[i].name.Equals(deathsToUpdate)){
					nc.redPlayers[i].increaseDeaths();
					Debug.Log("Kill death R " + deathsToUpdate + " " + nc.redPlayers[i].deaths);
				} 
			}
			for (int i = 0; i < nc.bluePlayers.Count; i++){
				if (nc.bluePlayers[i].name.Equals(killsToUpdate)){
					nc.bluePlayers[i].increaseKills();
					Debug.Log("Kill update B " + killsToUpdate + " " + nc.bluePlayers[i].kills);
				}
				if (nc.bluePlayers[i].name.Equals(deathsToUpdate)){
					nc.bluePlayers[i].increaseDeaths();
					Debug.Log("Kill death B " + deathsToUpdate + " " + nc.bluePlayers[i].deaths);
				}
			}
			HUD.SetUpScoreboard();

			if (photonView.isMine)
				photonView.RPC("UpdateScoreboardKills", PhotonTargets.OthersBuffered, killsToUpdate, deathsToUpdate);
		}
	}

	[RPC] void SendScrollingKillMessage(string message, Vector3 messageColor){
		Color c = new Color(messageColor.x, messageColor.y, messageColor.z);
		HUD.AddItemToScrollingList(message, c);
		if (photonView.isMine){
			photonView.RPC("SendScrollingKillMessage", PhotonTargets.OthersBuffered, message, messageColor);
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
		//Lower your team's score when you die
		if (PhotonView.Find(vID).gameObject.GetComponent<Char_AttributeScript>().team == Teams.BLUE){
			PhotonView.Find(vID).gameObject.GetComponent<Char_AttributeScript>().HUD.bluePoints.value -= 10;
		} else if (PhotonView.Find(vID).gameObject.GetComponent<Char_AttributeScript>().team == Teams.RED){
			PhotonView.Find(vID).gameObject.GetComponent<Char_AttributeScript>().HUD.redPoints.value -= 10;
		} 

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
		playHurtSound ();
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

	[RPC] void joinTeam(Vector3 color, int myTeam, string message)
	{
		renderer.material.color = new Color(color.x, color.y, color.z, 1f);
		armour.material.color = new Color(color.x, color.y, color.z, 1f);
		HUD = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();
		HUD.AddItemToScrollingList(message,  new Color(color.x, color.y, color.z, 1f));
		team = (Teams)myTeam;
		if (photonView.isMine)
			photonView.RPC("joinTeam", PhotonTargets.OthersBuffered, color, myTeam, message);
	}

	[RPC] void SetPlayerName(int vID, string pName){
		PhotonView.Find(vID).GetComponent<Char_AttributeScript>().playerName = pName;

		if (photonView.isMine)
			photonView.RPC("SetPlayerName", PhotonTargets.OthersBuffered, vID, pName);
	}
}
