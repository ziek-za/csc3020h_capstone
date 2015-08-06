using UnityEngine;
using System.Collections;

public class Ability_Sprint : Photon.MonoBehaviour {
	public float duration = 5;
	public int energyCost = 10;
	public int cooldown = 10;
	
	bool offCooldown = true;

	// Use this for initialization
	void Start () {
	
	}

	void cooledDown(){
		offCooldown = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
			if(Input.GetButtonDown("Sprint") && 
			   !transform.GetComponent<Char_AttributeScript> ().buffs.Contains ("sprint") &&
				transform.GetComponent<Char_AttributeScript>().energy >= energyCost &&
			  	offCooldown)
			{
					transform.GetComponent<Char_AttributeScript>().energy -= energyCost;
					transform.GetComponent<Char_AttributeScript>().speed += 50;
					transform.GetComponent<Char_AttributeScript>().buffs.Add("sprint");
					Invoke ("Debuff",duration);
					Invoke("cooledDown",cooldown);
					offCooldown = false;
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
