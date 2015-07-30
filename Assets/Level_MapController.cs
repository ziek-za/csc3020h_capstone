using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Level_MapController : MonoBehaviour {

	private GameObject Map;
	private JSONNode level_objects;

	// Use this for initialization
	void Start () {
		// Load level data from relevant JSON file
		_MainController.ImportMapObject ("1");
		// check to see if it is loaded before progressing
		if (_MainController.ImportedMapObjectBool) {
			Map = GameObject.Find ("Map");
			// Remove current LevelObjects
			Destroy (Map.transform.FindChild("LevelObjects").gameObject);
			// Get level objects
			//Debug.Log (_MainController.MapObject["level_objects"]);
			level_objects = _MainController.MapObject["level_objects"][0];
			RecurseChildren (level_objects, Map);
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
		}
		if (spawned_prefab.rigidbody != null) {
			spawned_prefab.rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
			StartCoroutine(ResetRigidbody(spawned_prefab));
		}
		spawned_prefab.gameObject.transform.SetParent(parent_go.transform);

		if (jn["children"].Count > 0) {
			// Contains children
			for (int i = 0; i < jn["children"].Count; i++) {
				RecurseChildren (jn["children"][i], spawned_prefab);
			}
		}

		if (spawned_prefab.rigidbody != null) {
			spawned_prefab.rigidbody.constraints = RigidbodyConstraints.None;
		}
		
	}

	IEnumerator ResetRigidbody(GameObject gm){
		yield return new WaitForSeconds(1f);
		gm.rigidbody.constraints = RigidbodyConstraints.None;
	}

}
