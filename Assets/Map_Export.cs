#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.IO;
using UnityEditor;
public class Map_Export : MonoBehaviour {

	public string LevelID = "";
	public string RawHeightMapID = "";
	public string FreezeMapID = "";
	public string ClampMapID = "";
	public string LevelName = "";

	private string defaultPath = "_LevelData/";
	private string defaultJSON = "Base";
	private string defaultSavePath = "Assets/Resources/";
	private string RawHeightMapSize = "257";
	private string FreezeMapSize = "257";
	private string ClampMapSize = "257";
	private Terrain terrain;

	// Load level objects
	public void Load() {
		// Load level data from relevant JSON file
		_MainController.ImportMapObject (LevelID);
		// check to see if it is loaded before progressing
		if (_MainController.ImportedMapObjectBool) {
			// Set terrain map
			terrain = Terrain.activeTerrain;
			Map_TerrainController tc = terrain.GetComponent<Map_TerrainController> ();
			tc.SetTerrainHeightMap ();
			// Load objects
			GameObject mc = GameObject.Find("Map Controller");
			Level_MapController mc_script = mc.GetComponent<Level_MapController>();
			mc_script.SetLevelObjects(true);
		} else {
			Debug.Log ("-- INVALID LEVEL IMPORT: "  + LevelID + " --");
		}
	}
	// Main method to export/save all objects
	public void Save() {
		// Load file if possibles
		try {
			if (LevelName.Length == 0 || LevelID.Length == 0 || RawHeightMapID.Length == 0) {
				throw new System.ArgumentException("LevelID and LevelName required");
			}
			TextAsset ta = Resources.Load (defaultPath + defaultJSON) as TextAsset;
			JSONNode jn = JSON.Parse (ta.text);
			jn["name"] = LevelName;
			jn["version"] = "1";
			jn["levelData"]["name"] = LevelID;
			// Terrain height map
			jn["terrainRaw"]["name"] = RawHeightMapID;
			jn["terrainRaw"]["size"] = RawHeightMapSize;
			// Terrain freeze map
			jn["terrainFreeze"]["name"] = FreezeMapID;
			jn["terrainFreeze"]["size"] = FreezeMapSize;
			// Terrain clamp map
			jn["terrainClamp"]["name"] = ClampMapID;
			jn["terrainClamp"]["size"] = ClampMapSize;
			// Generate children
			JSONArray ja = new JSONArray();
			ja = RecurseChildren(gameObject, ja);
			Debug.Log ("**Recursed Objects**");
			jn["level_objects"] = ja;
			// Save
			StreamWriter sr = File.CreateText(defaultSavePath + defaultPath + LevelID + ".json");
			sr.Write(jn.ToString());
			sr.Close();
			Debug.Log ("**Saved**");
		} catch (System.Exception e) {
			Debug.LogException (e);
		}
	}

	// Used to recurse and save all the level_objects
	private JSONArray RecurseChildren (GameObject curr_go, JSONArray ja) {
		bool ignore = false;
		JSONNode jn = JSON.Parse ("{}");
		jn ["isPrefab"] = "false";
		//Debug.Log (curr_go.name + " " + PrefabUtility.FindPrefabRoot (curr_go) + " " + PrefabUtility.GetPrefabParent (curr_go) + " " + PrefabUtility.GetPrefabObject(curr_go));// + " " + PrefabUtility.GetPrefabParent(curr_go));
		if (PrefabUtility.GetPrefabObject(curr_go) != null && curr_go.name == PrefabUtility.FindPrefabRoot (curr_go).name) {
			jn ["isPrefab"] = "true";
			jn ["prefab"] = curr_go.name;
		} else {
			if (curr_go.transform.parent.gameObject.name == "Map") {
				// Root LevelObjects object
				jn["name"] = "LevelObjects";
			} else {
				ignore = true;
				//jn["name"] = curr_go.name;
			}
		}
		if (!ignore) {
			// Position
			jn ["position"] ["x"] = curr_go.transform.position.x.ToString();
			jn ["position"] ["y"] = curr_go.transform.position.y.ToString();
			jn ["position"] ["z"] = curr_go.transform.position.z.ToString();
			// Rotation
			jn ["rotation"] ["x"] = curr_go.transform.rotation.eulerAngles.x.ToString();
			jn ["rotation"] ["y"] = curr_go.transform.rotation.eulerAngles.y.ToString();
			jn ["rotation"] ["z"] = curr_go.transform.rotation.eulerAngles.z.ToString();
			if (curr_go.transform.childCount > 0) {
				JSONArray children = new JSONArray();
				for (int i = 0; i < curr_go.transform.childCount; i++) {
					jn["children"] = RecurseChildren(curr_go.transform.GetChild (i).gameObject, children);
				}
				ja["-1"] = jn;
				return ja;
			} else {
				ja["-1"] = jn;
				return ja;
			}
		} else {
			return ja;
		}
	}
}
#endif
