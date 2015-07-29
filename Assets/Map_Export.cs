using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.IO;

public class Map_Export : MonoBehaviour {

	public string LevelID;
	public string LevelName;

	private string defaultPath = "_LevelData/";
	private string defaultJSON = "Base";
	private string defaultSavePath = "Assets/Resources/";

	// Main method to export/save all objects
	public void Save() {
		// Load file if possible
		try {
			if (LevelName.Length == 0 || LevelID.Length == 0) {
				throw new System.ArgumentException("LevelID and LevelName required");
			}
			TextAsset ta = Resources.Load (defaultPath + defaultJSON) as TextAsset;
			JSONNode jn = JSON.Parse (ta.text);
			jn["name"] = LevelName;
			jn["version"] = "1";
			jn["levelData"]["name"] = LevelID;
			StreamWriter sr = File.CreateText(defaultSavePath + defaultPath + LevelID + ".json");
			sr.Write(jn.ToString());
			sr.Close();
		} catch (System.Exception e) {
			Debug.LogException (e);
		}
	} 
}
