using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour {
	AudioSource audio;
	bool play=true;
	bool sound1=false;
	public AudioClip track1;
	public AudioClip track2;
	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (play) {
			play=false;
			generateNextTrack();
			audio.Play ();
			StartCoroutine(waitSongTime());
		}
	}

	void generateNextTrack(){
		if (!sound1) {
			audio.clip=track2;
			sound1= true;
		}else{
			audio.clip=track1;
			sound1=false;
		}
	}

	IEnumerator waitSongTime(){
		yield return new WaitForSeconds(audio.clip.length);
	}


}
