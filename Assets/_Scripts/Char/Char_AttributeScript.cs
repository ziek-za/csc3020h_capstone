using UnityEngine;
using System.Collections;

public class Char_AttributeScript : MonoBehaviour {

	public int health = 100;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (health < 0)
			Destroy(gameObject);
	}
}
