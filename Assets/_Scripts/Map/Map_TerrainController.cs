﻿using UnityEngine;
using System.Collections;
using System.IO;

public class Map_TerrainController : Photon.MonoBehaviour {
	
	public Terrain terrain;
	public bool RippleIsActive = false;
	public float RippleDamping = 0.7f;
	public float TerrainOffset = 128f;
	// ** PRIVATE VARIABLES ** //
	private float WIDTH, LENGTH, MAX_HEIGHT;
	private float[,] fmap;
	private float[,] height_buffer_1, heights_orig, height_buffer_2,
	height_buffer_original,	// temporary buffer when swapping values
	hm_temp_buf,	// original height map (HEIGHT_MAP)
	flatten_buf; 	// used to flatten terrain
	
	void Start () {
		//Setting up local dimensions
		//Initializing starting HM
		//height_buffer_1 = new float[(int)WIDTH, (int)LENGTH];
		//height_buffer_2 = new float[(int)WIDTH, (int)LENGTH];
		//flatten_buf = new float[(int)WIDTH, (int)LENGTH];
		//height_buffer_original = new float[(int)WIDTH, (int)LENGTH];
		//if (RippleIsActive) {
			// start cou routine for rippling/blurring effect
		//	StartCoroutine (BlurDisturbance ());
		//}
	}

	public void SetTerrainTexture(int x_min, int z_min, int x_max, int z_max) {
		int x_dif = x_max - x_min; int z_dif = z_max - z_min;
		float[,,] am = terrain.terrainData.GetAlphamaps(x_min, z_min, x_dif, z_dif);
		for (int x = x_min; x < x_max; x++) {
			for (int y = z_min; y < z_max; y++) {
				//get height
				float height = terrain.terrainData.GetHeight(x, y);
				float v3 = (MAX_HEIGHT - 15 - height)/(MAX_HEIGHT - 15);
				// white
				am[x - x_min,y - z_min,1] = 1 - v3;
				// red
				am[x - x_min,y - z_min,0] = v3;
			}
		}
		terrain.terrainData.SetAlphamaps (x_min, z_min, am);
	}

	public void SetFreezeMap(){
		if (_MainController.ImportedMapObjectBool) {
			string path_f = _MainController.MapObject["terrainFreeze"]["path"] +
			_MainController.MapObject["terrainFreeze"]["name"];
			try {
				// Load in the .bytes files as TextAssets
				TextAsset ta_f = Resources.Load(path_f) as TextAsset;
				// Create a byte stream
				Stream s_f = new MemoryStream(ta_f.bytes);
				// Create a binary reader
				BinaryReader br_f = new BinaryReader(s_f);
				// Set variables for resolution and sizes of maps
				int m_freezeMapRes = _MainController.MapObject["terrainFreeze"]["size"].AsInt;
				int size_f = m_freezeMapRes*m_freezeMapRes;
				// Create byte arrays to read in to
				byte[] data_f = new byte[size_f];
				// Read in
				br_f.Read (data_f, 0, size_f);
				br_f.Close();
				// Create 2D array of floats for freeze map
				fmap = new float[m_freezeMapRes, m_freezeMapRes];
				int i = 0;
				for (int x = 0; x < m_freezeMapRes; x++) {
					for (int y = 0; y < m_freezeMapRes; y++) {
						fmap[m_freezeMapRes-x-1,y] = data_f[i++]/255.0f;  
					}
				}
			} catch (System.Exception e) {
				Debug.LogException(e);
			}
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
				// Load in the .bytes files as TextAssets
				TextAsset ta = Resources.Load(path) as TextAsset;
				// Create a byte stream
				Stream s = new MemoryStream(ta.bytes);
				// Create a binary reader
				BinaryReader br = new BinaryReader(s);
				// Set variables for resolution and sizes of maps
				int m_heightMapRes = _MainController.MapObject["terrainRaw"]["size"].AsInt;
				int size = m_heightMapRes*m_heightMapRes*2;
				// Create byte arrays to read in to
				byte[] data = new byte[size];
				// Read in
				br.Read(data, 0, size);
				br.Close();
				// Create 2D array of floats for height map
				float[,] htmap = new float[m_heightMapRes, m_heightMapRes];
				int i=0;
				for(int x = 0 ; x < m_heightMapRes; x++) {
					for(int y = 0; y < m_heightMapRes; y++) {
						//Extract 16 bit data and normalize.
						float ht = (data[i++] + data[i++]*256.0f);
						htmap[m_heightMapRes-1-x,y] = ht / 65535.0f;
					}
				}
				Debug.Log ("[Setting height map]" + htmap[10, 10]);
				terrain.terrainData.SetHeights(0,0,htmap);
				WIDTH = terrain.terrainData.size.x;
				MAX_HEIGHT = terrain.terrainData.size.y;
				LENGTH = terrain.terrainData.size.z;
			} catch (System.Exception e) {
				Debug.LogException(e);
			}
		} else {
			Debug.Log (".raw data not loaded for height map");
		}
	}

	
	//Circular explosion
	public void ManipulateTerrain(Vector3 explosion_pos_orig, float explosion_radius, string force_type,
	                              float terrain_offset,float falloff,float peak_sharpness) {
		// disturbance array created fro explosion
		float[,] dist = new float[(int)WIDTH, (int)LENGTH];
		float val = 0;
		//Debug.Log (explosion_pos);
		Vector3 explosion_pos = new Vector3 (Mathf.Floor(explosion_pos_orig.x + TerrainOffset), 0, Mathf.Floor(explosion_pos_orig.z + TerrainOffset));
		//float terrain_offset = 30f; //the amount that the terrain has been offseted
		//falloff = 20f;	//large values gives a very big center peak
		//range 1.1f - infinity
		//peak_sharpness = 0.2f; //0 implies at the center is the max radius
		//  < 0 implies sharp points greater than radius
		// > 0 implies rounded and softer peak
		int x_min = (int)explosion_pos.x - (int)explosion_radius; int x_max = (int)explosion_pos.x + (int)explosion_radius;
		int z_min = (int)explosion_pos.z - (int)explosion_radius; int z_max = (int)explosion_pos.z + (int)explosion_radius;
		for (int x = x_min; x < x_max; x++) {
			for (int z = z_min; z < z_max; z++) {
					if (x < WIDTH && x >= 0 &&
				    z < LENGTH && z >= 0) {
					float radius = Mathf.Sqrt (Mathf.Pow (x - explosion_pos.x, 2) + Mathf.Pow (z - explosion_pos.z, 2));
					if (radius <= explosion_radius) {
						val = (Mathf.Pow (falloff, -(radius / explosion_radius) - peak_sharpness) * explosion_radius) / MAX_HEIGHT;;
						if (force_type == "push") {
							dist[z,x] = val;
							/*if (RippleIsActive) {
								height_buffer_1 [z, x] = val;
							} else {
								dist[z,x] = val;
							}*/
						} else if (force_type == "pull") {
							dist[z,x] = -val;
							/*
							if (RippleIsActive) {
								height_buffer_1 [z, x] = -val;
							} else {
								dist[z,x] = -val;
							}*/
						}
						//height_buffer_1[z, x] = -explosion_radius/MAX_HEIGHT; // to make a straight up terrain
					}
				}
			}
		}
		UpdateHeightMap(dist, x_min, z_min, x_max, z_max);
	}

	private void UpdateHeightMap(float [,] dist, int x_min, int z_min, int x_max, int z_max) {
		float [,] t = terrain.terrainData.GetHeights (0, 0, (int)WIDTH, (int)LENGTH);
		// add to exisiting terrain height map
		for (int i = 0; i < (int)WIDTH; i++) {
			for (int k = 0; k < (int)LENGTH; k++) {
				t[i,k] -= dist[i,k] * fmap[i,k];
			}
		}
		terrain.terrainData.SetHeights (0, 0, t);
		SetTerrainTexture (x_min, z_min, x_max, z_max);
	}

	/*
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
	}*/
}
