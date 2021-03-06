﻿using UnityEngine;
using System.Collections;
using System.IO;

public class Map_TerrainController : Photon.MonoBehaviour {
	
	public Terrain terrain;
	public bool RippleIsActive = false;
	public float RippleDamping = 0.7f;
	public float TerrainOffset = 64f;
	// ** PRIVATE VARIABLES ** //
	private int WIDTH, LENGTH, MAX_HEIGHT;
	private bool SetHeightMapBool = false;
	private float[,] fmap, cmap, htmap;
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
				float v3 = (MAX_HEIGHT - 10 - height)/(MAX_HEIGHT - 10);
				// white
				am[y - z_min,x - x_min,1] = 1 - v3;
				// red
				am[y - z_min,x - x_min,0] = v3;
			}
		}
		terrain.terrainData.SetAlphamaps (x_min, z_min, am);
	}

	public void SetClampMap(){
		if (_MainController.ImportedMapObjectBool && SetHeightMapBool) {
			string path_c = _MainController.MapObject["terrainClamp"]["path"] +
				_MainController.MapObject["terrainClamp"]["name"];
			try {
				// Load in the .bytes files as TextAssets
				Debug.Log ("clampmap path: " + path_c);
				TextAsset ta_c = Resources.Load(path_c) as TextAsset;
				// Create a byte stream
				Stream s_c = new MemoryStream(ta_c.bytes);
				// Create a binary reader
				BinaryReader br_c = new BinaryReader(s_c);
				// Set variables for resolution and sizes of maps
				int m_clampMapRes = _MainController.MapObject["terrainClamp"]["size"].AsInt;
				int size_c = m_clampMapRes*m_clampMapRes;
				// Create byte arrays to read in to
				byte[] data_c = new byte[size_c];
				// Read in
				br_c.Read (data_c, 0, size_c);
				br_c.Close();
				// Create 2D array of floats for clamp map
				cmap = new float[m_clampMapRes, m_clampMapRes];
				int i = 0;
				for (int x = 0; x < m_clampMapRes; x++) {
					for (int y = 0; y < m_clampMapRes; y++) {
						if (data_c[i++]/255.0f < 0.1f) {
							//Must be clamped
							cmap[m_clampMapRes-x-1,y] = htmap[m_clampMapRes-x-1,y];
						} else {
							cmap[m_clampMapRes-x-1,y] = MAX_HEIGHT;
						}

					}
				}
			} catch (System.Exception e) {
				Debug.LogException(e);
			}
		} else {
			Debug.Log ("[SetClampMap] - Level details not loaded OR height map not set.");
		}
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
				htmap = new float[m_heightMapRes, m_heightMapRes];
				int i=0;
				for(int x = 0 ; x < m_heightMapRes; x++) {
					for(int y = 0; y < m_heightMapRes; y++) {
						//Extract 16 bit data and normalize.
						float ht = (data[i++] + data[i++]*256.0f);
						htmap[x,y] = ht / 65535.0f;
					}
				}
				Debug.Log ("[Setting height map]" + htmap[10, 10]);
				terrain.terrainData.SetHeights(0,0,htmap);
				SetHeightMapBool = true;
				WIDTH = (int)terrain.terrainData.size.x;
				MAX_HEIGHT = (int)terrain.terrainData.size.y;
				LENGTH = (int)terrain.terrainData.size.z;
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
		float val = 0;
		int hm_w = (int)terrain.terrainData.heightmapWidth;
		int hm_h = (int)terrain.terrainData.heightmapHeight;
		int hm_scale = hm_w / WIDTH;
		Vector3 explosion_pos = new Vector3 (Mathf.Floor(explosion_pos_orig.x + TerrainOffset) * hm_scale,
		                                     0,
		                                     Mathf.Floor(explosion_pos_orig.z + TerrainOffset) * hm_scale);
		//float terrain_offset = 30f; //the amount that the terrain has been offseted
		//falloff = 20f;	//large values gives a very big center peak
		//range 1.1f - infinity
		//peak_sharpness = 0.2f; //0 implies at the center is the max radius
		//  < 0 implies sharp points greater than radius
		// > 0 implies rounded and softer peak
		int x_min = (int)explosion_pos.x - (int)explosion_radius * hm_scale; int x_max = (int)explosion_pos.x + (int)explosion_radius * hm_scale;
		int z_min = (int)explosion_pos.z - (int)explosion_radius * hm_scale; int z_max = (int)explosion_pos.z + (int)explosion_radius * hm_scale;
		// disturbance array created fro explosion
		float[,] dist = new float[hm_w, hm_h];
		for (int x = x_min; x < x_max; x++) {
			for (int z = z_min; z < z_max; z++) {
					if (x < hm_w && x >= 0 &&
				    z < hm_h && z >= 0) {
					float radius = Mathf.Sqrt (Mathf.Pow (x - explosion_pos.x, 2) + Mathf.Pow (z - explosion_pos.z, 2));
					if (radius <= explosion_radius) {
						val = (Mathf.Pow (falloff, -(radius / explosion_radius) - peak_sharpness) * explosion_radius) / MAX_HEIGHT;
			
							//val = -1*(Mathf.Pow (-falloff,(explosion_radius) + peak_sharpness)) / MAX_HEIGHT;
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
		int hm_w = terrain.terrainData.heightmapWidth;
		int hm_h = terrain.terrainData.heightmapHeight;
		float [,] t = terrain.terrainData.GetHeights (0, 0, (int)hm_w, (int)hm_h);
		float nh;
		float n = 0;
		float [,] hb = (float[,])dist.Clone ();
		float [,] hb_2 = new float[hm_w, hm_h];
		// blur the disturbance
		for (int ex_x = 1; ex_x < hm_w - 1; ex_x++) {
			for (int ex_z = 1; ex_z < hm_h - 1; ex_z++) {
				n = (( 	hb[(ex_x + 1), ex_z] +
				      hb[ (ex_x - 1), ex_z] +
				      hb[ ex_x, (ex_z + 1)] +
				      hb[ex_x,(ex_z - 1)] ) / 2f ) -
					hb_2[ex_x, ex_z];
				n *= RippleDamping;
				t[ex_x, ex_z] -= n;
				hb_2[ex_x, ex_z] = n;
			}
		}
		// add to exisiting terrain height map
		/*for (int i = 0; i < (int)hm_w; i++) {
			for (int k = 0; k < (int)hm_h; k++) {
				/*(nh = t[i,k] - dist[i,k] * fmap[i,k];
				if (nh > cmap[i,k]) {
					t[i,k] = cmap[i,k];
				} else {
					t[i,k] = nh;
				}
				t[i,k] -= dist[i,k];
			}
		}*/
		terrain.terrainData.SetHeights (0, 0, t);
		// Blur map
		//SetTerrainTexture (x_min, z_min, x_max, z_max);
	}

	/*
	void BlurDisturbance() {
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
