using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map_DestructableObject : MonoBehaviour {
	public int health = 100;
	// Destructable objects to be added from LEAST destroyed to MOST
	public List<GameObject> destructableObjsList;
	// Initialised to the current gameObject
	//public int currentIndex = -1;
	public ParticleSystem particleSystem;

	// Used when hit by a force
	public void Hit(int damage) {
		// Subtract from health
		health -= damage;
		if (destructableObjsList.Count != 0) {
			// switch to destructable game object based on threshold
			for (int i = destructableObjsList.Count - 1; i > -1; i--) {
				_DestructableObject doInstance = destructableObjsList[i].getComponent<_DestructableObject> as _DestructableObject;
				// Starting from the bottom up
				if (health < doInstance.threshold) {
					Mesh meshInstance = Instantiate(doInstance.mesh) as Mesh;
					GetComponent<MeshFilter>().mesh = meshInstance;
					// Play particle effect
					if (!particleSystem.isPlaying) {
						particleSystem.Play();		
					}
					break;
				}
			}
		}
	}
}
