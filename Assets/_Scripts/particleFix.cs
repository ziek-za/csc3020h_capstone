using UnityEngine;
using System.Collections;

public class particleFix : MonoBehaviour {
	int counter = 0;
	float shakeAmountX;
	float shakeAmountY;
	float shakeAmountZ;
	// Use this for initialization
	void Start () {

	}

	void Update(){
		counter++;
		if(particleSystem.time>=2.5f){
			if (counter%2==0){
				shakeAmountX = Random.Range (-0.1f, 0.1f);
				shakeAmountY = Random.Range (-0.1f, 0.1f);
				shakeAmountZ = Random.Range (-0.1f, 0.1f);
				particleSystem.transform.position = new Vector3(transform.position.x+shakeAmountX,transform.position.y+shakeAmountY,transform.position.z+shakeAmountZ);
			}
			else if (counter%2==1){
				particleSystem.transform.position = new Vector3(transform.position.x-shakeAmountX,transform.position.y-shakeAmountY,transform.position.z-shakeAmountZ);
			}

		}
	}

}
