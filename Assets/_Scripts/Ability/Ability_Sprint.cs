using UnityEngine;
using System.Collections;

public class Ability_Sprint : Photon.MonoBehaviour {
	public int energyCost = 1;
	
	bool startedSprint = false;

	// Use this for initialization
	void Start () {
	
	}
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			if (Input.GetButton("Ability 2") && transform.GetComponent<Char_AttributeScript>().energy >= energyCost){
				transform.GetComponent<Char_AttributeScript>().energy -= energyCost;
				if(!transform.GetComponent<Char_AttributeScript> ().buffs.Contains ("sprint")){
					transform.GetComponent<Char_BasicMoveScript>().moveSpeed += 20;
					transform.GetComponent<Char_AttributeScript>().buffs.Add("sprint");
					startedSprint = true;
				}
			} else if (startedSprint){
				startedSprint = false;
				Debuff ();
			}
		}
	}

	void Debuff(){
		if (transform.GetComponent<Char_AttributeScript> ().buffs.Contains ("sprint")) {
			transform.GetComponent<Char_AttributeScript>().buffs.Remove("sprint");
			transform.GetComponent<Char_BasicMoveScript>().moveSpeed-=20;
		}
	}
}
