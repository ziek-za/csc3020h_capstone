using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Level_MapController : MonoBehaviour {

	private GameObject Map;
	private JSONNode level_objects;
	private Terrain terrain;

	// Use this for initialization
	void Start () {
		SetupLevel ("1");
	}

	public void SetupLevel(string level) {
		terrain = Terrain.activeTerrain;
		// Load level data from relevant JSON file
		_MainController.ImportMapObject (level);
		// check to see if it is loaded before progressing
		if (_MainController.ImportedMapObjectBool) {
			Map_TerrainController tc = terrain.GetComponent<Map_TerrainController>();
			tc.SetTerrainHeightMap();
			Map = GameObject.Find ("Map");
			// Remove current LevelObjects
			GameObject levelObjects = Map.transform.FindChild("LevelObjects").gameObject;
			levelObjects.SetActive(false);
			Destroy (levelObjects);
			// Get level objects
			//Debug.Log (_MainController.MapObject["level_objects"]);
			level_objects = _MainController.MapObject["level_objects"][0];
			RecurseChildren (level_objects, Map);
		} else {
			Debug.Log ("Unable to initiate level creation, file could not be loaded");
		}
	}

	private void RecurseChildren(JSONNode jn, GameObject parent_go) {
		GameObject spawned_prefab;
		// Set orientation vectors
		Vector3 pos = new Vector3(jn["position"]["x"].AsFloat,
		                          jn["position"]["y"].AsFloat,
		                          jn["position"]["z"].AsFloat);
		Quaternion rotation = Quaternion.EulerAngles(jn["rotation"]["x"].AsFloat,
		                               jn["rotation"]["y"].AsFloat,
		                               jn["rotation"]["z"].AsFloat);
		// Load resource object and instantiate
		Debug.Log (jn ["prefab"]);
		if (jn ["prefab"].Value == "LevelObjects") {
			spawned_prefab = new GameObject();//Instantiate(pos, rotation);
			spawned_prefab.name = "LevelObjects";
			spawned_prefab.transform.position = pos;
			spawned_prefab.transform.rotation = rotation;
		} else {
			GameObject prefab = Resources.Load ("_Prefabs/" + jn ["prefab"]) as GameObject;
			spawned_prefab = Instantiate(prefab, pos, rotation) as GameObject;
			spawned_prefab.name = jn["prefab"];
		}

		spawned_prefab.gameObject.transform.parent = parent_go.transform;//SetParent(parent_go.transform);

		if (jn["children"].Count > 0) {
			// Contains children
			for (int i = 0; i < jn["children"].Count; i++) {
				RecurseChildren (jn["children"][i], spawned_prefab);
			}
		}
		
	}

	IEnumerator ResetRigidbody(GameObject gm){
		yield return new WaitForSeconds(1f);
		gm.rigidbody.constraints = RigidbodyConstraints.None;
	}

}
