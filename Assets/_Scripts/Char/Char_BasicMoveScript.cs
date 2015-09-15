﻿using UnityEngine;
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
	//public Transform currentPlayer;
	
	float mouseSensitivity=2f;
	bool isJumping=false;
	public float clampYAxis = 60.0f;

	public bool inVortex = false;

	// Use this for initialization
	void Start () {
		anim=GetComponentInChildren<Animator>();
		if (photonView.isMine) {

			Screen.lockCursor=true;

		}
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log(PhotonNetwork.networkingPeer.RoundTripTime);
		if (photonView.isMine)
		{
			if (!inVortex)
				InputMovement();
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

		if(Input.GetButtonDown("Jump") && isJumping==false){
			anim.SetBool("Jumping",true);
			isJumping=true;
			Vector3 v3 = rigidbody.velocity;
			v3.y=jumpSpeed;
			rigidbody.velocity=v3;
		} else if (transform.rigidbody.velocity.y < -2f){//Assumed to be falling
			anim.SetBool("Falling",true);
			isJumping = true;
			anim.SetBool("Jumping",false);
		}else{
			anim.SetBool("Falling",false);
		}

	}

	void MouseView(){
		transform.Rotate (0, Input.GetAxis ("Mouse X") * mouseSpeed, 0);
		mouseSensitivity -= Input.GetAxis ("Mouse Y") * mouseSpeed;
		mouseSensitivity = Mathf.Clamp (mouseSensitivity, -clampYAxis, clampYAxis);

		FPSCameraPos.transform.localRotation = Quaternion.Euler (mouseSensitivity, 0, 0);
	}

	void OnCollisionEnter(Collision other){
		if (Mathf.Abs(transform.rigidbody.velocity.y) < 1f){//Bottom of collider is colliding
			isJumping = false;
		}
	}
}
