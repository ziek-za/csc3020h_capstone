using UnityEngine;
using System.Collections;
using SimpleJSON;

public class _MainController {

	public static bool RoomJoined;
	public static JSONNode StringLookup;
	public static JSONNode MapObject;
	public static bool ImportedMapObjectBool = false;
	public static bool ImportedStringLookupBool = false;

	public static string ExampleLookupMethod(){
		return "some info";
	}

	public static void CreateServer(){
		Application.LoadLevel("Level");
		ImportMapObject("2");
		if (PhotonNetwork.room == null){
			PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString("N"), true, true, 5);
			Level_NetworkController.firstPlayer = true;
		}
	}

	/*
	public static void JoinServer(string serverName){
		Application.LoadLevel("Level");
		ImportMapObject("1");
		PhotonNetwork.JoinRoom(serverName);
	}*/

	public static void ImportMapObject(string level) {
		// Load in the file from resources directory
		Debug.Log ("[Attempting to import map object]");
		try {
			string filename = "_LevelData/" + level;
			TextAsset file = Resources.Load (filename) as TextAsset;
			if (file == null) { 
				ImportedMapObjectBool = false;
			} else {
				MapObject = JSON.Parse (file.text);
				if (MapObject != null) {
					// If successfully loaded file
					ImportedMapObjectBool = true;
					Debug.Log ("[Successfully loaded map object]");
				} else {
					throw new System.ArgumentException("Unable to parse JSON file: " + filename);
				}
			}
		} catch (System.Exception e) {
			Debug.LogException(e);
		}
	}

	public static void ImportStringLookup() {
		// Load in the file from resources directory
		try {
			string filename = "StringLookup";
			TextAsset file = Resources.Load (filename) as TextAsset;
			StringLookup = JSON.Parse (file.text);
			if (StringLookup != null) {
				// If successfully loaded file
				ImportedStringLookupBool = true;
			} else {
				throw new System.ArgumentException("Unable to load file: " + filename);
			}
		} catch (System.Exception e) {
			Debug.LogException(e);
		}
	}

}
