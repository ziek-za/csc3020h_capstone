using UnityEngine;
using System.Collections;

public class Ability_Sprint : Photon.MonoBehaviour {
	public int energyCost = 1;
	public AudioSource thiefSource;
	public AudioClip sprintSound;
	
	bool startedSprint = false;
	ParticleSystem[] sprintEffects;

	// Use this for initialization
	void Start () {
		try {
			sprintEffects = GetComponentsInChildren<ParticleSystem>();
		} catch (System.Exception e){}
	}

	void FixedUpdate() {
		if(photonView.isMine){
			if (Input.GetButton("Ability 2") && transform.GetComponent<Char_AttributeScript>().energy >= energyCost){
				transform.GetComponent<Char_AttributeScript>().energy -= energyCost;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			if (Input.GetButton("Ability 2") && transform.GetComponent<Char_AttributeScript>().energy >= energyCost){
				if(!transform.GetComponent<Char_AttributeScript> ().buffs.Contains ("sprint")){
					transform.GetComponent<Char_BasicMoveScript>().moveSpeed += 20;
					transform.GetComponent<Char_AttributeScript>().buffs.Add("sprint");
					startedSprint = true;
					thiefSource.PlayOneShot(sprintSound);
					if (sprintEffects != null){
						SprintEffect(true);
					}
				}
			} else if (startedSprint){
				thiefSource.Stop ();
				startedSprint = false;
				Debuff ();
				if (sprintEffects != null){
					SprintEffect(false);
				}
			}
		}
	}

	[RPC] void SprintEffect(bool start){
		if (start){
			for (int i = 0; i < sprintEffects.Length; i++){
				if (sprintEffects[i].CompareTag("Sprint")){
					sprintEffects[i].Play();
				}
			}
		} else {
			for (int i = 0; i < sprintEffects.Length; i++){
				if (sprintEffects[i].CompareTag("Sprint")){
					sprintEffects[i].Stop();
				}
			}
		}	
		if (photonView.isMine)
			photonView.RPC("SprintEffect",PhotonTargets.OthersBuffered, start);
	}

	void Debuff(){
		if (transform.GetComponent<Char_AttributeScript> ().buffs.Contains ("sprint")) {
			transform.GetComponent<Char_AttributeScript>().buffs.Remove("sprint");
			transform.GetComponent<Char_BasicMoveScript>().moveSpeed-=20;
		}
	}
}
