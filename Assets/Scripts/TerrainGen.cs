using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProcGen {
	public enum NoiseType {
		Perlin,
		PerlinAbs,
		PerlinOneMinusAbs
	}

	public class TerrainGen : MonoBehaviour {
		
		[Header("General")]
		public Material terrainMat;
		public int seed = 111;
		public bool randomSeed = false;
		private bool regenerate = true;

		[Header("Chunks")]
		public int chunksX;
		public int chunksZ;
		public Vector2 chunkSize;
		[Range(1, 250)]
		public int chunkQuadsX;
		[Range(1, 250)]
		public int chunkQuadsZ;
		public int lodQuadsDivider = 2;
		public int lodChunkDistance = 1;
		public int maxLOD = 20;
		private Dictionary<Vector2i, Chunk> chunks = new Dictionary<Vector2i, Chunk>();

		[Header("Noise")]
		public NoiseType noiseType;
		public float frequency = 0.05f;
		public float amplitude = 50f;
		public int octaveCount = 8;
		public float lacunarity = 1.841f;
		public float gain = .5f;
		[HideInInspector]
		public float offsetX;
		[HideInInspector]
		public float offsetZ;
		public MathUtils.NoiseFunction noise;

		[Header("Water")]
		public GameObject water;
		public float waterLevel = .2f;
		public float shore = .1f;

		[Header("Biomes")]
		public float mountainessFrequency = .05f;

		private void Start() {
			if (randomSeed) {
				seed = Random.Range(0, int.MaxValue);
			}
			Generate();
		}

		private void Update() {
			if (regenerate) {
				Generate();
				regenerate = false;
			}
			TrackCamera();
		}

		private void OnValidate() {
			if (!EditorApplication.isPlaying) return;

			regenerate = true;
		}

		private Vector2i camChunkPos;
		private void TrackCamera() {
			if (SceneView.currentDrawingSceneView != null) {
				Vector3 camPos = SceneView.currentDrawingSceneView.camera.transform.position;
				Vector2i prevCamChunkPos = camChunkPos;
				camChunkPos = new Vector2i((int) (camPos.x / chunkSize.x), (int) (camPos.z / chunkSize.y));
				if (prevCamChunkPos != camChunkPos) {
					UpdateLOD();
				}
			}
		}

		private void UpdateLOD() {
			// Generation Pass
			foreach (Chunk chk in chunks.Values) {
				int prevLOD = chk.lod;
				chk.lod = (int) (Vector2i.Distance(chk.ChunkPos, camChunkPos) / lodChunkDistance);
				if (chk.lod > maxLOD) {
					chk.lod = maxLOD;
				}
				if (prevLOD != chk.lod) {
					chk.Generate();
				}
			}
			// Edge Seams Pass
			foreach (Chunk chk in chunks.Values) {
				chk.FixEdgeSeams();
			}
		}

		private void Generate() {
			InitChunks();
			Random.InitState(seed);
			offsetX = Random.Range(-10000f, 10000f);
			offsetZ = Random.Range(-10000f, 10000f);
			UpdateWater();
			ResetNoiseFunc();
			//GenerateChunks();
			UpdateLOD();
		}

		private void ResetNoiseFunc() {
			switch (noiseType) {
				case NoiseType.Perlin:
					noise = MathUtils.Perlin;
					break;
				case NoiseType.PerlinAbs:
					noise = MathUtils.PerlinAbs;
					break;
				case NoiseType.PerlinOneMinusAbs:
					noise = MathUtils.PerlinOneMinusAbs;
					break;
			}
		}

		private void GenerateChunks() {
			foreach (Chunk chk in chunks.Values) {
				chk.Generate();
			}
			foreach (Chunk chk in chunks.Values) {
				chk.FixEdgeSeams();
			}
		}

		private void UpdateWater() {
			Vector2 center = new Vector2(chunkSize.x * chunksX, chunkSize.y * chunksZ) / 2f;
			water.transform.position = new Vector3(center.x, waterLevel, center.y);
			water.transform.localScale = new Vector3(chunkSize.x * chunksX / 10f, 1f, chunkSize.y * chunksZ / 10f);
			terrainMat.SetFloat("_WaterLevel", waterLevel);
		}

		private void InitChunks() {
			DestroyChunks();
			for (int z = 0; z < chunksZ; z++) {
				for (int x = 0; x < chunksX; x++) {
					Vector2i chkPos = new Vector2i(x, z);
					chunks.Add(chkPos, Chunk.Create(this, chkPos, chunkSize, chunkQuadsX, chunkQuadsZ));
				}
			}
		}

		private void DestroyChunks() {
			foreach (Chunk chk in chunks.Values) {
				Destroy(chk.gameObject);
			}
			chunks.Clear();
		}

		public Chunk GetChunkAt(Vector2i pos) {
			Chunk chk;
			chunks.TryGetValue(pos, out chk);
			return chk;
		}

	}
}
