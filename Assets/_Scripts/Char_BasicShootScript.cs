using UnityEngine;
using System.Collections;

public class Char_BasicShootScript : MonoBehaviour {

	private RaycastHit hit;
	private Ray ray;
	
	void Update()
	{
		
		Vector2 screenCenterPoint = new Vector2(Screen.width/2, Screen.height/2);
		ray = Camera.main.ScreenPointToRay(screenCenterPoint);

		if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane)) 
		{
			Debug.DrawLine(transform.position, hit.point);
		}
	
	}
}
