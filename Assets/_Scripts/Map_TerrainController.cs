using UnityEngine;
using System.Collections;

public class Map_TerrainController : MonoBehaviour {
	// ** PUBLIC VARIABLES ** //
	public Texture2D 		HEIGHT_MAP;
	public Terrain			TERRAIN;
	
	// ** PRIVATE VARIABLES ** //
	private int				WIDTH, LENGTH,
	HM_WIDTH, HM_LENGTH;
	private float[,]		hm_buf_1,
	hm_buf_2,
	hm_temp_buf,	// temporary buffer when swapping values
	hm_buf_orig;	// original height map (HEIGHT_MAP)
	private Color[]			hm_original;	// pixel values of original height map
	
	void Start () {
		//Setting up local dimensions
		HM_WIDTH = HEIGHT_MAP.width;
		HM_LENGTH = HEIGHT_MAP.height;
		//Initializing starting HM
		hm_buf_1 = new float[HM_WIDTH, HM_LENGTH];
		hm_buf_2 = new float[HM_WIDTH, HM_LENGTH];
		hm_buf_orig = new float[HM_WIDTH, HM_LENGTH];
		hm_original = HEIGHT_MAP.GetPixels(0);
		/*for (int i = 0; i < HM_WIDTH; i++) {
			for (int k = 0; k < HM_LENGTH; k++) {
				hm_buf_1[i, k] = 0f; hm_buf_2[i, k] = 0f;
				hm_buf_orig[i, k] = hm_original[HM_WIDTH * k + i].a * 0.3f;
			}
		}*/

		//set initial terrain
		//TERRAIN.terrainData.SetHeights (0, 0, hm_buf_orig);
	}
	
	/*void Update () {
		//caluculate height map
		float damping = 0.7f;
		float n = 0;
		//select
		if ( Input.GetMouseButtonDown(1)){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		
			
			if (Physics.Raycast (ray, out hit)) {

				Explode(hit.point);
				//Debug.Log(hit.collider.transform.position);
			}
		}
		for (int ex_x = 1; ex_x < HM_WIDTH - 1; ex_x++) {
			for (int ex_z = 1; ex_z < HM_LENGTH - 1; ex_z++) {
				n = (( 	hm_buf_1[(ex_x + 1), ex_z] +
				      hm_buf_1[ (ex_x - 1), ex_z] +
				      hm_buf_1[ ex_x, (ex_z + 1)] +
				      hm_buf_1[ex_x,(ex_z - 1)] ) / 2f ) -
					hm_buf_2[ex_x, ex_z];
				n *= damping;
				hm_buf_orig[ex_x, ex_z] += n;
				hm_buf_2[ex_x, ex_z] = n;
			}
		}
		//update height map
		TERRAIN.terrainData.SetHeights (0, 0, hm_buf_orig);
		//switch values
		hm_temp_buf = (float[,])hm_buf_1.Clone ();
		hm_buf_1 = (float[,])hm_buf_2.Clone ();
		hm_buf_2 = (float[,])hm_temp_buf.Clone ();


	}

	void Explode(Vector3 explosion_pos) {
		hm_buf_2 [(int)explosion_pos.z, (int)explosion_pos.x] = 1f;

	}*/
}
