using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map_DestructableObject : MonoBehaviour {
	public int health = 100;
	// Destructable objects to be added from LEAST destroyed to MOST
	// (size of threshold list dictates how much it can be destroyed)
	public List<int> thresholds;
	public bool HasThresholdMeshesBool = false;
	public bool HasParticleSystem = false;
	public bool DestroyFinal = false;
	// Initialised to the current gameObject
	private int currentIndex = -1;
	public ParticleSystem particleSystem;
	public AudioClip explosion;
	//public AudioClip explode_tile;
	//public AudioClip explode_wall;


	void Start() {
		// Add the default value to the end
		thresholds.Add (0);
	}

	// Used when hit by a force
	public void Hit(int damage) {
		// Subtract from health
		if (thresholds.Count != 0 && health >= 0) {
			health += damage;
			// switch to destructable game object based on threshold
			for (int i = thresholds.Count - 1; i > -1; i--) {
				// Find the first threshold it is below
				if (health <= thresholds[i]) {
					if (HasThresholdMeshesBool) {
						int k = i;
						if (HasParticleSystem) { k++; }
						if (i == thresholds.Count - 1 && DestroyFinal) {
							this.gameObject.transform.GetChild(k).gameObject.SetActive(false);
						} else {
							Debug.Log("k: " + k + " " + this.gameObject.transform.GetChild(k).gameObject.name);
							// Set the next 'destroyed object' to false in the children list
							this.gameObject.transform.GetChild(k).gameObject.SetActive(false);
							// Set the previous game object to false
							this.gameObject.transform.GetChild(k + 1).gameObject.SetActive(true);
						}
					}
					// If there is no mesh which is set as the final form and
					// the index of the threshold is at the last one,
					// then set the entire object to inactive.
					else if (thresholds.Count == 1) {
						//there is no mesh, set the entire object to false
						this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
					}
					// Play particle effect
					if (HasParticleSystem) {
						if (!particleSystem.isPlaying) {
							particleSystem.Clear ();
						}
						if (currentIndex != i) { particleSystem.Play (); 
							Debug.Log(particleSystem.name);
							//if(particleSystem.name=="wall_explosion"){
								//AudioSource.PlayClipAtPoint(explode_wall,transform.position);
							//	Debug.Log ("Wall explosion play");
							//}else if(particleSystem.name=="tile_explosion"){
								//AudioSource.PlayClipAtPoint(explode_tile,transform.position);
							//	Debug.Log ("Wall explosion play");
							//}
							AudioSource.PlayClipAtPoint(explosion,transform.position);
							//AudioSource.PlayClipAtPoint();
						}
					}
					currentIndex = i;
					break;
				}
			}
		}
	}
}
