using System.Collections.Generic;
using UnityEngine;

namespace ProcGen {
	[RequireComponent(typeof(TerrainGen))]
	public class TownGen : MonoBehaviour {

		public int townDensity = 30;
		public float townSlopeThreashold = .9f;
		public float maxTownLinkDistance = 200f;
		public int maxTownLinkCount = 4;
		public float highwayWidth = 3f;
		public float highwaySubdivDistance = 1f;
		public GameObject townDebug;

		public TerrainGen terrainGen;

		private bool terrainGenComplete = false;
		private bool regenerate = false;

		private List<Townv2> towns = new List<Townv2>();
		private GameObject townContainer;

		private void Awake() {
			terrainGen = GetComponent<TerrainGen>();
			terrainGen.OnTerrainGenCompleteEvent += OnTerrainGenComplete;
		}

		private void Update() {
			if (terrainGenComplete && regenerate) {
				Generate();
				regenerate = false;
			}
		}

		private void OnTerrainGenComplete() {
			terrainGenComplete = true;
			regenerate = true;
		}

		private void OnValidate() {
			regenerate = true;
		}

		private void Generate() {
			Random.InitState(terrainGen.seed);
			InitTowns();
			CreateTowns();
			LinkTowns();
			DeleteUnlinkedTowns();
		}

		private void InitTowns() {
			if (townContainer != null) {
				Destroy(townContainer);
			}
			towns.Clear();
			townContainer = new GameObject("Towns");
			townContainer.transform.SetParent(transform, false);
		}

		private void CreateTowns() {
			for (int i = 0; i < townDensity; i++) {
				Vector3 townPosition = new Vector3(Random.Range(0f, terrainGen.chunkSize.x * terrainGen.chunksX), 0f, Random.Range(0f, terrainGen.chunkSize.y * terrainGen.chunksZ));
				RaycastHit hit;
				if (terrainGen.DownRaycast(townPosition, out hit) && hit.point.y > terrainGen.waterLevel && Vector3.Dot(Vector3.up, hit.normal) > townSlopeThreashold) {
					townPosition.y = hit.point.y;
					towns.Add(Townv2.CreateNew(this, townContainer.transform, townPosition, townDebug));
				}
			}
		}

		private void LinkTowns() {
			foreach (Townv2 town in towns) {
				town.Link(towns);
			}
			foreach (Townv2 town in towns) {
				town.CreateLinkHighways();
			}
		}

		private void DeleteUnlinkedTowns() {
			foreach (Townv2 town in towns) {
				if (!town.HasLinks()) {
					Destroy(town.gameObject);
				}
			}
		}

	}
}
