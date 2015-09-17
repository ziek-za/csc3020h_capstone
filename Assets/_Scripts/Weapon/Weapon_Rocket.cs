using UnityEngine;
using System.Collections;

public class Weapon_Rocket : Photon.MonoBehaviour {

	public GameObject explosion;
	public Char_AttributeScript whoFiredMe;

	// Use this for initialization
	void Start () {
		Destroy(gameObject,10f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0,5,0));
		//rigidbody.velocity += Physics.gravity*0.005f;
	}

	public void Explode(){
		GameObject expl = PhotonNetwork.Instantiate (explosion.name,transform.position,Quaternion.identity,0) as GameObject;
		expl.GetComponent<Weapon_RocketExplosion>().whoFiredMe = whoFiredMe;
		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision other){
		Explode();
		if (other.gameObject.GetComponent<Char_AttributeScript>()){
			try {
				if (other.gameObject.GetComponent<Char_AttributeScript>().health <= 0 && 
				    whoFiredMe.team != other.gameObject.GetComponent<Char_AttributeScript>().team){
					whoFiredMe.EnableKillHUD(other.transform.GetComponent<Char_AttributeScript>().playerName);
				}
			} catch (System.NullReferenceException e){}
			DamagePlayer(-20,other.gameObject.GetComponent<PhotonView>().viewID);
		}
	}
	[RPC] void DamagePlayer(int damage, int vID){
		Char_AttributeScript cas = PhotonView.Find(vID).transform.GetComponent<Char_AttributeScript>();
		cas.ChangeHP(damage, Vector3.zero);
		if (photonView.isMine)
			photonView.RPC("DamagePlayer", PhotonTargets.OthersBuffered, damage, vID);
	}

}
