using UnityEngine;
using System.Collections;

public class Char_BasicMoveScript : Photon.MonoBehaviour {
		
	//float shake = 0f;
	//float shakeAmount  = 0.05f;
	//float decreaseFactor = 10.0f;
	public Animator anim;
	public float moveSpeed = 10.0f;
	public float mouseSpeed = 3.0f;
	public float jumpSpeed=5.0f;
	public Transform FPSCameraPos;
	public float sniperRotationModifier = 0f;
	public AudioClip[] footstepsCement;
	public AudioClip[] footstepsWood;
	public AudioClip[] footstepsSand;
	public AudioClip[] footstepsMetal;
	public AudioClip jump;
	public AudioClip moveFast;
	public AudioSource jumpAudio;
	bool step = true;
	string terrainBelowTag;
	AudioSource audio;
	float audioLength;
	AudioReverbZone[] reverbzones;

	//public Transform currentPlayer;

	float mouseSensitivity=2f;
	bool isJumping=false, inAir = false;
	float clampYAxis = 90.0f;

	public bool inVortex = false, respawning = false;
	float inVortexTime = 2;
	private RaycastHit hit;

	// Use this for initialization
	void Start () {
		reverbzones = GetComponentsInChildren<AudioReverbZone> ();
		for (int i = 0; i < reverbzones.Length; i++) {
			reverbzones[i].gameObject.SetActive(false);
				}
		audio = GetComponent<AudioSource> ();


		anim=GetComponentInChildren<Animator>();
		if (photonView.isMine) {

			Screen.lockCursor=true;

		}
	}

	void EnableKeys(){
		inVortex = false;
	}

	void Update () {
		if (photonView.isMine){
			UpdateCameraPos();
		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		//Debug.Log(PhotonNetwork.networkingPeer.RoundTripTime);
		if (photonView.isMine)
		{
			if (!respawning){
				if (!inVortex)
					InputMovement();
				else if (!IsInvoking("EnableKeys"))
					Invoke ("EnableKeys",inVortexTime);
				//InputColorChange();

				MouseView();

				//UpdateCameraPos();
			}
			//SetSynchronizedValues
		}


	}

	void UpdateCameraPos(){
		Camera.main.transform.position = FPSCameraPos.position;
		Camera.main.transform.rotation = FPSCameraPos.rotation;
	}

	private void InputColorChange()
	{
		if (Input.GetKeyDown(KeyCode.E))
			ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
	}
	
	[RPC] void ChangeColorTo(Vector3 color)
	{
		renderer.material.color = new Color(color.x, color.y, color.z, 1f);
		
		if (photonView.isMine)
			photonView.RPC("ChangeColorTo", PhotonTargets.OthersBuffered, color);
	}
	
	void InputMovement()
	{	
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		if (h != 0f || v != 0){
			Vector3 speed =  0.75f * ((transform.forward*moveSpeed*Time.deltaTime * v) + (transform.right*moveSpeed*Time.deltaTime * h));
			transform.rigidbody.MovePosition(transform.position + speed);

			anim.SetFloat("Speed",Mathf.Abs(speed.magnitude)*8);
		}else{
			anim.SetFloat ("Speed",0);
		}

		if (Physics.Raycast(transform.position, Vector3.down,out hit, 1.25f)) {
			Debug.DrawLine(transform.position, hit.point, Color.red);
			terrainBelowTag=hit.transform.tag;
			inAir = false;
			anim.SetBool("Jumping",false);
		} else {
			//In air
			inAir = true;
			anim.SetBool("Jumping",true);
		}

		if(Input.GetButtonDown("Jump") && inAir == false){
			SendJumpSound(GetComponent<PhotonView>().viewID);
			anim.SetBool("Jumping",true);
			//isJumping=true;
			Vector3 v3 = rigidbody.velocity;
			v3.y=jumpSpeed;
			rigidbody.velocity=v3;
		} 

		//Determining footsteps sound
		if(anim.GetFloat ("Speed")>=0.05f && !anim.GetBool("Jumping") && step==true){
			SendFootstepSound(GetComponent<PhotonView>().viewID, terrainBelowTag);
		}
	}

	IEnumerator waitFootsteps(){
		yield return new WaitForSeconds (0.33f);
		//yield return new WaitForSeconds (audio.clip.length);
		step=true;
		}

	void generateFootstep(string terrainType){
		float oldVolume = audio.volume;
		audio.volume *= 5;

		if (terrainType == "Wood" && step==true) {
			//Debug.Log("Wood");
			step=false;
			audio.clip=footstepsWood[Random.Range(0,footstepsWood.Length)];
			audio.Play();
			StartCoroutine (waitFootsteps ());
		}
		else if (terrainType == "Sand" && step==true) {
			//Debug.Log("Cement");
			Debug.Log (Time.time +" " + terrainType+" "+audio.clip);
			step=false;
			audio.clip=footstepsSand[Random.Range(0,footstepsSand.Length)];
			audio.Play();
			StartCoroutine (waitFootsteps ());
		}
		else if (terrainType == "Metal" && step==true) {
			//Debug.Log("Metal");
			step=false;
			audio.clip=footstepsMetal[Random.Range(0,footstepsMetal.Length)];
			audio.Play();
			StartCoroutine (waitFootsteps ());
		}
		else{//Cement or untagged
			step=false;
			audio.clip=footstepsCement[Random.Range(0,footstepsCement.Length)];
			audio.Play();
			StartCoroutine (waitFootsteps ());
		}
		audioLength = audio.clip.length;
		audio.volume = oldVolume;
	}

	void MouseView(){
		transform.Rotate (0, Input.GetAxis ("Mouse X") * mouseSpeed, 0);
		mouseSensitivity -= Input.GetAxis ("Mouse Y") * mouseSpeed;
		mouseSensitivity = Mathf.Clamp (mouseSensitivity, -clampYAxis, clampYAxis);

		FPSCameraPos.transform.localRotation = Quaternion.Euler (mouseSensitivity + sniperRotationModifier, 0, 0);
	}

	void OnTriggerEnter(Collider other){
		if (other.name == "reverbCave") {
			for(int i = 0; i < reverbzones.Length;i++){
				if(reverbzones[i].name=="CaveZone"){
					reverbzones[i].gameObject.SetActive(true);
				}
			}
		}
			else if(other.name == "reverbBuilding"){
				for(int i = 0; i < reverbzones.Length;i++){
					if(reverbzones[i].name=="BuildingZone"){
						reverbzones[i].gameObject.SetActive(true);
					}
				}
			}
		else if(other.name == "reverbRoom"){
			for(int i = 0; i < reverbzones.Length;i++){
				if(reverbzones[i].name=="RoomZone"){
					reverbzones[i].gameObject.SetActive(true);
				}
			}
		}
		else if(other.name == "reverbCrane"){
			for(int i = 0; i < reverbzones.Length;i++){
				if(reverbzones[i].name=="MountainZone"){
					reverbzones[i].gameObject.SetActive(true);
				}
			}
		}
		Debug.Log (other.name);
	}

	void OnTriggerExit(Collider other){
		if (other.name == "reverbCave") {
			for(int i = 0; i < reverbzones.Length;i++){
				if(reverbzones[i].name=="CaveZone"){
					reverbzones[i].gameObject.SetActive(false);
				}
			}
		}
		else if(other.name == "reverbBuilding"){
			for(int i = 0; i < reverbzones.Length;i++){
				if(reverbzones[i].name=="BuildingZone"){
					reverbzones[i].gameObject.SetActive(false);
				}
			}
		}
		else if(other.name == "reverbRoom"){
			for(int i = 0; i < reverbzones.Length;i++){
				if(reverbzones[i].name=="RoomZone"){
					reverbzones[i].gameObject.SetActive(false);
				}
			}
		}
		else if(other.name == "reverbCrane"){
			for(int i = 0; i < reverbzones.Length;i++){
				if(reverbzones[i].name=="MountainZone"){
					reverbzones[i].gameObject.SetActive(false);
				}
			}
		}
		}

	[RPC] void SendFootstepSound(int vID, string terrainType){
		PhotonView.Find (vID).GetComponent<Char_BasicMoveScript> ().generateFootstep (terrainType);
		if(photonView.isMine){
			photonView.RPC("SendFootstepSound", PhotonTargets.OthersBuffered, vID, terrainType);
			//photonView.RPC("TeleoutEffect",PhotonTargets.OthersBuffered, position);
		}
	}

	[RPC] void SendJumpSound(int vID){
		PhotonView.Find (vID).GetComponent<Char_BasicMoveScript> ().jumpAudio.PlayOneShot(jump);;
		if(photonView.isMine){
			photonView.RPC("SendJumpSound", PhotonTargets.OthersBuffered, vID);
			//photonView.RPC("TeleoutEffect",PhotonTargets.OthersBuffered, position);
		}
	}
}
