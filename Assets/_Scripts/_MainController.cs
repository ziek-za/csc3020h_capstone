﻿using UnityEngine;
using System.Collections;
using SimpleJSON;

public class _MainController {

	public static bool RoomJoined;
	public static JSONNode StringLookup;
	public static bool ImportedStringLookupBool = false;

	public static string ExampleLookupMethod(){
		return "some info";
	}

	public static void CreateServer(){
		Application.LoadLevel("Level");
		if (PhotonNetwork.room == null){
			PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString("N"), true, true, 5);
			Map_TerrainController.hmReset = false;
		}
	}

	public static void JoinServer(string serverName){
		Application.LoadLevel("Level");
		PhotonNetwork.JoinRoom(serverName);
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
