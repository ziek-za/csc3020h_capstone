using UnityEngine;
using System.Collections;

public class Map_LinkScript : Photon.MonoBehaviour {

	//Set this value when initializing GameObject
	public enum Teams {BLUE, RED};
	public Teams currentTeam;


	public ParticleSystem redBeam, blueBeam;

	// Use this for initialization
	void Start () {
		currentTeam = Teams.BLUE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		blueBeam.Play();
	}
	
	// Update is called once per frame
	void Update () {
	}

	//[RPC] void RedCaptureRPC(){
	//	if (photonView.isMine)
	//		photonView.RPC("RedCaptureRPC", PhotonTargets.OthersBuffered);
	//}

	[RPC] IEnumerator RedTeamCapture(){
		yield return new WaitForSeconds(5f);
		currentTeam = Teams.RED;
		Debug.Log("Red Team Captures Link");
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.red;
		blueBeam.Stop();
		redBeam.Play();

		if (photonView.isMine)
			photonView.RPC("RedTeamCapture", PhotonTargets.OthersBuffered);
	}

	[RPC] IEnumerator BlueTeamCapture(){
		yield return new WaitForSeconds(5f);
		currentTeam = Teams.BLUE;
		Debug.Log("Blue Team Captures Link");
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		blueBeam.Play();
		redBeam.Stop();

		if (photonView.isMine)
			photonView.RPC("BlueTeamCapture", PhotonTargets.OthersBuffered);
	}

	void OnTriggerExit(Collider other){
		if (other.GetComponent<Char_AttributeScript>()){
			if (currentTeam == Teams.BLUE){
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.RED){
					StopCoroutine("RedTeamCapture");
				}
			} else {
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.BLUE){
					StopCoroutine("BlueTeamCapture");
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Char_AttributeScript>()){
			if (currentTeam == Teams.BLUE){
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.RED){
					StartCoroutine("RedTeamCapture");
				}
			} else {
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.BLUE){
					StartCoroutine("BlueTeamCapture");
				}
			}
		}
	}
}
