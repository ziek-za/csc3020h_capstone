using UnityEngine;
using System.Collections;

public class Weapon_Rocket : Photon.MonoBehaviour {

	public GameObject explosion;
	public Char_AttributeScript whoFiredMe;
	
	private RaycastHit hit;

	// Use this for initialization
	void Start () {
		Destroy(gameObject,10f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0,5,0));

		Vector3 fwd = transform.TransformDirection(Vector3.up);
		if (Physics.Raycast(transform.position, fwd,out hit, 2)) {
			Explode(hit.transform.gameObject);
		}
	}

	public void Explode(GameObject other){
		GameObject expl = PhotonNetwork.Instantiate (explosion.name,transform.position,Quaternion.identity,0) as GameObject;
		expl.GetComponent<Weapon_RocketExplosion>().whoFiredMe = whoFiredMe;

		if (other.GetComponent<Char_AttributeScript>()){
			DamagePlayer(-20,other.gameObject.GetComponent<PhotonView>().viewID);
			try {
				if (other.GetComponent<Char_AttributeScript>().health <= 0 && 
				    whoFiredMe.team != other.gameObject.GetComponent<Char_AttributeScript>().team){
					whoFiredMe.EnableKillHUD(other.transform.GetComponent<Char_AttributeScript>().playerName);
				}
			} catch (System.NullReferenceException e){}

		}

		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision other){
		Explode(other.gameObject);
	}

	[RPC] void DamagePlayer(int damage, int vID){
		Char_AttributeScript cas = PhotonView.Find(vID).transform.GetComponent<Char_AttributeScript>();
		cas.ChangeHP(damage, Vector3.zero);
		if (photonView.isMine)
			photonView.RPC("DamagePlayer", PhotonTargets.OthersBuffered, damage, vID);
	}

}
