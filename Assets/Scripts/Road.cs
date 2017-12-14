using UnityEngine;

namespace ProcGen {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Road : MonoBehaviour {

		public static Road CreateNew(Transform parent, Junction j1, Junction j2, float width) {
			GameObject go = new GameObject();
			go.name = "Road";
			go.transform.SetParent(parent, false);
			Road road = go.AddComponent<Road>();
			road.Junction1 = j1;
			road.Junction2 = j2;
			road.width = width;
			road.ComputePositioning();
			road.GenerateMesh();
			return road;
		}

		public static bool Intersection(Road road1, Road road2, out Vector3 intersect) {
			float dot = Vector3.Dot(road1.transform.right, road2.transform.right);
			if (dot > 1f - Mathf.Epsilon || dot < -1f + Mathf.Epsilon) { // Parallel
				intersect = Vector3.zero;
				return false;
			} else { // Line Intersect
				return MathUtils.LineLineIntersection(out intersect, road1.SegmentOrigin, road1.SegmentVec, road2.SegmentOrigin, road2.SegmentVec);
			}
		}

		public float width;
		public float length;

		private Junction _junc1;
		public Junction Junction1 {
			get {
				return _junc1;
			}
			set { // Linking
				if (_junc1 != null) {
					_junc1.roads.Remove(this);
				}
				_junc1 = value;
				if (_junc1 != null) {
					_junc1.roads.Add(this);
				}
			}
		}
		private Junction _junc2;
		public Junction Junction2 {
			get {
				return _junc2;
			}
			set { // Linking
				if (_junc2 != null) {
					_junc2.roads.Remove(this);
				}
				_junc2 = value;
				if (_junc2 != null) {
					_junc2.roads.Add(this);
				}
			}
		}

		// Components
		private MeshFilter filter;
		private MeshRenderer meshRenderer;

		private Mesh mesh;

		// Props
		public Vector3 SegmentOrigin {
			get {
				return Junction1.transform.position;
			}
		}
		public Vector3 SegmentVec {
			get {
				return Junction2.transform.position - Junction1.transform.position;
			}
		}

		private void Awake() {
			filter = GetComponent<MeshFilter>();
			mesh = filter.mesh;
			meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.material = MaterialsManager.I.roadsMat;
		}

		private void ComputePositioning() {
			transform.position = (Junction1.transform.position + Junction2.transform.position) / 2f;
			Vector3 dir = (Junction2.transform.position - Junction1.transform.position).normalized;
			transform.rotation = Quaternion.Euler(0f, Vector3.SignedAngle(Vector3.right, dir, Vector3.up), 0f);
			length = Vector3.Distance(Junction1.transform.position, Junction2.transform.position);
		}

		private void GenerateMesh() {
			mesh.GeneratePlane(new Vector2(length, width), 2, 1, true, true);
		}

		public Road Split(Vector3 splitPoint) {
			// Compute new roads
			Junction splitJunc = Junction.CreateNew(transform.parent, splitPoint);
			Junction junc2 = Junction2;
			Junction2 = splitJunc;
			Road nRoad = CreateNew(transform.parent, splitJunc, junc2, width);

			// Recalculate current road
			ComputePositioning();
			GenerateMesh();

			return nRoad;
		}

		public void Trim(Junction nDestJunction) {
			Junction2.NotifyUnlink();
			Junction2 = nDestJunction;

			// Recalculate road
			ComputePositioning();
			GenerateMesh();
		}

		public bool HasCommonJunction(Road road) {
			return (road.Junction1 == Junction1 || road.Junction1 == Junction2 || road.Junction2 == Junction1 || road.Junction2 == Junction2);
		}

	}
}
