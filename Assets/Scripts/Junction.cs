using System.Collections.Generic;
using UnityEngine;

namespace ProcGen {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Junction : MonoBehaviour {

		public static Junction CreateNew(Transform parent) {
			return CreateNew(parent, Vector3.zero);
		}

		public static Junction CreateNew(Transform parent, Vector3 pos) {
			GameObject go = new GameObject();
			go.name = "Junc";
			go.transform.SetParent(parent, false);
			go.transform.localPosition = pos;
			Junction junc = go.AddComponent<Junction>();
			return junc;
		}

		public List<Road> roads = new List<Road>();

		// Components
		private MeshFilter filter;
		private MeshRenderer meshRenderer;

		private Mesh mesh;

		public void Awake() {
			filter = GetComponent<MeshFilter>();
			mesh = filter.mesh;
			meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material = MaterialsManager.I.junctionsMat;
		}

		public void CreateRoadTo(Junction junc, float width, bool followGround, float subdivDistance, TerrainGen terrain) {
			Road road = Road.CreateNew(transform.parent, this, junc, width, followGround, subdivDistance, terrain);
			roads.Add(road);
		}

		public void NotifyUnlink() {
			if (roads.Count == 0) {
				Destroy(gameObject);
			}
		}

		public void GenerateRoads(Town town, int nbRoads, float minLength, float maxLength, float width) {
			switch (town.topology) {
				case TownTopology.Linear:
					Dictionary<Orientation, Junction> juncs = GenerateLinearRoads(town, Orientation.None, minLength, maxLength, width);
					foreach (KeyValuePair<Orientation, Junction> pair in juncs) {
						pair.Value.GenerateLinearRoads(town, pair.Key.GetOposite(), minLength, maxLength, width);
					}
					return;
				case TownTopology.Random:
					GenerateRandomRoads(nbRoads, minLength, maxLength, width);
					return;
				case TownTopology.Circular:
					GenerateCircularRoads();
					return;
			}
		}

		private Dictionary<Orientation, Junction> GenerateLinearRoads(Town town, Orientation exclude, float minLength, float maxLength, float width) {
			Dictionary<Orientation, Junction> createdJuncs = new Dictionary<Orientation, Junction>();
			for (int i = 0; i < 4; i++) {
				if ((Orientation) i == exclude) continue;
				float distance = Random.Range(minLength, maxLength);
				Vector3 nJuncPos = transform.localPosition + Quaternion.Euler(0f, 90 * i, 0f) * (Vector3.forward * distance);
				Junction nJunc = CreateNew(transform.parent, nJuncPos);
				Road road = Road.CreateNew(transform.parent, this, nJunc, width);
				createdJuncs.Add((Orientation) i, nJunc);
			}
			return createdJuncs;
		}

		private void GenerateRandomRoads(int nbRoads, float minLength, float maxLength, float width) {
			for (int i = 0; i < nbRoads; i++) {
				float distance = Random.Range(minLength, maxLength);
				float angle = Random.Range(0f, 360f);
				Vector3 nJuncPos = Quaternion.Euler(0f, angle, 0f) * (Vector3.right * distance);
				Junction nJunc = CreateNew(transform.parent, nJuncPos);
				Road road = Road.CreateNew(transform.parent, this, nJunc, width);
			}
		}

		private void GenerateCircularRoads() {

		}

	}
}
