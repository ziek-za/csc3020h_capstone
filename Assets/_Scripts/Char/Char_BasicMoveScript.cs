using UnityEngine;
using System.Collections;

public class Char_BasicMoveScript : Photon.MonoBehaviour {

	
	public float moveSpeed = 10.0f;
	public float mouseSpeed = 3.0f;
	public float jumpSpeed=5.0f;
	public Transform FPSCameraPos;
	//public Transform currentPlayer;
	
	float mouseSensitivity=2f;
	bool isJumping=false;
	public float clampYAxis = 60.0f;

	// Use this for initialization
	void Start () {
		if (photonView.isMine) {
			Screen.lockCursor=true;
			transform.GetComponent<Renderer>().enabled = false;

		}
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log(PhotonNetwork.networkingPeer.RoundTripTime);
		if (photonView.isMine)
		{
			//Camera.main.transform.parent = rigidbody.transform;
			//Camera.main.transform.position = new Vector3(rigidbody.position.x, rigidbody.position.y, rigidbody.position.z);

			InputMovement();
			InputColorChange();
			MouseView();
			UpdateCameraPos();
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
				transform.Translate(Vector3.forward*moveSpeed*Time.deltaTime * v);
				transform.Translate(Vector3.right*moveSpeed*Time.deltaTime * h);
		}
		if(Input.GetButtonDown("Jump") && isJumping==false){
			isJumping=true;
			Vector3 v3 = rigidbody.velocity;
			v3.y=jumpSpeed;
			rigidbody.velocity=v3;
		}
	}

	void MouseView(){
		transform.Rotate (0, Input.GetAxis ("Mouse X") * mouseSpeed, 0);
		mouseSensitivity -= Input.GetAxis ("Mouse Y") * mouseSpeed;
		mouseSensitivity = Mathf.Clamp (mouseSensitivity, -clampYAxis, clampYAxis);

		float rotateY = Input.GetAxis ("Mouse Y") * mouseSpeed;
		FPSCameraPos.transform.localRotation = Quaternion.Euler (mouseSensitivity, 0, 0);
	}

	void OnCollisionEnter(Collision c){
				if (photonView.isMine) {
						isJumping = false;
				}
		}
}
