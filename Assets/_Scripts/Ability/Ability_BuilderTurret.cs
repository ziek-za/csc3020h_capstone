using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability_BuilderTurret : MonoBehaviour {

	public Char_AttributeScript.Teams currentTeam;
	float lifeTime = 10000f;
	public int health = 100;
	public GameObject muzzle;
	
	float lifetimeAccum = 0;
	
	List<GameObject> trackedEnemies;

	public GameObject leftConeEdge, rightConeEdge;
	
	// Use this for initialization
	void Start () {
		trackedEnemies = new List<GameObject>();
	}
	
	public void ChangeHP(int change){
		health += change;
	}

	float oldAngle = 0;
	float rotationsPerSecond = 0.1f;
	float rotationAccum = 0;

	void Update(){
		lifetimeAccum += Time.deltaTime;
		
		if (lifetimeAccum >= lifeTime){
			KillSelf();
		}
		
		if (health <= 0){
			KillSelf();
		}

		rotationAccum += Time.deltaTime;

		if (trackedEnemies.Count > 0){

			Vector3 dir = transform.position - trackedEnemies[0].transform.position; // find direction
			//dir.y = 0; // keep only the horizontal direction
			Quaternion rot = Quaternion.LookRotation(dir);

			//muzzle.transform.LookAt(trackedEnemies[0].transform.position);
			//Vector3 muzzleDir = muzzle.transform.position - trackedEnemies[0].transform.position;
			//Debug.Log(Vector3.Angle(dir,muzzleDir));
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5f * Time.deltaTime);
			//Debug.Log (dir);
		}
	}
	
	public void SetTeam(Char_AttributeScript.Teams newTeam){
		currentTeam = newTeam;
		if (currentTeam == Char_AttributeScript.Teams.BLUE)
			InitBlue();
		else if (currentTeam == Char_AttributeScript.Teams.RED)
			InitRed();
		//Invoke("KillSelf",lifeTime);
	}
	
	public void SetLifetime(float baselifetime){
		lifetimeAccum = baselifetime;
	}
	
	void InitBlue(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
	}

	void InitRed(){
		currentTeam = Char_AttributeScript.Teams.RED;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.red;
	}

	
	void OnTriggerExit(Collider other){
		if (other.GetComponent<Char_AttributeScript>()){// &&  other.GetComponent<Char_AttributeScript>().team != currentTeam){
			Debug.Log("exit");
			trackedEnemies.Remove(other.gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Char_AttributeScript>()){// &&  other.GetComponent<Char_AttributeScript>().team != currentTeam){
			Debug.Log("enter");
			trackedEnemies.Add(other.gameObject);
		}
	}

	void KillSelf(){
		gameObject.tag = "Untagged";
		particleSystem.Play();
		Invoke("ActuallyRemove",0.1f);
	}
	
	void ActuallyRemove(){
		Destroy(transform.parent.gameObject);
	}
}
