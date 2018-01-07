using System.Collections.Generic;
using UnityEngine;

namespace ProcGen {
	public class Townv2 : MonoBehaviour {

		public static Townv2 CreateNew(TownGen gen, Transform parent, Vector3 position, GameObject debug) {
			GameObject go = new GameObject("Town");
			go.transform.SetParent(parent);
			go.transform.position = position;
			if (debug != null) {
				Instantiate(debug, go.transform).transform.localPosition = Vector3.zero;
			}
			Townv2 town = go.AddComponent<Townv2>();
			town.Init(gen);
			return town;
		}

		private TownGen generator;

		private List<TownLink> links = new List<TownLink>();

		public float radius;
		private GameObject buildingContainer;
		private List<Building> buildings = new List<Building>();
		
		private void Init(TownGen gen) {
			generator = gen;
		}

		public bool HasLinkTo(Townv2 to) {
			foreach (TownLink lnk in links) {
				if (lnk.IsLinkingTown(to)) {
					return true;
				}
			}
			return false;
		}
		
		public bool HasLinks() {
			return links.Count > 0;
		}

		public void Link(List<Townv2> towns) {
			foreach (Townv2 town in towns) {
				if (town == this) continue;
				if (Vector3.Distance(transform.position, town.transform.position) < generator.maxTownLinkDistance && !town.HasLinkTo(this)) {
					TownLink lnk = new TownLink(this, town);
					if (lnk.ComputePath()) { // Link only if path found
						links.Add(lnk);
						town.links.Add(lnk);
					}
				}
			}
			TrimLinks();
		}

		private void TrimLinks() {
			if (links.Count < generator.maxTownLinkCount) return;
			float[] minDistances = new float[generator.maxTownLinkCount];
			for (int i = 0; i < minDistances.Length; i++) {
				minDistances[i] = 1000000f;
			}
			int[] indexes = new int[generator.maxTownLinkCount];
			for (int i = 0; i < indexes.Length; i++) {
				indexes[i] = -1;
			}
			for (int lnkIndex = 0; lnkIndex < links.Count; lnkIndex++) {
				TownLink lnk = links[lnkIndex];
				float dist = lnk.PathDistance;
				for (int i = 0; i < minDistances.Length; i++) {
					if (dist < minDistances[i]) { // shorter
						for (int j = minDistances.Length - 1; j > i; j--) { // Offset
							minDistances[j] = minDistances[j - 1];
							indexes[j] = indexes[j - 1];
						}
						minDistances[i] = dist;
						indexes[i] = lnkIndex;
						break;
					}
				}
			}
			for (int lnkIndex = 0; lnkIndex < links.Count; lnkIndex++) { // Unlink further away links
				bool keep = false;
				for (int i = 0; i < indexes.Length; i++) {
					if (indexes[i] == -1) break;
					if (lnkIndex == indexes[i]) {
						keep = true;
						break;
					}
				}
				if (!keep) {
					links[lnkIndex].Other(this).links.Remove(links[lnkIndex]);
				}
			}
			List<TownLink> nLinks = new List<TownLink>();
			foreach (int index in indexes) {
				if (index < 0) break;
				nLinks.Add(links[index]);
			}
			links = nLinks;
		}

		public void CreateLinkHighways() {
			foreach (TownLink lnk in links) {
				if (lnk.RoadCreated) continue; // Already created -> skip
				lnk.CreateHighway(generator.highwayWidth, generator.highwaySubdivDistance, generator.terrainGen);
			}
		}

		public void GenerateTown() {
			InitBuildingStats();
			GenerateSimpleTown();
		}

		private void InitBuildingStats() {
			radius = Random.Range(generator.townMinRadius, generator.townMaxRadius);
			buildingContainer = new GameObject("Buildings");
			buildingContainer.transform.SetParent(transform);
		}

		private void GenerateSimpleTown() {
			int maxBuildingsRow = Mathf.CeilToInt(radius / generator.buildingSpacing);
			for (int y = 0; y < maxBuildingsRow; y++) {
				for (int x = 0; x < maxBuildingsRow; x++) {
					Vector3 buildingPos = transform.position + new Vector3((x / (float) maxBuildingsRow) * (radius * 2) - radius, 0f, (y / (float) maxBuildingsRow) * (radius * 2) - radius);
					RaycastHit hit;
					if (generator.terrainGen.DownRaycast(buildingPos, out hit) && hit.point.y > generator.terrainGen.waterLevel + generator.terrainGen.shoreHeight && Vector3.Dot(Vector3.up, hit.normal) > generator.buildingSlopeThreashold) {
						buildingPos.y = hit.point.y;
						float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(buildingPos.x, buildingPos.z));
						if (distance > radius) continue;
						float buildingHeight = generator.buildingHeightCurve.Evaluate(1f - distance / radius) * generator.heightMultiplierFromRadius.Evaluate((radius - generator.townMinRadius) / generator.townMaxRadius) * Random.Range(.75f, 1.25f);
						buildings.Add(Building.CreateNew(buildingContainer.transform, generator.baseBuilding, buildingPos, buildingHeight));
					}
				}
			}
		}

	}
}
