using System.Collections.Generic;
using UnityEngine;

namespace ProcGen {
	/// <summary>
	/// Old Town Class
	/// </summary>
	public class Town : MonoBehaviour {

		public float radius;
		public AnimationCurve heightCurve;
		public float density;
		public TownTopology topology;

		private List<GameObject> buildings = new List<GameObject>();
		public Junction centralJunction;

		public void GenerateBuildings(BasicTerrainGen gen, GameObject buildingPrefab) {
			Ray ray = new Ray(Vector3.zero, Vector3.down);
			RaycastHit hit;
			int maxBuildings = (int) (radius * density);
			for (int y = 0; y <= maxBuildings; y++) {
				for (int x = 0; x <= maxBuildings; x++) {
					ray.origin = transform.position + new Vector3((x / (float) maxBuildings) * (radius * 2) - radius, 100f, (y / (float) maxBuildings) * (radius * 2) - radius);
					if (gen.meshCol.Raycast(ray, out hit, 1000f) && hit.point.y > gen.waterLevel + gen.shore && Vector3.Dot(hit.normal, Vector3.up) > .9f) {
						float distance = Vector3.Distance(hit.point, transform.position);
						if (distance > radius) continue;
						GameObject go = Instantiate(buildingPrefab, hit.point, Quaternion.identity, transform);
						Vector3 scale = go.transform.localScale;
						scale.y *= heightCurve.Evaluate(distance / radius) * radius;
						scale.y += Random.Range(0f, 1f);
						go.transform.localScale = scale;
						go.transform.position += Vector3.up * (scale.y / 2);
						buildings.Add(go);
					}
				}
			}
		}

		private void ClearBuildings() {
			for (int i = 0; i < buildings.Count; i++) {
				Destroy(buildings[i]);
			}
			buildings.Clear();
		}

		public void GenerateRoads(BasicTerrainGen gen) {
			centralJunction = Junction.CreateNew(transform);
			centralJunction.GenerateRoads(this, gen.centerRoadsCount, gen.minRoadsLength, gen.maxRoadsLength, gen.centerRoadsWidth);
		}

	}
}
