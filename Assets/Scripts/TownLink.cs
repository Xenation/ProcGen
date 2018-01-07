using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ProcGen {
	public class TownLink {

		public Townv2 town1;
		public Townv2 town2;

		public NavMeshPath path;
		public float PathDistance {
			get {
				if (path.status != NavMeshPathStatus.PathComplete || path.corners.Length < 2) return 10000000f;
				float distance = 0f;
				Vector3 prevCorner = path.corners[0];
				for (int i = 1; i < path.corners.Length; i++) {
					distance += Vector3.Distance(prevCorner, path.corners[i]);
					prevCorner = path.corners[i];
				}
				return distance;
			}
		}
		private List<Junction> junctions = new List<Junction>();

		public bool RoadCreated { get; private set; }

		public TownLink(Townv2 t1, Townv2 t2) {
			town1 = t1;
			town2 = t2;
			RoadCreated = false;
		}

		public bool IsLinkingTown(Townv2 town) {
			return town1 == town || town2 == town;
		}

		public Townv2 Other(Townv2 t) {
			return (t == town1) ? town2 : town1;
		}

		public bool ComputePath() {
			path = new NavMeshPath();
			NavMesh.CalculatePath(town1.transform.position, town2.transform.position, NavMesh.AllAreas, path);
			return path.status == NavMeshPathStatus.PathComplete;
		}

		public void CreateHighway(float width, float subdivDistance, TerrainGen terrain) {
			CreateJunctions();
			CreateRoads(width, subdivDistance, terrain);
			RoadCreated = true;
		}

		private void CreateJunctions() {
			Vector3[] corners = path.corners;
			if (corners.Length < 2) return;
			for (int i = 0; i < corners.Length; i++) {
				junctions.Add(Junction.CreateNew(town1.transform, town1.transform.worldToLocalMatrix.MultiplyPoint(corners[i])));
			}
		}

		private void CreateRoads(float width, float subdivDistance, TerrainGen terrain) {
			if (junctions.Count < 2) return;
			Junction prevJunc = junctions[0];
			for (int i = 1; i < junctions.Count; i++) {
				prevJunc.CreateRoadTo(junctions[i], width, true, subdivDistance, terrain);
				prevJunc = junctions[i];
			}
		}

	}
}
