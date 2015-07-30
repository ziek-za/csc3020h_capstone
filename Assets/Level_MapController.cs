using UnityEngine;
using System.Collections;

public class Level_MapController : MonoBehaviour {

	private GameObject Map;

	// Use this for initialization
	void Start () {
		// Load level data from relevant JSON file
		_MainController.ImportMapObject ("1");
		// check to see if it is loaded before progressing
		if (_MainController.ImportedMapObjectBool) {
			Map = GameObject.Find ("Map");
			// Remove current LevelObjects
			Destroy (Map.transform.GetChild (0).gameObject);
			// Get level objects
			//_MainController.MapObject["level_objects"][0];
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
