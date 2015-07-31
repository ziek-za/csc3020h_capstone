#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Map_Export))]
public class Map_Export_editor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector ();

		Map_Export me = (Map_Export)target;
		if (GUILayout.Button ("Save")) {
			me.Save();
		}
		if (GUILayout.Button ("Load")) {
			me.Load();
		}
	}
}
#endif
