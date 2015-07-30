using UnityEngine;
using System.Collections;

public class Level_MapController : MonoBehaviour {

	private GameObject Map;

	// Use this for initialization
	void Start () {
		Map = GameObject.Find ("Map");
		// Remove current LevelObjects
		Destroy(Map.transform.GetChild (0));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
