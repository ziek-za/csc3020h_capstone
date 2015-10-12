using UnityEngine;
using System.Collections;

public class Ambience : MonoBehaviour {
	public AudioClip[] ambientSounds;
	AudioSource thisSource;
	bool play=true;
	// Use this for initialization
	void Start () {
		thisSource = transform.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (thisSource.clip + " " + thisSource.isPlaying +" "+thisSource.clip.length);
		if (play) {
			play=false;
			generateAmbientSound();
			thisSource.PlayOneShot (thisSource.clip);
			StartCoroutine(waitRandomTime());
		}
	}

	void generateAmbientSound(){
		audio.clip = ambientSounds[Random.Range(0, ambientSounds.Length)];
	}

	IEnumerator waitRandomTime(){
		yield return new WaitForSeconds(thisSource.clip.length+Random.Range(2,5));
		play = true;
	}
}
