using System;
using UnityEngine;

namespace ProcGen {
	public class PreparedMesh {

		private Mesh mesh;

		public Vector3[] vertices;
		public Vector3[] normals;
		public int[] indices;
		public MeshTopology topology;
		public Bounds bounds;

		public PreparedMesh(Mesh m) {
			mesh = m;
			Reset();
		}

		public void Reset() {
			vertices = new Vector3[0];
			normals = new Vector3[0];
			indices = new int[0];
			topology = MeshTopology.Triangles;
		}

		public void Apply() {
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.SetIndices(indices, topology, 0);
			//Reset();
		}

		internal void RecalculateNormals() {
			normals = new Vector3[vertices.Length];
			for (int i = 0; i < normals.Length; i++) {
				normals[i] = Vector3.zero;
			}
			for (int i = 0; i < indices.Length - 3; i += 3) {
				Vector3 faceNormal = Vector3.Cross((vertices[indices[i + 1]] - vertices[indices[i]]), (vertices[indices[i + 2]] - vertices[indices[i]]));
				normals[indices[i]] += faceNormal;
				normals[indices[i + 1]] += faceNormal;
				normals[indices[i + 2]] += faceNormal;
			}
			for (int i = 0; i < normals.Length; i++) {
				normals[i].Normalize();
			}
		}

		internal void RecalculateBounds() {
			if (vertices.Length == 0) return;
			Vector3 max;
			Vector3 min;
			max = vertices[0];
			min = vertices[0];
			for (int i = 0; i < vertices.Length; i++) {
				if (vertices[i].x > max.x) {
					max.x = vertices[i].x;
				}
				if (vertices[i].y > max.y) {
					max.y = vertices[i].y;
				}
				if (vertices[i].z > max.z) {
					max.z = vertices[i].z;
				}
				if (vertices[i].x < min.x) {
					min.x = vertices[i].x;
				}
				if (vertices[i].y < min.y) {
					min.y = vertices[i].y;
				}
				if (vertices[i].z < min.z) {
					min.z = vertices[i].z;
				}
			}
			bounds.max = max;
			bounds.min = min;
		}
	}

	public static class MeshHelper {

		public static void GeneratePlane(this Mesh mesh, Vector2 size, int quadsX, int quadsY) {
			mesh.GeneratePlane(size, quadsX, quadsY, false, false);
		}

		public static void GeneratePlane(this Mesh mesh, Vector2 size, int quadsX, int quadsY, bool centerX, bool centerY) {
			Vector3[] verts;
			int[] indices;
			GeneratePlane(out verts, out indices, size, quadsX, quadsY, centerX, centerY);

			// Mesh Update
			mesh.Clear();
			mesh.vertices = verts;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		}

		private static void GeneratePlane(out Vector3[] verts, out int[] indices, Vector2 size, int quadsX, int quadsY, bool centerX, bool centerY) {
			if (quadsX < 1) {
				quadsX = 1;
			}
			if (quadsY < 1) {
				quadsY = 1;
			}

			// Vertices
			verts = new Vector3[(quadsX + 1) * (quadsY + 1)];
			int index = 0;
			for (int y = 0; y <= quadsY; y++) { // vert loop
				for (int x = 0; x <= quadsX; x++) {
					verts[index++] = new Vector3(size.x / quadsX * x - ((centerX) ? size.x / 2f : 0f), 0, size.y / quadsY * y - ((centerY) ? size.y / 2f : 0f));
				}
			}

			// Indices
			index = 0;
			indices = new int[quadsX * quadsY * 6];
			for (int y = 0; y < quadsY; y++) { // quad loop
				for (int x = 0; x < quadsX; x++) {
					int bl = quadsX * y + y + x;
					int br = bl + 1;
					int tl = quadsX * (y + 1) + (y + 1) + x;
					int tr = tl + 1;

					indices[index++] = bl;
					indices[index++] = tl;
					indices[index++] = tr;

					indices[index++] = bl;
					indices[index++] = tr;
					indices[index++] = br;
				}
			}
		}

		public static PreparedMesh CreatePreparedMesh(this Mesh mesh) {
			return new PreparedMesh(mesh);
		}

		public static void GeneratePlane(this PreparedMesh pMesh, Vector2 size, int quadsX, int quadsY) {
			pMesh.GeneratePlane(size, quadsX, quadsY, false, false);
		}

		public static void GeneratePlane(this PreparedMesh pMesh, Vector2 size, int quadsX, int quadsY, bool centerX, bool centerY) {
			Vector3[] verts;
			int[] indices;
			GeneratePlane(out verts, out indices, size, quadsX, quadsY, centerX, centerY);

			pMesh.vertices = verts;
			pMesh.indices = indices;
			pMesh.topology = MeshTopology.Triangles;
		}

	}
}
