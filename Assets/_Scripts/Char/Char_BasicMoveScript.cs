using UnityEngine;
using System.Collections;

public class Char_BasicMoveScript : Photon.MonoBehaviour {
		
	//float shake = 0f;
	//float shakeAmount  = 0.05f;
	//float decreaseFactor = 10.0f;
	public Animator anim;
	public float moveSpeed = 10.0f;
	public bool sprint=false;
	public float mouseSpeed = 3.0f;
	public float jumpSpeed=5.0f;
	public Transform FPSCameraPos;
	public float sniperRotationModifier = 0f;
	public AudioClip footsteps;
	public AudioClip footsteps_sprint;
	public AudioClip jump;
	public AudioSource jumpAudio;
	AudioSource audio;

	//public Transform currentPlayer;

	float mouseSensitivity=2f;
	bool isJumping=false, inAir = false;
	float clampYAxis = 90.0f;

	public bool inVortex = false;
	float inVortexTime = 2;
	private RaycastHit hit;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource> ();
		audio.clip = footsteps;


		anim=GetComponentInChildren<Animator>();
		if (photonView.isMine) {

			Screen.lockCursor=true;

		}
	}

	void EnableKeys(){
		inVortex = false;
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log(PhotonNetwork.networkingPeer.RoundTripTime);
		if (photonView.isMine)
		{
			if (!inVortex)
				InputMovement();
			else if (!IsInvoking("EnableKeys"))
				Invoke ("EnableKeys",inVortexTime);
			//InputColorChange();
			MouseView();

			UpdateCameraPos();
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
			inAir = false;
			anim.SetBool("Jumping",false);
		} else {
			//In air
			inAir = true;
			anim.SetBool("Jumping",true);
		}

		if(Input.GetButtonDown("Jump") && inAir == false){
			jumpAudio.PlayOneShot(jump);
			anim.SetBool("Jumping",true);
			//isJumping=true;
			Vector3 v3 = rigidbody.velocity;
			v3.y=jumpSpeed;
			rigidbody.velocity=v3;
		} 

		//Determining footsteps sound
		if(audio.isPlaying==false){
			audio.Play ();
		}
		//Debug.Log(audio.isPlaying+" "+audio.pitch+" "+audio.clip);
		if (anim.GetBool("Jumping")==true) {
			audio.pitch=0;
		}
		else if(anim.GetFloat ("Speed")>=0.05f){
			audio.pitch=1;
			if(sprint){
				audio.clip=footsteps_sprint;
				//audio.pitch=0;
			}else{
				//audio.pitch=1;
				audio.clip=footsteps;
			}
		}
		else if(anim.GetFloat("Speed")<=0.05f){
			audio.pitch=0;
		}
	}

	void MouseView(){
		transform.Rotate (0, Input.GetAxis ("Mouse X") * mouseSpeed, 0);
		mouseSensitivity -= Input.GetAxis ("Mouse Y") * mouseSpeed;
		mouseSensitivity = Mathf.Clamp (mouseSensitivity, -clampYAxis, clampYAxis);

		FPSCameraPos.transform.localRotation = Quaternion.Euler (mouseSensitivity + sniperRotationModifier, 0, 0);
	}

	/*
	void OnCollisionEnter(Collision other){
		if (Mathf.Abs(transform.rigidbody.velocity.y) < 1f){//Bottom of collider is colliding
			isJumping = false;
		}
	}*/
}
