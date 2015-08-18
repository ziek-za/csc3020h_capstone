﻿using UnityEngine;
using System.Collections;

public class Char_BasicMoveScript : Photon.MonoBehaviour {
		
	//float shake = 0f;
	//float shakeAmount  = 0.05f;
	//float decreaseFactor = 10.0f;
	
	public float moveSpeed = 10.0f;
	public float mouseSpeed = 3.0f;
	public float jumpSpeed=5.0f;
	public Transform FPSCameraPos;
	//public Transform currentPlayer;
	
	float mouseSensitivity=2f;
	bool isJumping=false;
	public float clampYAxis = 60.0f;

	Vector3 prevPos = Vector3.zero;

	// Use this for initialization
	void Start () {
		if (photonView.isMine) {
			Screen.lockCursor=true;


			//Don't hide our player for now
			//transform.GetComponent<Renderer>().enabled = false;

		}
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log(PhotonNetwork.networkingPeer.RoundTripTime);
		if (photonView.isMine)
		{
			//if (Input.GetButton("Fire1"))
			//	shake = 0.5f;

			InputMovement();
			InputColorChange();
			MouseView();
			UpdateCameraPos();
			//SetSynchronizedValues
		}

	}

	void UpdateCameraPos(){
		/*if (shake > 0) {
			Debug.Log (shake);
			FPSCameraPos.position += Random.insideUnitSphere * shakeAmount;
			shake -= Time.deltaTime * decreaseFactor;			
		} else {
			shake = 0.0f;
		}*/
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
			Vector3 speed = (Vector3.forward*moveSpeed*Time.deltaTime * v) + (Vector3.right*moveSpeed*Time.deltaTime * h);
			//transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime * v);
			//transform.Translate(Vector3.right*moveSpeed*Time.deltaTime * h);
			transform.Translate(speed);
			//gameObject.GetComponent<PhotonTransformView>().SetSynchronizedValues(speed,mouseSpeed);
		}

		/*float yDiff = (transform.position.y - prevPos.y)/Time.deltaTime;
		if (Mathf.Abs(yDiff) < 0.1f){
			isJumping = false;
		} else {
			isJumping = true;
		}*/

		if(Input.GetButtonDown("Jump") && isJumping==false){
			isJumping=true;
			Vector3 v3 = rigidbody.velocity;
			v3.y=jumpSpeed;
			rigidbody.velocity=v3;
		} else if (transform.rigidbody.velocity.y < -2f){//Assumed to be falling
			isJumping = true;
		}



		//prevPos = transform.position;
	}

	void MouseView(){
		transform.Rotate (0, Input.GetAxis ("Mouse X") * mouseSpeed, 0);
		mouseSensitivity -= Input.GetAxis ("Mouse Y") * mouseSpeed;
		mouseSensitivity = Mathf.Clamp (mouseSensitivity, -clampYAxis, clampYAxis);

		float rotateY = Input.GetAxis ("Mouse Y") * mouseSpeed;
		FPSCameraPos.transform.localRotation = Quaternion.Euler (mouseSensitivity, 0, 0);
	}

	void OnCollisionEnter(Collision other){
		if (Mathf.Abs(transform.rigidbody.velocity.y) < 1f){//Bottom of collider is colliding
			isJumping = false;
		}
	}
}
