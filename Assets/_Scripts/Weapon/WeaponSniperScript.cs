using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponSniperScript : Char_BasicShootScript {

	public GameObject sniperCrosshair;
	public GameObject actualCameraPosition;

	public float zoomedAccuracy = 100f;
	float originalAccuracy;

	bool zoomed = false;

	// Use this for initialization
	void Start () {
		base.Start();
		sniperCrosshair = GameObject.Find ("SniperCrosshair");
		originalAccuracy = base.weaponAccuracy;
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
				} else {
					sniperCrosshair.GetComponent<RawImage>().enabled = false;
					Camera.main.fieldOfView = 60;
					zoomed = false;
					SkinnedMeshRenderer[] meshes = actualCameraPosition.GetComponentsInChildren<SkinnedMeshRenderer>();
					for (int i = 0; i < meshes.Length; i++){
						meshes[i].enabled = true;
					}
					base.weaponAccuracy = originalAccuracy;
				}
			}
		}

	}
}
