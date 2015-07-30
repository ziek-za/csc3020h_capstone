using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.IO;

public class Map_Export : MonoBehaviour {

	public string LevelID = "";
	public string RawID = "";
	public string LevelName = "";

	private string defaultPath = "_LevelData/";
	private string defaultJSON = "Base";
	private string defaultSavePath = "Assets/Resources/";

	// Main method to export/save all objects
	public void Save() {
		// Load file if possibles
		try {
			if (LevelName.Length == 0 || LevelID.Length == 0 || RawID.Length == 0) {
				throw new System.ArgumentException("LevelID and LevelName required");
			}
			TextAsset ta = Resources.Load (defaultPath + defaultJSON) as TextAsset;
			JSONNode jn = JSON.Parse (ta.text);
			jn["name"] = LevelName;
			jn["version"] = "1";
			jn["levelData"]["name"] = LevelID;
			jn["terrainRaw"]["name"] = RawID;
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

	private JSONArray RecurseChildren (GameObject curr_go, JSONArray ja) {
		JSONNode jn = JSON.Parse ("{}");
		jn ["prefab"] = curr_go.name;
		// Position
		jn ["position"] ["x"] = curr_go.transform.position.x.ToString();
		jn ["position"] ["y"] = curr_go.transform.position.y.ToString();
		jn ["position"] ["z"] = curr_go.transform.position.z.ToString();
		// Rotation
		jn ["rotation"] ["x"] = curr_go.transform.localRotation.x.ToString();
		jn ["rotation"] ["y"] = curr_go.transform.localRotation.y.ToString();
		jn ["rotation"] ["z"] = curr_go.transform.localRotation.z.ToString();
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
	}
}
