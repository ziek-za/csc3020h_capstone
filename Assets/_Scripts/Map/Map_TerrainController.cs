using UnityEngine;
using System.Collections;
using System.IO;

public class Map_TerrainController : Photon.MonoBehaviour {

	//Variable to reset the hm when room first created
	public static bool hmReset;
	public Terrain terrain;
	public bool RippleIsActive = false;
	public float RippleDamping = 0.7f;
	public float TerrainOffset = 128f;
	// ** PRIVATE VARIABLES ** //
	private float WIDTH, LENGTH, MAX_HEIGHT;
	private float[,] height_buffer_1, heights_orig, height_buffer_2,
	height_buffer_original,	// temporary buffer when swapping values
	hm_temp_buf,	// original height map (HEIGHT_MAP)
	flatten_buf; 	// used to flatten terrain
	
	void Start () {
		//Setting up local dimensions
		WIDTH = terrain.terrainData.size.x;
		MAX_HEIGHT = terrain.terrainData.size.y;
		LENGTH = terrain.terrainData.size.z;
		//Initializing starting HM
		height_buffer_1 = new float[(int)WIDTH, (int)LENGTH];
		height_buffer_2 = new float[(int)WIDTH, (int)LENGTH];
		flatten_buf = new float[(int)WIDTH, (int)LENGTH];
		height_buffer_original = new float[(int)WIDTH, (int)LENGTH];
		if (!hmReset){
			//terrain.terrainData.SetHeights (0, 0, flatten_buf);
			SetTerrainHeightMap ();
			height_buffer_original = terrain.terrainData.GetHeights(0,0,(int)WIDTH, (int)LENGTH);
			Debug.Log ("hmn reset");
			hmReset = true;
		}
		if (RippleIsActive) {
			// start cou routine for rippling/blurring effect
			StartCoroutine (BlurDisturbance ());
		}
	}

	IEnumerator BlurDisturbance() {
		float n = 0;
		while (true) { // used for co-routine
			for (int ex_x = 1; ex_x < WIDTH - 1; ex_x++) {
				for (int ex_z = 1; ex_z < LENGTH - 1; ex_z++) {
					n = (( 	height_buffer_1[(ex_x + 1), ex_z] +
					      height_buffer_1[ (ex_x - 1), ex_z] +
					      height_buffer_1[ ex_x, (ex_z + 1)] +
					      height_buffer_1[ex_x,(ex_z - 1)] ) / 2f ) -
						height_buffer_2[ex_x, ex_z];
					n *= RippleDamping;
					height_buffer_original[ex_x, ex_z] -= n;
					height_buffer_2[ex_x, ex_z] = n;
				}
			}
			terrain.terrainData.SetHeights (0, 0, height_buffer_original);
			hm_temp_buf = (float[,])height_buffer_1.Clone ();
			height_buffer_1 = (float[,])height_buffer_2.Clone ();
			height_buffer_2 = (float[,])hm_temp_buf.Clone ();
			yield return new WaitForSeconds (0.07f);
		}
	}

	// Used to extrapolate the height values from the .bytes (renamed from .raw)
	// relating to the loaded map data
	public void SetTerrainHeightMap() {
		if (_MainController.ImportedMapObjectBool) {
			Debug.Log ("[Inside SetTerrainHeightMap]");
			// Path to file under resources
			string path = _MainController.MapObject["terrainRaw"]["path"] +
				_MainController.MapObject["terrainRaw"]["name"];
			try {
				Debug.Log(path);
				//Texture2D hm_tex = Resources.Load(path) as Texture2D;
				TextAsset ta = Resources.Load(path) as TextAsset;
				Debug.Log(ta);
				Stream s = new MemoryStream(ta.bytes);
				BinaryReader br = new BinaryReader(s);
				int m_heightMapRes = _MainController.MapObject["terrainRaw"]["size"].AsInt;
				int size = m_heightMapRes*m_heightMapRes*2;
				Debug.Log (m_heightMapRes + " " + size);
				byte[] data = new byte[size];
				br.Read(data, 0, size);
				br.Close();
				float[,] htmap = new float[m_heightMapRes, m_heightMapRes];
				int i=0;
				for(int x = 0 ; x < m_heightMapRes; x++) {
					for(int y = 0; y < m_heightMapRes; y++) {
						//Extract 16 bit data and normalize.
						float ht = (data[i++] + data[i++]*256.0f);
						htmap[m_heightMapRes-1-x,y] = ht / 65535.0f;
					}
				}
				Debug.Log (htmap.Length);
				Debug.Log ("[Setting height map]" + htmap[10, 10]);
				terrain.terrainData.SetHeights(0,0,htmap);
			} catch (System.Exception e) {
				Debug.LogException(e);
			}
		} else {
			Debug.Log (".raw data not loaded for height map");
		}
	}
		
	void Update () {

	}

	/*
	[RPC] public void SyncTerrain(Vector3 explosion_pos){
		ManipulateTerrain(explosion_pos, 5f, "pull");
		if (photonView.isMine)
			photonView.RPC("SyncTerrain",PhotonTargets.OthersBuffered, explosion_pos);
	}*/
	
	//Circular explosion
	public void ManipulateTerrain(Vector3 explosion_pos_orig, float explosion_radius, string force_type,
	                              float terrain_offset,float falloff,float peak_sharpness) {
		// disturbance array created fro explosion
		float[,] dist = new float[(int)WIDTH, (int)LENGTH];
		float val = 0;
		//Debug.Log (explosion_pos);
		Vector3 explosion_pos = new Vector3 (Mathf.Floor(explosion_pos_orig.x + TerrainOffset), 0, Mathf.Floor(explosion_pos_orig.z + TerrainOffset));
		//float terrain_offset = 30f; //the amount that the terrain has been offseted
		//float falloff = 2f;	//large values gives a very big center peak
		//range 1.1f - infinity
		//float peak_sharpness = 2f; //0 implies at the center is the max radius
		//  < 0 implies sharp points greater than radius
		// > 0 implies rounded and softer peak

		for (int x = (int)(explosion_pos.x - explosion_radius); x < (int)(explosion_pos.x + explosion_radius); x++) {
			for (int z = (int)(explosion_pos.z - explosion_radius); z < (int)(explosion_pos.z + explosion_radius); z++) {
				if (x < WIDTH && x >= 0 &&
				    z < LENGTH && z >= 0) {
					float radius = Mathf.Sqrt (Mathf.Pow (x - explosion_pos.x, 2) + Mathf.Pow (z - explosion_pos.z, 2));
					if (radius <= explosion_radius) {
						val = (Mathf.Pow (falloff, -(radius / explosion_radius) - peak_sharpness) * explosion_radius) / MAX_HEIGHT;;
						if (force_type == "push") {
							if (RippleIsActive) {
								height_buffer_1 [z, x] = val;
							} else {
								dist[z,x] = val;
							}
						} else if (force_type == "pull") {
							if (RippleIsActive) {
								height_buffer_1 [z, x] = -val;
							} else {
								dist[z,x] = -val;
							}
						}
						//height_buffer_1[z, x] = -explosion_radius/MAX_HEIGHT; // to make a straight up terrain
					}
				}
			}
		}
		UpdateHeightMap(dist);
	}

	private void UpdateHeightMap(float [,] dist) {
		float [,] t = terrain.terrainData.GetHeights (0, 0, (int)WIDTH, (int)LENGTH);
		// add to exisiting terrain height map
		for (int i = 0; i < (int)WIDTH; i++) {
			for (int k = 0; k < (int)LENGTH; k++) {
				t[i,k] -= dist[i,k];
			}
		}
		terrain.terrainData.SetHeights (0, 0, t);
	}
}
