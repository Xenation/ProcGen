using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProcGen {
	public class BasicTerrainGen : MonoBehaviour {

		[Header("Mesh")]
		public MeshFilter filter;
		public MeshCollider meshCol;
		public Vector2 size;
		[Range(1, 300)]
		public int divX;
		[Range(1, 300)]
		public int divY;
		public Material terrainMat;

		[Header("Terrain Perlin")]
		public int seed = 0;
		public bool randomSeed = true;
		public float perlinScale = 0.5f;
		public float perlinPower = 3f;
		public float perlinAmplitude = 2f;

		[Header("Towns")]
		public GameObject buildingPrefab;
		public Town townPrefab;
		public int nbTowns = 10;
		public float minTownRadius = 2.5f;
		public float maxTownRadius = 15f;

		[Header("Water")]
		public GameObject water;
		public float waterLevel = .2f;
		public float shore = .1f;

		[Header("Roads")]
		public int centerRoadsCount = 4;
		public float minRoadsLength = 1f;
		public float maxRoadsLength = 5f;
		public float centerRoadsWidth = .3f;

		private Mesh mesh;
		private MeshRenderer meshRenderer;
		private Material runtimeMat;
		private bool regenerate = true;
		private List<Town> towns = new List<Town>();

		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

		private void Awake() {
			if (filter == null) {
				filter = GetComponent<MeshFilter>();
			}
			if (meshCol == null) {
				meshCol = GetComponent<MeshCollider>();
			}
			mesh = filter.mesh;
			meshRenderer = GetComponent<MeshRenderer>();
			runtimeMat = new Material(terrainMat);
			meshRenderer.material = runtimeMat;
		}

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
		}

		private void OnValidate() {
			if (!EditorApplication.isPlaying) return;

			regenerate = true;
		}

		private void Generate() {
			sw.Reset();
			sw.Start();

			Random.InitState(seed);
			mesh.GeneratePlane(size, divX, divY);
			UpdateWater();
			GenerateTerrain();
			meshCol.sharedMesh = mesh;
			GenerateTowns();

			sw.Stop();
			Debug.Log("Generation Time: " + sw.ElapsedMilliseconds + "ms");
		}

		private void UpdateWater() {
			water.transform.position = new Vector3(water.transform.position.x, waterLevel, water.transform.position.z);
			runtimeMat.SetFloat("_WaterLevel", waterLevel);
		}

		public void GenerateTowns() {
			ClearTowns();
			Ray ray = new Ray(Vector3.zero, Vector3.down);
			RaycastHit hit;
			while (towns.Count < nbTowns) {
				ray.origin = transform.position + new Vector3(Random.Range(transform.position.x, transform.position.x + size.x), 100f, Random.Range(transform.position.z, transform.position.z + size.y));
				float radius = Random.Range(minTownRadius, maxTownRadius);
				if (meshCol.Raycast(ray, out hit, 1000f) && hit.point.y > waterLevel + shore && Vector3.Dot(hit.normal, Vector3.up) > .9f && TownIsFarEnough(hit.point, radius)) {
					Town town = Instantiate(townPrefab, hit.point, Quaternion.identity, transform);
					town.radius = radius;
					town.GenerateBuildings(this, buildingPrefab);
					town.GenerateRoads(this);
					towns.Add(town);
				}
			}
		}

		private bool TownIsFarEnough(Vector3 pos, float radius) {
			for (int i = 0; i < towns.Count; i++) {
				if (Vector3.Distance(pos, towns[i].transform.position) - (radius + towns[i].radius) < 0) {
					return false;
				}
			}
			return true;
		}

		private void ClearTowns() {
			for (int i = 0; i < towns.Count; i++) {
				Destroy(towns[i].gameObject);
			}
			towns.Clear();
		}

		public void GenerateTerrain() {
			float offsetX = Random.Range(-100000f, 100000f);
			float offsetZ = Random.Range(-100000f, 100000f);
			Vector3[] verts = mesh.vertices;
			for (int i = 0; i < verts.Length; i++) {
				verts[i].y = Mathf.Pow(Mathf.PerlinNoise(verts[i].x * perlinScale + offsetX, verts[i].z * perlinScale + offsetZ), perlinPower) * perlinAmplitude;
			}
			mesh.vertices = verts;
			mesh.RecalculateNormals();
		}

	}
}
