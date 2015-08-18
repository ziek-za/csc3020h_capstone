using UnityEngine;
using System.Collections;

public class Map_LinkScript : Photon.MonoBehaviour {

	//Set this value when initializing GameObject
	public Char_AttributeScript.Teams currentTeam;

	public ParticleSystem redBeam, blueBeam, neutralBeam;

	Level_GUIController gui;

	// Use this for initialization
	void Start () {
		gui = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();
		if (currentTeam == Char_AttributeScript.Teams.BLUE)
			InitBlue();
		else if (currentTeam == Char_AttributeScript.Teams.RED)
			InitRed();
		else
			InitNeutral();
	}

	// Update is called once per frame
	void Update () {
	}

	public void InitBlue(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		blueBeam.Play();
	}

	public void InitRed(){
		currentTeam = Char_AttributeScript.Teams.RED;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.red;
		redBeam.Play();
	}

	public void InitNeutral(){
		currentTeam = Char_AttributeScript.Teams.NONE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.white;
		neutralBeam.Play();
	}

	//[RPC] void RedCaptureRPC(){
	//	if (photonView.isMine)
	//		photonView.RPC("RedCaptureRPC", PhotonTargets.OthersBuffered);
	//}

	[RPC] IEnumerator RedTeamCapture(){
		yield return new WaitForSeconds(5f);
		currentTeam = Char_AttributeScript.Teams.RED;
		Debug.Log("Red Team Captures Link");
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.red;
		blueBeam.Stop();
		neutralBeam.Stop();
		redBeam.Play();
		gui.SetUpLinkButtons();

		if (photonView.isMine)
			photonView.RPC("RedTeamCapture", PhotonTargets.OthersBuffered);
	}

	[RPC] IEnumerator BlueTeamCapture(){
		yield return new WaitForSeconds(5f);
		currentTeam = Char_AttributeScript.Teams.BLUE;
		Debug.Log("Blue Team Captures Link");
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		blueBeam.Play();
		redBeam.Stop();
		neutralBeam.Stop();
		gui.SetUpLinkButtons();

		if (photonView.isMine)
			photonView.RPC("BlueTeamCapture", PhotonTargets.OthersBuffered);
	}

	void OnTriggerExit(Collider other){
		if (other.GetComponent<Char_AttributeScript>()){
			if (currentTeam == Char_AttributeScript.Teams.BLUE){
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.RED){
					StopCoroutine("RedTeamCapture");
				}
			} else if (currentTeam == Char_AttributeScript.Teams.RED) {
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.BLUE){
					StopCoroutine("BlueTeamCapture");
				}
			//Neutral Checkpoint
			} else {
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.BLUE){
					StopCoroutine("BlueTeamCapture");
				} else if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.RED){
					StopCoroutine("RedTeamCapture");
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Char_AttributeScript>()){
			if (currentTeam == Char_AttributeScript.Teams.BLUE){
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.RED){
					StartCoroutine("RedTeamCapture");
				}
			} else if (currentTeam == Char_AttributeScript.Teams.RED) {
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.BLUE){
					StartCoroutine("BlueTeamCapture");
				}
			//Neutral Checkpoint
			} else {
				if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.BLUE){
					StartCoroutine("BlueTeamCapture");
				} else if (other.GetComponent<Char_AttributeScript>().team == Char_AttributeScript.Teams.RED){
					StartCoroutine("RedTeamCapture");
				}
			}
		}
	}
}
