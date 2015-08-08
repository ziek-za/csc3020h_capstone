using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Level_MapController : MonoBehaviour {

	private GameObject Map;
	private JSONNode level_objects;
	private bool IsUnityEditorBool;

	// Use this for initialization
	void Start () {
	}

	public void SetLevelObjects(bool IsUnityEditorBool) {
		// Load level data from relevant JSON file
		//_MainController.ImportMapObject (level);
		// check to see if it is loaded before progressing
		if (_MainController.ImportedMapObjectBool) {
			//Map_TerrainController tc = terrain.GetComponent<Map_TerrainController>();
			//tc.SetTerrainHeightMap();
			Map = GameObject.Find ("Map");
			// Remove current LevelObjects
			GameObject levelObjects = Map.transform.FindChild("LevelObjects").gameObject;
			levelObjects.SetActive(false);
			// Get level objects
			level_objects = _MainController.MapObject["level_objects"][0];
			this.IsUnityEditorBool = IsUnityEditorBool;		
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
		Quaternion rotation = Quaternion.Euler(jn["rotation"]["x"].AsFloat,
		                               jn["rotation"]["y"].AsFloat,
		                               jn["rotation"]["z"].AsFloat);
		// Load resource object and instantiate
		if (jn ["isPrefab"].AsBool == false) {
			spawned_prefab = new GameObject();
			if (jn["name"].Value == "LevelObjects") {
				spawned_prefab.name = "LevelObjects - (ID: " + _MainController.MapObject["levelData"]["name"] + ") " +
					_MainController.MapObject["name"] + " v" +_MainController.MapObject["version"];
					//#if UNITY_EDITOR
					// add scripts and values from script
					spawned_prefab.AddComponent<Map_Export>();
					Map_Export me = spawned_prefab.GetComponent<Map_Export>();
					me.LevelName = _MainController.MapObject["name"];
					me.LevelID = _MainController.MapObject["levelData"]["name"];
					me.RawHeightMapID = _MainController.MapObject["terrainRaw"]["name"];
					me.FreezeMapID = _MainController.MapObject["terrainFreeze"]["name"];
					me.ClampMapID = _MainController.MapObject["terrainClamp"]["name"];
					//#endif 

			} else {
				spawned_prefab.name = jn["name"];
			}
			spawned_prefab.transform.position = pos;
			spawned_prefab.transform.rotation = rotation;
		} else {
			//Debug.Log ("[Instantiating prefab: " + jn ["prefab"] + "]");
			GameObject prefab = Resources.Load (jn ["prefab"]) as GameObject;
			if (IsUnityEditorBool) {
				spawned_prefab = Instantiate(prefab, pos, rotation) as GameObject;
			} else {
				spawned_prefab = PhotonNetwork.Instantiate(prefab.name, pos, rotation,0) as GameObject;
			}
			spawned_prefab.name = jn["prefab"];
		}

		spawned_prefab.gameObject.transform.parent = parent_go.transform;

		if (jn["children"].Count > 0) {
			// Contains children
			for (int i = 0; i < jn["children"].Count; i++) {
				RecurseChildren (jn["children"][i], spawned_prefab);
			}
		}
		
	}
}
