using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProcGen {
	public enum NoiseType {
		Perlin,
		PerlinAbs,
		PerlinOneMinusAbs
	}

	[System.Serializable]
	public class LODLevel {

		[Tooltip("must be a power of 2")]
		public int quads;
		[Tooltip("distance in chunk grid positions")]
		public int distance;

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
		[SerializeField]
		private LODLevel[] lodLevels;
		public bool useSceneViewCameraForLOD = true;
		public Player player;
		public Dictionary<Vector2i, Chunk> chunks = new Dictionary<Vector2i, Chunk>();

		[Header("Noise")]
		public NoiseType noiseType;
		public float frequency = 0.05f;
		public float amplitude = 50f;
		public int octaveCount = 8;
		public float lacunarity = 1.841f;
		public float gain = .5f;
		public float continentsFrequency = .0025f;
		public AnimationCurve continentValueToAmplification;
		[HideInInspector]
		public float offsetX;
		[HideInInspector]
		public float offsetZ;
		public MathUtils.NoiseFunction noise;

		[Header("Water")]
		public GameObject water;
		public float waterLevel = .2f;

		private GenerationThread genThread;
		private bool _finalizeNeeded = false;
		private object _finalizeNeededLock = new object();
		private bool finalizeNeeded {
			get {
				lock (_finalizeNeededLock) {
					return _finalizeNeeded;
				}
			}
			set {
				lock (_finalizeNeededLock) {
					_finalizeNeeded = value;
				}
			}
		}

		private void Start() {
			genThread = new GenerationThread();
			genThread.Start();
			genThread.Init(this, OnGenerationFinished);
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
			if (finalizeNeeded) {
				FinalizeChunks();
			}
		}

		private void OnDestroy() {
			genThread.Stop();
		}

		private void OnValidate() {
			if (!EditorApplication.isPlaying) return;

			regenerate = true;
		}

		private Vector2i camChunkPos;
		private void TrackCamera() {
			if (useSceneViewCameraForLOD) {
				if (SceneView.currentDrawingSceneView != null) {
					Vector3 camPos = SceneView.currentDrawingSceneView.camera.transform.position;
					Vector2i prevCamChunkPos = camChunkPos;
					camChunkPos = new Vector2i((int) (camPos.x / chunkSize.x), (int) (camPos.z / chunkSize.y));
					if (prevCamChunkPos != camChunkPos) {
						StartLODUpdate();
					}
				}
			} else {
				Vector3 camPos = player.transform.position;
				Vector2i prevCamChunkPos = camChunkPos;
				camChunkPos = new Vector2i((int) (camPos.x / chunkSize.x), (int) (camPos.z / chunkSize.y));
				if (prevCamChunkPos != camChunkPos) {
					StartLODUpdate();
				}
			}
		}

		private void StartLODUpdate() {
			if (!genThread.HasFinished) {
				genThread.Cancel();
				FinalizeChunks();
			}
			genThread.Generate(GetChunksList());
		}

		private void OnGenerationFinished() {
			finalizeNeeded = true;
		}

		private void FinalizeChunks() {
			foreach (Chunk chk in chunks.Values) {
				chk.Finalise();
			}
			finalizeNeeded = false;
		}

		public int GetLODIndex(Chunk chk) {
			return GetLODIndexFromDistance((int) Vector2i.Distance(chk.ChunkPos, camChunkPos));
		}

		private int GetLODIndexFromDistance(int distance) {
			int curLODIndex = 0;
			for (int i = 0; i < lodLevels.Length; i++) {
				if (distance < lodLevels[i].distance) {
					break;
				}
				curLODIndex = i;
			}
			return curLODIndex;
		}

		public LODLevel GetLODLevel(int index) {
			if (index < 0) return lodLevels[lodLevels.Length - 1]; // TODO unsafe
			return lodLevels[index];
		}

		private void Generate() {
			InitChunks();
			Random.InitState(seed);
			offsetX = Random.Range(-10000f, 10000f);
			offsetZ = Random.Range(-10000f, 10000f);
			UpdateWater();
			ResetNoiseFunc();
			//GenerateChunks();
			StartLODUpdate();
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
					chunks.Add(chkPos, Chunk.Create(this, chkPos, chunkSize));
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

		public List<Chunk> GetChunksList() {
			List<Chunk> chks = new List<Chunk>();
			foreach (Chunk chk in chunks.Values) {
				chks.Add(chk);
			}
			return chks;
		}

	}
}
