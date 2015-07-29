using UnityEngine;
using System.Collections;

public class Ability_ForceGrenade: Photon.MonoBehaviour {

	private Terrain terrain;
	private Map_TerrainController MTC;

	public Transform grenadePrefab;
	public Transform grenadePosition;

	public float fuseTime = 3f;
	public string mode = "push";

	// Use this for initialization
	void Start () {
		terrain = Terrain.activeTerrain;
		MTC = terrain.GetComponent<Map_TerrainController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			if (Input.GetButtonDown("Grenade")) {
				GameObject gr = PhotonNetwork.Instantiate(grenadePrefab.name, grenadePosition.position, Quaternion.identity, 0) as GameObject;
				gr.rigidbody.velocity = (transform.forward * 15) + (transform.up * 7);
				StartCoroutine(Explode(gr));
				//Invoke("Explode",fuseTime);
			}
		}				               				              
	}

	IEnumerator Explode(GameObject gr){
		yield return new WaitForSeconds(fuseTime);
		SyncTerrain(gr.transform.position);
		gr.GetComponent<Weapon_ForceGrenade>().Explode(mode);
		//gr.transform.GetComponent<SphereCollider>().isTrigger = true;
		//Destroy(gr);
	}


	[RPC] void SyncTerrain(Vector3 explosion_pos){		
		MTC.ManipulateTerrain(explosion_pos, 5f, mode);
		if (photonView.isMine) {
			photonView.RPC("SyncTerrain",PhotonTargets.OthersBuffered, explosion_pos);
		}
	}
}
