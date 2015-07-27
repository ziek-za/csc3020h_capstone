using UnityEngine;
using System.Collections;

public class Menu_CameraScript : MonoBehaviour {
	Vector3 startPos;
	
	public float amplitude = 10f;
	public float period = 5f;
	
	protected void Start() {
		startPos = transform.position;
	}
	
	protected void Update() {
		float theta = Time.timeSinceLevelLoad / period;
		float distance_x = amplitude * Mathf.Sin(theta);
		float distance_y = amplitude * Mathf.Sin(theta + 35f);
		transform.position = startPos + (Vector3.up * distance_y) + (Vector3.left * distance_x);
	}
}
