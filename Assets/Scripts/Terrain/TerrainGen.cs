using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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
		public bool genForest;

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
		public int maxFinalizeFrameTime = 10;

		[Header("Noise")]
		public NoiseType noiseType;
		public float frequency = 0.05f;
		public float amplitude = 50f;
		public int octaveCount = 8;
		public float lacunarity = 1.841f;
		public float gain = .5f;
		public float continentsFrequency = .0025f;
		public AnimationCurve continentValueToAmplification;
		public float forestsFrequency = .01f;
		public float forestsThreashold = .5f;
		public float forestsMaxAltitude = 40f;
		public int treesPerChunk = 20;
		public int treesPerContainer = 100;
		public GameObject treePrefab;
		public Material treeMaterial;
		[HideInInspector]
		public float offsetX;
		[HideInInspector]
		public float offsetZ;
		public MathUtils.NoiseFunction noise;

		[Header("Water")]
		public GameObject water;
		public float waterLevel = .2f;
		public float shoreHeight = 1f;

		[Header("NavMesh")]
		public float agentClimb = 3f;
		public float agentHeight = 4f;
		public float agentRadius = 5f;
		public float agentSlope = 45f;
		public float minRegionArea = 50f;
		public float boundsMaxHeight = 200f;
		public float boundsMinHeight = -50f;
		public bool overrideVoxelSize = false;
		public float voxelSize = 0f;
		public bool overrideTileSize = false;
		public int tileSize = 0;
		private NavMeshBuildSettings buildSettings;
		private NavMeshData navMeshData;
		private Bounds navMeshBounds = new Bounds();
		private AsyncOperationCallback navMeshOperation;
		private int queriedNavMeshBuilds = 0;
		private System.Diagnostics.Stopwatch navMeshWatch = new System.Diagnostics.Stopwatch();

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
		private Coroutine finalizeRoutine;
		private bool finalizeInProgress = false;

		private void Start() {
			genThread = new GenerationThread();
			genThread.Start();
			genThread.Init(this, OnGenerationFinished, continentValueToAmplification);
			if (randomSeed) {
				seed = Random.Range(0, int.MaxValue);
			}
			InitNavMeshSettings();
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
			if (navMeshOperation != null) {
				navMeshOperation.UpdateStatus();
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
			if (finalizeInProgress) {
				StopCoroutine(finalizeRoutine);
			}
			finalizeRoutine = StartCoroutine(FinalizeCoroutine());
		}

		private System.Collections.IEnumerator FinalizeCoroutine() {
			finalizeInProgress = true;
			finalizeNeeded = false;
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			foreach (Chunk chk in chunks.Values) {
				chk.Finalise();
				if (sw.ElapsedMilliseconds > maxFinalizeFrameTime) {
					sw.Reset();
					yield return null;
					sw.Start();
				}
			}
			StartNavMeshBuild();
			finalizeInProgress = false;
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
			StartLODUpdate();
		}

		private void InitNavMeshSettings() {
			navMeshData = new NavMeshData();
			navMeshBounds.min = Vector3.zero + Vector3.up * boundsMinHeight;
			navMeshBounds.max = new Vector3(chunksX * chunkSize.x, boundsMaxHeight, chunksZ * chunkSize.y);
			buildSettings.agentClimb = agentClimb;
			buildSettings.agentHeight = agentHeight;
			buildSettings.agentRadius = agentRadius;
			buildSettings.agentSlope = agentSlope;
			buildSettings.minRegionArea = minRegionArea;
			buildSettings.overrideVoxelSize = overrideVoxelSize;
			buildSettings.voxelSize = voxelSize;
			buildSettings.overrideTileSize = overrideTileSize;
			buildSettings.tileSize = tileSize;
			string[] report = buildSettings.ValidationReport(navMeshBounds);
			Debug.Log("NavMesh Settings Report:");
			foreach (string str in report) {
				Debug.Log(str);
			}
			Debug.Log("Voxel size: " + buildSettings.voxelSize);
			Debug.Log("Tile size: " + buildSettings.tileSize);
		}

		private void StartNavMeshBuild() {
			queriedNavMeshBuilds++;
			if (queriedNavMeshBuilds == 1) {
				BuildNavMesh();
			}
		}

		private void OnNavMeshBuildFinished() {
			Debug.Log("Finished Building NavMesh in " + navMeshWatch.ElapsedMilliseconds + "ms");
			navMeshWatch.Reset();
			if (queriedNavMeshBuilds > 0) {
				BuildNavMesh();
			}
		}

		private void BuildNavMesh() {
			queriedNavMeshBuilds--;
			Debug.Log("Starting NavMesh build with:\nbounds: min=" + navMeshBounds.min + " max=" + navMeshBounds.max);
			navMeshWatch.Start();
			List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
			foreach (Chunk chk in chunks.Values) {
				sources.Add(CreateNavMeshBuildSource(chk.Get3DSize(), chk.GetMesh(), chk.transform.localToWorldMatrix, NavMesh.GetAreaFromName("Walkable")));
			}
			navMeshOperation = new AsyncOperationCallback(NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, buildSettings, sources, navMeshBounds));
			navMeshOperation.callback += OnNavMeshBuildFinished;
			if (navMeshData == null) {
				Debug.Log("NavMeshBulding error");
			}
			NavMesh.AddNavMeshData(navMeshData);
		}

		private NavMeshBuildSource CreateNavMeshBuildSource(Vector3 size, Mesh mesh, Matrix4x4 transf, int area) {
			NavMeshBuildSource source = new NavMeshBuildSource();
			source.shape = NavMeshBuildSourceShape.Mesh;
			source.size = size;
			source.sourceObject = mesh;
			source.transform = transf;
			source.area = 0;
			return source;
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
			terrainMat.SetFloat("_SandFade", shoreHeight);
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
