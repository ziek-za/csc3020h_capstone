using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Map_LinkScript : Photon.MonoBehaviour {

	//Set this value when initializing GameObject
	public Slider linkSlider;
	
	public Char_AttributeScript.Teams currentTeam;
	public MeshRenderer newLinkPart1, newLinkPart2;

	float linkValue=50;
	int redCounter=0;
	int blueCounter=0;
	float defaultCaptureSpeed=10f;
	float captureSpeed=0;
	public ParticleSystem redBeam, blueBeam, neutralBeam;
	public AudioClip captureSound;

	protected Level_GUIController gui;
	float ScrollingMessageTimeout = 1f;

	// Use this for initialization
	void Start () {
		//linkSlider.gameObject.SetActive (false);
		gui = GameObject.Find("GUI Controller").GetComponent<Level_GUIController>();
		if (currentTeam == Char_AttributeScript.Teams.BLUE){
			linkValue = 0;
			InitBlue();
		}
		else if (currentTeam == Char_AttributeScript.Teams.RED){
			linkValue = 100;
			InitRed();
		}
		else
			InitNeutral();
	}

	// Update is called once per frame
	void Update () {
		ScrollingMessageTimeout -= Time.deltaTime;
		if (blueCounter < 0)
			blueCounter = 0;
		if (redCounter < 0)
			redCounter = 0;

		captureSpeed = defaultCaptureSpeed * Time.deltaTime * (5/(2+3*(Mathf.Pow(Mathf.Abs(redCounter-blueCounter),-2))));
	
		if (linkValue > 100) {
			linkValue=100;
		}else if(linkValue<0){
			linkValue=0;
		}
		if (redCounter - blueCounter > 0) {//Start converting red
			convertRed();
		}
		else if (blueCounter - redCounter > 0) {//Start converting red
			convertBlue();
		}else if(redCounter>0||blueCounter>0){
		}
		else {
			if(currentTeam==Char_AttributeScript.Teams.BLUE){
				linkValue = linkValue-defaultCaptureSpeed * Time.deltaTime;
			} else if(currentTeam==Char_AttributeScript.Teams.RED){
				linkValue = linkValue+defaultCaptureSpeed * Time.deltaTime;
			} else if(currentTeam==Char_AttributeScript.Teams.NONE){
				if(linkValue>50){
					linkValue = linkValue-defaultCaptureSpeed * Time.deltaTime;
				}else if(linkValue<50){
					linkValue = linkValue+defaultCaptureSpeed * Time.deltaTime;
				}
			} 
		}
	}

	public void InitBlue(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		newLinkPart1.materials[0].color =  Color.blue;
		newLinkPart2.materials[0].color =  Color.blue;
		blueBeam.Play(true);
	}

	public void InitRed(){
		currentTeam = Char_AttributeScript.Teams.RED;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.red;
		newLinkPart1.materials[0].color =  Color.red;
		newLinkPart2.materials[0].color =  Color.red;
		redBeam.Play(true);
	}

	public void InitNeutral(){
		currentTeam = Char_AttributeScript.Teams.NONE;
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.white;
		neutralBeam.Play(true);
	}
	
	[RPC] void RedTeamCapture(){
		currentTeam = Char_AttributeScript.Teams.RED;
		Debug.Log("Red Team Captures Link");
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.red;
		newLinkPart1.materials[0].color =  Color.red;
		newLinkPart2.materials[0].color =  Color.red;
		blueBeam.Stop(true);
		neutralBeam.Stop(true);
		redBeam.Play(true);
		gui.SetUpLinkButtons();
		AudioSource.PlayClipAtPoint (captureSound, transform.position);
		if (ScrollingMessageTimeout < 0){
			gui.AddItemToScrollingList("Red team captures link",Color.red);
			ScrollingMessageTimeout = 1f;
		}

		if (photonView.isMine)
			photonView.RPC("RedTeamCapture", PhotonTargets.OthersBuffered);
	}

	[RPC] void BlueTeamCapture(){
		currentTeam = Char_AttributeScript.Teams.BLUE;
		Debug.Log("Blue Team Captures Link");
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.blue;
		newLinkPart1.materials[0].color =  Color.blue;
		newLinkPart2.materials[0].color =  Color.blue;
		blueBeam.Play(true);
		redBeam.Stop(true);
		neutralBeam.Stop(true);
		gui.SetUpLinkButtons();
		AudioSource.PlayClipAtPoint (captureSound, transform.position);
		if (ScrollingMessageTimeout < 0){
			gui.AddItemToScrollingList("Blue team captures link",Color.blue);
			ScrollingMessageTimeout = 1f;
		}

		if (photonView.isMine)
			photonView.RPC("BlueTeamCapture", PhotonTargets.OthersBuffered);
	}

	[RPC] void  NeutralTeamCapture(){
		currentTeam = Char_AttributeScript.Teams.NONE;
		Debug.Log("Link has been neutralized");
		this.GetComponent<MeshRenderer>().materials[0].color =  Color.white;
		newLinkPart1.materials[0].color =  Color.white;
		newLinkPart2.materials[0].color =  Color.white;
		neutralBeam.Play(true);
		redBeam.Stop(true);
		blueBeam.Stop(true);
		gui.SetUpLinkButtons();

		if (photonView.isMine)
			photonView.RPC("NeutralTeamCapture", PhotonTargets.OthersBuffered);
	}

	public void PlayerDeathMethod(Collider other){
		if(other.GetComponent<Char_AttributeScript>().team== Char_AttributeScript.Teams.BLUE){
			blueCounter--;
		}
		else if(other.GetComponent<Char_AttributeScript>().team== Char_AttributeScript.Teams.RED){
			redCounter--;
		}
		if (other.GetComponent<PhotonView>().isMine){
			try {
				linkSlider.gameObject.SetActive (false);
			} catch (System.Exception e){}
			CancelInvoke("UpdateSlider");
		}
	}

	void OnTriggerExit(Collider other){
		Debug.Log ("Leaving collider: ");
		if (other.GetComponent<Char_AttributeScript>()){
			//other.GetComponent<Char_AttributeScript>().currentLink = null;
			other.GetComponent<Char_AttributeScript>().
				ExitLink(other.GetComponent<PhotonView>().viewID);
			if(other.GetComponent<Char_AttributeScript>().team== Char_AttributeScript.Teams.BLUE){
				blueCounter--;
			}
			else if(other.GetComponent<Char_AttributeScript>().team== Char_AttributeScript.Teams.RED){
				redCounter--;
			}
			if (other.GetComponent<PhotonView>().isMine){
				linkSlider.gameObject.SetActive (false);
				CancelInvoke("UpdateSlider");
			}
				
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Char_AttributeScript>()){
			//other.GetComponent<Char_AttributeScript>().currentLink = transform.GetComponent<Map_LinkScript>();
			other.GetComponent<Char_AttributeScript>().
				EnterLink(other.GetComponent<PhotonView>().viewID, GetComponent<PhotonView>().viewID);
			if(other.GetComponent<Char_AttributeScript>().team== Char_AttributeScript.Teams.BLUE){
				blueCounter++;
				if(linkValue>0 && other.GetComponent<PhotonView>().isMine){
					linkSlider.gameObject.SetActive (true);
				}
			}
			else if(other.GetComponent<Char_AttributeScript>().team== Char_AttributeScript.Teams.RED){
				redCounter++;
				if(linkValue<100 && other.GetComponent<PhotonView>().isMine){
					linkSlider.gameObject.SetActive (true);
				}
			}
			//linkSlider.gameObject.SetActive (true);
			if(other.GetComponent<PhotonView>().isMine){
				InvokeRepeating("UpdateSlider",0,0.01f);
			}
		}
	}

	void UpdateSlider(){
		linkSlider.value=linkValue;
	}

	void convertBlue(){
		if (linkValue <= 0 && currentTeam!=Char_AttributeScript.Teams.BLUE) {
			//if(currentTeam != Char_AttributeScript.Teams.BLUE){
				BlueTeamCapture();
			//}
		}else if(linkValue>=49 && linkValue <= 51 && currentTeam!=Char_AttributeScript.Teams.NONE){
			//if(currentTeam==Char_AttributeScript.Teams.RED){
				NeutralTeamCapture();
			//}
			linkValue = linkValue-captureSpeed;
		}else {
			linkValue = linkValue-captureSpeed;
		}
	}

	void convertRed(){
		if (linkValue >= 100 && currentTeam!=Char_AttributeScript.Teams.RED) {
			//if(currentTeam == Char_AttributeScript.Teams.NONE){
				RedTeamCapture();
			//}
		} else if(linkValue>=49 && linkValue <= 51 && currentTeam!=Char_AttributeScript.Teams.NONE){
			//if(currentTeam==Char_AttributeScript.Teams.BLUE){
				NeutralTeamCapture();
			//}
			linkValue = linkValue+captureSpeed;
		}else {
			linkValue = linkValue+captureSpeed;
		}
	}

	void convertNeutral(){
		if (linkValue < 50) {
			linkValue=linkValue+captureSpeed;
				}
		else if(linkValue>50){
			linkValue=linkValue-captureSpeed;
		}
	}
}
