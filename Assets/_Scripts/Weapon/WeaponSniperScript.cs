using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponSniperScript : Char_BasicShootScript {

	public GameObject sniperCrosshair;
	public GameObject actualCameraPosition;

	public float zoomedAccuracy = 100f;
	float originalAccuracy;

	float originalSensitivity;
	Char_BasicMoveScript moveScript;

	bool zoomed = false;

	// Use this for initialization
	void Start () {
		base.Start();
		sniperCrosshair = GameObject.Find ("SniperCrosshair");
		originalAccuracy = base.weaponAccuracy;
		moveScript = GetComponentInParent<Char_BasicMoveScript>();
		originalSensitivity = moveScript.mouseSpeed;
	}

	// Update is called once per frame
	void Update () {
		base.Update();

		if (photonView.isMine){
			if (Input.GetButtonDown("Fire3")){
				if (!zoomed){
					sniperCrosshair.GetComponent<RawImage>().enabled = true;
					Camera.main.fieldOfView = 12;
					SkinnedMeshRenderer[] meshes = actualCameraPosition.GetComponentsInChildren<SkinnedMeshRenderer>();
					for (int i = 0; i < meshes.Length; i++){
						meshes[i].enabled = false;
					}
					zoomed = true;
					base.weaponAccuracy = zoomedAccuracy;
					moveScript.mouseSpeed = moveScript.mouseSpeed/5;
				} else {
					sniperCrosshair.GetComponent<RawImage>().enabled = false;
					Camera.main.fieldOfView = 60;
					zoomed = false;
					SkinnedMeshRenderer[] meshes = actualCameraPosition.GetComponentsInChildren<SkinnedMeshRenderer>();
					for (int i = 0; i < meshes.Length; i++){
						meshes[i].enabled = true;
					}
					base.weaponAccuracy = originalAccuracy;
					moveScript.mouseSpeed = originalSensitivity;
				}
			}
			/*if (Input.GetButtonDown("Fire1")){
				if(zoomed)
					base.CameraShake(1f,0.2f);
			}*/
		}

	}
}
