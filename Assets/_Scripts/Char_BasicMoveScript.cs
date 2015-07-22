using UnityEngine;
using System.Collections;

public class Char_BasicMoveScript : Photon.MonoBehaviour {

	public float speed = 10f;
	public float jumpheight = 200;
	private bool isJumping = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log(PhotonNetwork.networkingPeer.RoundTripTime);
		if (photonView.isMine)
		{
			Camera.main.transform.parent = rigidbody.transform;
			//Camera.main.transform.position = new Vector3(rigidbody.position.x, rigidbody.position.y, rigidbody.position.z);
			InputMovement();
			InputColorChange();
		}

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
		//if (photonView.isMine) {
						if (Input.GetKey (KeyCode.W))
			//transform.position += transform.forward * speed;		
								rigidbody.MovePosition (rigidbody.position + transform.forward * speed * Time.deltaTime);
		
						if (Input.GetKey (KeyCode.S))
			//transform.position += -transform.forward * speed;		
								rigidbody.MovePosition (rigidbody.position - transform.forward * speed * Time.deltaTime);
		
						if (Input.GetKey (KeyCode.D))
			//transform.position += transform.right * speed;
								rigidbody.MovePosition (rigidbody.position + transform.right * speed * Time.deltaTime);
		
						if (Input.GetKey (KeyCode.A))
			//transform.position += -transform.right * speed;
								rigidbody.MovePosition (rigidbody.position - transform.right * speed * Time.deltaTime);
						if (!isJumping) {
								if (Input.GetKeyDown (KeyCode.Space)) {
										transform.rigidbody.AddForce (0, jumpheight, 0);
										isJumping = true;
								}
						}
		//		}
	}

	void OnCollisionEnter(Collision c){
				if (photonView.isMine) {
						if (c.gameObject.name == "Plane")
								isJumping = false;
				}
		}
}
