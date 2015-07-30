using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.IO;

public class Map_Export : MonoBehaviour {

	public string LevelID = "";
	public string LevelName = "";

	private string defaultPath = "_LevelData/";
	private string defaultJSON = "Base";
	private string defaultSavePath = "Assets/Resources/";

	// Main method to export/save all objects
	public void Save() {
		// Load file if possibles
		try {
			if (LevelName.Length == 0 || LevelID.Length == 0) {
				throw new System.ArgumentException("LevelID and LevelName required");
			}
			TextAsset ta = Resources.Load (defaultPath + defaultJSON) as TextAsset;
			JSONNode jn = JSON.Parse (ta.text);
			jn["name"] = LevelName;
			jn["version"] = "1";
			jn["levelData"]["name"] = LevelID;
			// Generate children
			JSONArray ja = new JSONArray();
			ja = RecurseChildren(gameObject, ja);
			jn["level_objects"] = ja;
			// Save
			StreamWriter sr = File.CreateText(defaultSavePath + defaultPath + LevelID + ".json");
			sr.Write(jn.ToString());
			sr.Close();
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
		jn ["rotation"] ["x"] = curr_go.transform.rotation.x.ToString();
		jn ["rotation"] ["y"] = curr_go.transform.rotation.y.ToString();
		jn ["rotation"] ["z"] = curr_go.transform.rotation.z.ToString();
		if (curr_go.transform.childCount > 0) {
			JSONArray children = new JSONArray();
			for (int i = 0; i < curr_go.transform.childCount; i++) {
				ja["-1"] = JSON.Parse(RecurseChildren(curr_go.transform.GetChild (i).gameObject, children).ToString());
				return ja;
			}
			return ja;
		} else {
			ja["-1"] = jn;
			return ja;
		}
	}
}
