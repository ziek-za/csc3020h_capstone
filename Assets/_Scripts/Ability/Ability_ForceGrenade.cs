using UnityEngine;
using System.Collections;

public class Ability_ForceGrenade: Photon.MonoBehaviour {

	private Terrain terrain;
	private Map_TerrainController MTC;

	public Transform grenadePrefab;
	public Transform grenadePosition;
	public int energyCost = 30;
	public int cooldown = 4;

	GameObject cameraDirection;

	public float fuseTime = 3f;
	public float initialForwardVelocity = 15f;
	public float initialUpwardsVelocity = 7f;
	string mode = "push";
	bool offCooldown = true;

	// Use this for initialization
	void Start () {
		terrain = Terrain.activeTerrain;
		MTC = terrain.GetComponent<Map_TerrainController>();
		cameraDirection = transform.FindChild("CameraPosition").gameObject; //Used so grenade is always thrown forwards
	}

	void cooledDown(){
		offCooldown = true;
	}

	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			if (Input.GetButtonDown("Grenade") && transform.GetComponent<Char_AttributeScript>().energy >= energyCost && offCooldown) {
				//Uses energy
				transform.GetComponent<Char_AttributeScript>().energy -= energyCost;
				//Start cooldown timer
				Invoke("cooledDown",cooldown);
				offCooldown = false;
				//Creates the actual grenade over the photon network
				GameObject gr = PhotonNetwork.Instantiate(grenadePrefab.name, grenadePosition.position, Quaternion.identity, 0) as GameObject;
				gr.rigidbody.velocity = (cameraDirection.transform.forward * initialForwardVelocity) + 
										(cameraDirection.transform.up * initialUpwardsVelocity);
				StartCoroutine(Explode(gr)); //Starts the Coroutine which will cause the grenade to explode after fuseTime seconds
			}

			//Changing modes
			if (Input.GetButtonDown("1")){
				mode = "push";
			}
			if (Input.GetButtonDown("2")){
				mode = "pull";
			}

		}				               				              
	}

	//THIS 'explode' method calls the Weapon_ForceGrenade 'explode' method on the actual grenade after the fuse time
	//BUT if a PUSH is applied, this method also modifies the terrain because this script is aware of the terrain
	//while the Weapon_ForceGrenade script is not.
	IEnumerator Explode(GameObject grenade){
		yield return new WaitForSeconds(fuseTime);
		if (mode.Equals("push"))
			PushTerrain(grenade.transform.position);
		grenade.GetComponent<Weapon_ForceGrenade>().Explode(mode);
	}

	//Used to modify the terrain on PUSH only
	[RPC] void PushTerrain(Vector3 explosion_pos){		
		//2 1.1
		MTC.ManipulateTerrain(explosion_pos, 5f, "push", 30f, 2f, 1.1f);
		if (photonView.isMine) {
			photonView.RPC("PushTerrain",PhotonTargets.OthersBuffered, explosion_pos);
		}
	}
}
