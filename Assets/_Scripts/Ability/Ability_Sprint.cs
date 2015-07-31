using UnityEngine;
using System.Collections;

public class Ability_Sprint : Photon.MonoBehaviour {
	public float cooldown = 5;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			if(Input.GetButtonDown("Sprint") && 
			   !transform.GetComponent<Char_AttributeScript> ().buffs.Contains ("sprint")){
				transform.GetComponent<Char_AttributeScript>().speed += 50;
				transform.GetComponent<Char_AttributeScript>().buffs.Add("sprint");
				Invoke ("Debuff",cooldown);
			}
		}
	}

	void Debuff(){
		if (transform.GetComponent<Char_AttributeScript> ().buffs.Contains ("sprint")) {
			transform.GetComponent<Char_AttributeScript>().buffs.Remove("sprint");
			transform.GetComponent<Char_AttributeScript>().speed-=50;
				}
		}
}
