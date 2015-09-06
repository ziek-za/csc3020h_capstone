using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map_DestructableObject : MonoBehaviour {
	public int health = 100;
	// Destructable objects to be added from LEAST destroyed to MOST
	public List<GameObject> objects;
	public List<int> thresholds;
	// Initialised to the current gameObject
	//private int currentIndex = 0;
	public ParticleSystem particleSystem;

	// Used when hit by a force
	public void Hit(int damage) {
		// Subtract from health
		health -= damage;
		if (thresholds.Count != 0) {
			Debug.Log ("HIT_2");
			// switch to destructable game object based on threshold
			for (int i = thresholds.Count - 1; i > -1; i--) {
				// Find the first threshold it is below
				Debug.Log (health);
				if (health < thresholds[i]) {
					if (objects.Count - 1 > i) {
						//switch out the mesh for the desired threshold if it exists
						Mesh meshInstance = Instantiate(objects[i].GetComponent<MeshFilter>()) as Mesh;
						GetComponent<MeshFilter>().mesh = meshInstance;
					} else {
						//there is no mesh, destroy the object
						gameObject.SetActive(false);
					}
					// Play particle effect
					/*if (!particleSystem.isPlaying) {
						particleSystem.Play();		
					}*/
					break;
				}
			}
		}
	}
}
