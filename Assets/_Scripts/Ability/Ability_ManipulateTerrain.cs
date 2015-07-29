using UnityEngine;
using System.Collections;

public class Ability_ManipulateTerrain : Photon.MonoBehaviour {

	private Terrain terrain;
	private Map_TerrainController MTC;

	// Use this for initialization
	void Start () {
		terrain = Terrain.activeTerrain;
		MTC = terrain.GetComponent<Map_TerrainController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			if (Input.GetButtonDown("Fire1")) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				
				if (Physics.Raycast (ray, out hit)) {
					SyncTerrain (hit.point);
				}
			}
		}


	}


	[RPC] void SyncTerrain(Vector3 explosion_pos){
		
		MTC.ManipulateTerrain(explosion_pos, 5f, "pull");
		if (photonView.isMine) {
			photonView.RPC("SyncTerrain",PhotonTargets.OthersBuffered, explosion_pos);
		}
	}
}
