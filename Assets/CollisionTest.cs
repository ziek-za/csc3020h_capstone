using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour {

	void OnCollisionEnter(Collision other) {
		Debug.Log (other.gameObject.name);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
