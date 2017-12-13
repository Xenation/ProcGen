using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		private Vector2 prevSize;
		private int prevDivX;
		private int prevDivY;
		private int prevSeed;
		private float prevPerlinScale;
		private float prevPerlinPower;
		private float prevPerlinAmplitude;
		private int prevNbTowns;
		private int prevRaysY;
		private float prevMinTownRadius;
		private float prevMaxTownRadius;
		private float prevWaterLevel;

		private Mesh mesh;
		private List<GameObject> objs = new List<GameObject>();
		private List<Town> towns = new List<Town>();

		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

		private void Start() {
			if (filter == null) {
				filter = GetComponent<MeshFilter>();
			}
			if (meshCol == null) {
				meshCol = GetComponent<MeshCollider>();
			}
			mesh = filter.mesh;
			if (randomSeed) {
				seed = Random.Range(0, int.MaxValue);
			}
		}

		public void Update() {
			if (CheckChange()) {
				sw.Reset();
				sw.Start();

				Random.InitState(seed);
				GeneratePlane(size, divX, divY);
				UpdateWater();
				GenerateTerrain();
				meshCol.sharedMesh = mesh;
				GenerateTowns();

				sw.Stop();
				Debug.Log("Generation Time: " + sw.ElapsedMilliseconds + "ms");
			}
		}

		private void UpdateWater() {
			water.transform.position = new Vector3(water.transform.position.x, waterLevel, water.transform.position.z);
		}

		private bool CheckChange() {
			bool hasChanged = false;
			if (prevSize != size) {
				hasChanged = true;
				prevSize = size;
			}
			if (prevDivX != divX) {
				hasChanged = true;
				prevDivX = divX;
			}
			if (prevDivY != divY) {
				hasChanged = true;
				prevDivY = divY;
			}
			if (prevSeed != seed) {
				hasChanged = true;
				prevSeed = seed;
			}
			if (prevPerlinScale != perlinScale) {
				hasChanged = true;
				prevPerlinScale = perlinScale;
			}
			if (prevPerlinPower != perlinPower) {
				hasChanged = true;
				prevPerlinPower = perlinPower;
			}
			if (prevPerlinAmplitude != perlinAmplitude) {
				hasChanged = true;
				prevPerlinAmplitude = perlinAmplitude;
			}
			if (prevNbTowns != nbTowns) {
				hasChanged = true;
				prevNbTowns = nbTowns;
			}
			if (prevMinTownRadius != minTownRadius) {
				hasChanged = true;
				prevMinTownRadius = minTownRadius;
			}
			if (prevMaxTownRadius != maxTownRadius) {
				hasChanged = true;
				prevMaxTownRadius = maxTownRadius;
			}
			if (prevWaterLevel != waterLevel) {
				hasChanged = true;
				prevWaterLevel = waterLevel;
			}
			return hasChanged;
		}

		public void GenerateTowns() {
			ClearTowns();
			Ray ray = new Ray(Vector3.zero, Vector3.down);
			RaycastHit hit;
			while (towns.Count < nbTowns) {
				ray.origin = transform.position + new Vector3(Random.Range(transform.position.x, transform.position.x + size.x), 100f, Random.Range(transform.position.z, transform.position.z + size.y));
				float radius = Random.Range(minTownRadius, maxTownRadius);
				if (meshCol.Raycast(ray, out hit, 1000f) && hit.point.y > waterLevel && Vector3.Dot(hit.normal, Vector3.up) > .9f && TownIsFarEnough(hit.point, radius)) {
					Town town = Instantiate(townPrefab, hit.point, Quaternion.identity, transform);
					town.radius = radius;
					town.GenerateBuildings(this, buildingPrefab);
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

		public void GeneratePlane(Vector2 size, int divX, int divY) {
			if (divX < 1) {
				divX = 1;
			}
			if (divY < 1) {
				divY = 1;
			}
			// Vertices
			Vector3[] verts = new Vector3[(divX + 1) * (divY + 1)];
			int index = 0;
			for (int y = 0; y <= divY; y++) { // vert loop
				for (int x = 0; x <= divX; x++) {
					verts[index++] = new Vector3(size.x / divX * x, 0, size.y / divY * y);
				}
			}

			// Indices
			index = 0;
			int[] indices = new int[divX * divY * 6];
			for (int y = 0; y < divY; y++) { // quad loop
				for (int x = 0; x < divX; x++) {
					int bl = divX * y + y + x;
					int br = bl + 1;
					int tl = divX * (y + 1) + (y + 1) + x;
					int tr = tl + 1;

					indices[index++] = bl;
					indices[index++] = tl;
					indices[index++] = tr;

					indices[index++] = bl;
					indices[index++] = tr;
					indices[index++] = br;
				}
			}
			mesh.Clear();
			mesh.vertices = verts;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		}

	}
}
